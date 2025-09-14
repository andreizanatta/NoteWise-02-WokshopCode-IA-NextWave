namespace NotesWise.API.Extensions;

public static class HttpContextExtensions
{
    public static string? GetUserId(this HttpContext context)
    {
        return context.Items["UserId"] as string;
    }

    public static string GetUserIdOrThrow(this HttpContext context)
    {
        var userId = context.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }
}