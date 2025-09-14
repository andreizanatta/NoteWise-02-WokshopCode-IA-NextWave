using MongoDB.Driver;
using NotesWise.API.Models;

namespace NotesWise.API.Services;

public class MongoDataStore : IDataStore
{
    private readonly IMongoCollection<Category> _categories;
    private readonly IMongoCollection<Note> _notes;
    private readonly IMongoCollection<Flashcard> _flashcards;

    public MongoDataStore(IMongoDatabase database, MongoDbSettings settings)
    {
        _categories = database.GetCollection<Category>(settings.CategoriesCollectionName);
        _notes = database.GetCollection<Note>(settings.NotesCollectionName);
        _flashcards = database.GetCollection<Flashcard>(settings.FlashcardsCollectionName);
    }

    #region Categories

    public async Task<IEnumerable<Category>> GetCategoriesAsync(string userId)
    {
        return await _categories
            .Find(c => c.UserId == userId)
            .SortBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(string id, string userId)
    {
        return await _categories
            .Find(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _categories.InsertOneAsync(category);
        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        var result = await _categories.ReplaceOneAsync(
            c => c.Id == category.Id && c.UserId == category.UserId,
            category);

        return result.MatchedCount > 0 ? category : null;
    }

    public async Task<bool> DeleteCategoryAsync(string id, string userId)
    {
        var deleteResult = await _categories.DeleteOneAsync(c => c.Id == id && c.UserId == userId);
        
        if (deleteResult.DeletedCount > 0)
        {
            // Update notes that reference this category
            var updateDefinition = Builders<Note>.Update
                .Set(n => n.CategoryId, null as string)
                .Set(n => n.UpdatedAt, DateTime.UtcNow);
            
            await _notes.UpdateManyAsync(
                n => n.CategoryId == id && n.UserId == userId,
                updateDefinition);
        }

        return deleteResult.DeletedCount > 0;
    }

    #endregion

    #region Notes

    public async Task<IEnumerable<Note>> GetNotesAsync(string userId, string? categoryId = null)
    {
        var filter = Builders<Note>.Filter.Eq(n => n.UserId, userId);
        
        if (!string.IsNullOrEmpty(categoryId))
        {
            filter = filter & Builders<Note>.Filter.Eq(n => n.CategoryId, categoryId);
        }

        return await _notes
            .Find(filter)
            .SortByDescending(n => n.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(string id, string userId)
    {
        return await _notes
            .Find(n => n.Id == id && n.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<Note> CreateNoteAsync(Note note)
    {
        await _notes.InsertOneAsync(note);
        return note;
    }

    public async Task<Note?> UpdateNoteAsync(Note note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        var result = await _notes.ReplaceOneAsync(
            n => n.Id == note.Id && n.UserId == note.UserId,
            note);

        return result.MatchedCount > 0 ? note : null;
    }

    public async Task<bool> DeleteNoteAsync(string id, string userId)
    {
        var deleteResult = await _notes.DeleteOneAsync(n => n.Id == id && n.UserId == userId);
        
        if (deleteResult.DeletedCount > 0)
        {
            // Delete associated flashcards
            await _flashcards.DeleteManyAsync(f => f.NoteId == id);
        }

        return deleteResult.DeletedCount > 0;
    }

    #endregion

    #region Flashcards

    public async Task<IEnumerable<Flashcard>> GetFlashcardsAsync(string userId)
    {
        // Get flashcards for notes owned by the user
        var userNoteIds = await _notes
            .Find(n => n.UserId == userId)
            .Project(n => n.Id)
            .ToListAsync();

        return await _flashcards
            .Find(f => userNoteIds.Contains(f.NoteId))
            .SortByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByNoteIdAsync(string noteId)
    {
        return await _flashcards
            .Find(f => f.NoteId == noteId)
            .SortByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Flashcard> CreateFlashcardAsync(Flashcard flashcard)
    {
        await _flashcards.InsertOneAsync(flashcard);
        return flashcard;
    }

    public async Task<Flashcard?> GetFlashcardByIdAsync(string id)
    {
        return await _flashcards
            .Find(f => f.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Flashcard?> UpdateFlashcardAsync(Flashcard flashcard)
    {
        var result = await _flashcards.ReplaceOneAsync(
            f => f.Id == flashcard.Id,
            flashcard);

        return result.MatchedCount > 0 ? flashcard : null;
    }

    public async Task<bool> DeleteFlashcardAsync(string id)
    {
        var deleteResult = await _flashcards.DeleteOneAsync(f => f.Id == id);
        return deleteResult.DeletedCount > 0;
    }

    #endregion
}