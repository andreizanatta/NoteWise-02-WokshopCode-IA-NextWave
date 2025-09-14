using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesWise.API.Endpoints;
using NotesWise.API.Middleware;
using NotesWise.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // More permissive CORS for development
            policy.WithOrigins(
                    "http://localhost:3000",    // Create React App
                    "http://localhost:5173",    // Vite default
                    "http://localhost:8080",    // Alternative Vite port
                    "http://localhost:4173",    // Vite preview
                    "http://[::]:8080",         // IPv6 localhost
                    "http://192.168.1.5:8080"   // Network IP
                  )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetIsOriginAllowed(origin => true); // Allow any origin in development
        }
        else
        {
            // Strict CORS for production
            policy.WithOrigins("https://your-production-domain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddScoped<IMongoClient>(serviceProvider =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDB settings not configured");
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDB settings not configured");
    return client.GetDatabase(settings.DatabaseName);
});

// Register MongoDB data store
builder.Services.AddScoped<IDataStore, MongoDataStore>();

// Register AI service
builder.Services.AddHttpClient<IAIService, AIService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

// Use CORS before HTTPS redirection
app.UseCors("AllowReactApp");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Use Supabase authentication middleware
//app.UseSupabaseAuth();

// Map API endpoints
app.MapCategoryEndpoints();
app.MapNoteEndpoints();
app.MapFlashcardEndpoints();
app.MapAIEndpoints();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();