using NotesWise.API.Models;

namespace NotesWise.API.Services;

public interface IDataStore
{
    // Categories
    Task<IEnumerable<Category>> GetCategoriesAsync(string userId);
    Task<Category?> GetCategoryByIdAsync(string id, string userId);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category?> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(string id, string userId);
    
    // Notes
    Task<IEnumerable<Note>> GetNotesAsync(string userId, string? categoryId = null);
    Task<Note?> GetNoteByIdAsync(string id, string userId);
    Task<Note> CreateNoteAsync(Note note);
    Task<Note?> UpdateNoteAsync(Note note);
    Task<bool> DeleteNoteAsync(string id, string userId);
    
    // Flashcards
    Task<IEnumerable<Flashcard>> GetFlashcardsAsync(string userId);
    Task<IEnumerable<Flashcard>> GetFlashcardsByNoteIdAsync(string noteId);
    Task<Flashcard?> GetFlashcardByIdAsync(string id);
    Task<Flashcard> CreateFlashcardAsync(Flashcard flashcard);
    Task<Flashcard?> UpdateFlashcardAsync(Flashcard flashcard);
    Task<bool> DeleteFlashcardAsync(string id);
}