using NotesWise.API.Extensions;
using NotesWise.API.Models;
using NotesWise.API.Services;

namespace NotesWise.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("", GetCategories)
            .WithName("GetCategories")
            .WithOpenApi();

        group.MapPost("", CreateCategory)
            .WithName("CreateCategory")
            .WithOpenApi();

        group.MapPut("{id}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithOpenApi();

        group.MapDelete("{id}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithOpenApi();
    }

    private static async Task<IResult> GetCategories(HttpContext context, IDataStore dataStore)
    {
        try
        {
            var userId = context.GetUserIdOrThrow();
            var categories = await dataStore.GetCategoriesAsync(userId);
            return Results.Ok(categories);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> CreateCategory(
        HttpContext context, 
        CreateCategoryRequest request, 
        IDataStore dataStore)
    {
        try
        {
            var userId = context.GetUserIdOrThrow();
            
            var category = new Category
            {
                Name = request.Name,
                Color = request.Color,
                UserId = userId
            };

            var createdCategory = await dataStore.CreateCategoryAsync(category);
            return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> UpdateCategory(
        HttpContext context, 
        string id, 
        UpdateCategoryRequest request, 
        IDataStore dataStore)
    {
        try
        {
            var userId = context.GetUserIdOrThrow();
            
            var existingCategory = await dataStore.GetCategoryByIdAsync(id, userId);
            if (existingCategory == null)
            {
                return Results.NotFound();
            }

            // Update properties
            if (!string.IsNullOrEmpty(request.Name))
                existingCategory.Name = request.Name;
            if (request.Color != null)
                existingCategory.Color = request.Color;

            var updatedCategory = await dataStore.UpdateCategoryAsync(existingCategory);
            return updatedCategory != null ? Results.Ok(updatedCategory) : Results.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> DeleteCategory(
        HttpContext context, 
        string id, 
        IDataStore dataStore)
    {
        try
        {
            var userId = context.GetUserIdOrThrow();
            
            var success = await dataStore.DeleteCategoryAsync(id, userId);
            return success ? Results.NoContent() : Results.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }
}