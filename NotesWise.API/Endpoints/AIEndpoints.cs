using NotesWise.API.Extensions;
using NotesWise.API.Models;
using NotesWise.API.Services;

namespace NotesWise.API.Endpoints;

public static class AIEndpoints
{
    public static void MapAIEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ai").WithTags("AI");

        group.MapPost("generate-summary", GenerateSummary)
            .WithName("GenerateSummary")
            .WithOpenApi();

        group.MapPost("generate-flashcards", GenerateFlashcards)
            .WithName("GenerateFlashcards")
            .WithOpenApi();

        group.MapPost("generate-audio", GenerateAudio)
            .WithName("GenerateAudio")
            .WithOpenApi();
    }

    private static async Task<IResult> GenerateSummary(
        HttpContext context,
        GenerateSummaryRequest request,
        IAIService aiService)
    {
        try
        {
            // Validate user is authenticated
            context.GetUserIdOrThrow();
            
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest("Content is required");
            }

            var summary = await aiService.GenerateSummaryAsync(request.Content);
            
            return Results.Ok(new GenerateSummaryResponse { Summary = summary });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Failed to generate summary",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> GenerateFlashcards(
        HttpContext context,
        GenerateFlashcardsRequest request,
        IAIService aiService)
    {
        try
        {
            // Validate user is authenticated
            //context.GetUserIdOrThrow();
            
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest("Content is required");
            }

            var flashcards = await aiService.GenerateFlashcardsAsync(request.Content);
            
            return Results.Ok(new GenerateFlashcardsResponse { Flashcards = flashcards });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Failed to generate flashcards",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> GenerateAudio(
        HttpContext context,
        GenerateAudioRequest request,
        IAIService aiService)
    {
        try
        {
            // Validate user is authenticated
            //context.GetUserIdOrThrow();
            
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return Results.BadRequest("Text is required");
            }

            var audioContent = await aiService.GenerateAudioAsync(request.Text, request.Voice);
            
            return Results.Ok(new GenerateAudioResponse { AudioContent = audioContent });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Failed to generate audio",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}