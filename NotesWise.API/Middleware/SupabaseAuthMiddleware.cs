using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NotesWise.API.Middleware;

public class SupabaseAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SupabaseAuthMiddleware> _logger;
    private readonly string _supabaseJwtSecret;

    public SupabaseAuthMiddleware(RequestDelegate next, ILogger<SupabaseAuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _supabaseJwtSecret = configuration["Supabase:JwtSecret"] ?? throw new InvalidOperationException("Supabase JWT Secret not configured");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractBearerToken(context.Request);
        if (!string.IsNullOrEmpty(token))
        {
            var userId = ValidateSupabaseToken(token);
            if (!string.IsNullOrEmpty(userId))
            {
                context.Items["UserId"] = userId;
                
                // Add user claim for authorization
                var claims = new List<Claim>
                {
                    new("sub", userId),
                    new("user_id", userId)
                };
                
                var identity = new ClaimsIdentity(claims, "Supabase");
                context.User = new ClaimsPrincipal(identity);
            }
        }

        await _next(context);
    }

    private static string? ExtractBearerToken(HttpRequest request)
    {
        var authorizationHeader = request.Headers.Authorization.FirstOrDefault();
        if (authorizationHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
        {
            return authorizationHeader["Bearer ".Length..].Trim();
        }
        return null;
    }

    private string? ValidateSupabaseToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create token validation parameters using the Supabase JWT secret
            tokenHandler.MapInboundClaims = false;
            tokenHandler.InboundClaimTypeMap.Clear();
            tokenHandler.OutboundClaimTypeMap.Clear();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = "https://tybjkxyebirmzkuxtiuh.supabase.co/auth/v1",
                ValidateAudience = true,
                ValidAudience = "authenticated",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes clock skew
            };

            // Validate the token
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            // Extract user ID from the 'sub' claim
            var userId =
                principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                (validatedToken as JwtSecurityToken)?.Subject;
                
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Token does not contain a valid user ID");
                return null;
            }

            _logger.LogDebug("Token validated successfully for user: {UserId}", userId);
            
            return userId;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning("Token expired: {Message}", ex.Message);
            return null;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning("Token signature invalid: {Message}", ex.Message);
            return null;
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.LogWarning("Token validation failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return null;
        }
    }
}

// Extension method for easier registration
public static class SupabaseAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseSupabaseAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SupabaseAuthMiddleware>();
    }
}