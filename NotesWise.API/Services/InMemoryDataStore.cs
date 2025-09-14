using System.Collections.Concurrent;
using NotesWise.API.Models;

namespace NotesWise.API.Services;

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<string, Category> _categories = new();
    private readonly ConcurrentDictionary<string, Note> _notes = new();
    private readonly ConcurrentDictionary<string, Flashcard> _flashcards = new();

    #region Categories
    
    public Task<IEnumerable<Category>> GetCategoriesAsync(string userId)
    {
        var categories = _categories.Values
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .AsEnumerable();
        
        return Task.FromResult(categories);
    }

    public Task<Category?> GetCategoryByIdAsync(string id, string userId)
    {
        if (_categories.TryGetValue(id, out var category) && category.UserId == userId)
        {
            return Task.FromResult<Category?>(category);
        }
        
        return Task.FromResult<Category?>(null);
    }

    public Task<Category> CreateCategoryAsync(Category category)
    {
        _categories[category.Id] = category;
        return Task.FromResult(category);
    }

    public Task<Category?> UpdateCategoryAsync(Category category)
    {
        if (_categories.TryGetValue(category.Id, out var existing) && existing.UserId == category.UserId)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _categories[category.Id] = category;
            return Task.FromResult<Category?>(category);
        }
        
        return Task.FromResult<Category?>(null);
    }

    public Task<bool> DeleteCategoryAsync(string id, string userId)
    {
        if (_categories.TryGetValue(id, out var category) && category.UserId == userId)
        {
            _categories.TryRemove(id, out _);
            
            // Update notes that reference this category
            var notesToUpdate = _notes.Values.Where(n => n.CategoryId == id && n.UserId == userId);
            foreach (var note in notesToUpdate)
            {
                note.CategoryId = null;
                note.UpdatedAt = DateTime.UtcNow;
            }
            
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    #endregion

    #region Notes

    public Task<IEnumerable<Note>> GetNotesAsync(string userId, string? categoryId = null)
    {
        var notes = _notes.Values
            .Where(n => n.UserId == userId);

        if (!string.IsNullOrEmpty(categoryId))
        {
            notes = notes.Where(n => n.CategoryId == categoryId);
        }

        var result = notes.OrderByDescending(n => n.UpdatedAt).AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<Note?> GetNoteByIdAsync(string id, string userId)
    {
        if (_notes.TryGetValue(id, out var note) && note.UserId == userId)
        {
            return Task.FromResult<Note?>(note);
        }
        
        return Task.FromResult<Note?>(null);
    }

    public Task<Note> CreateNoteAsync(Note note)
    {
        _notes[note.Id] = note;
        return Task.FromResult(note);
    }

    public Task<Note?> UpdateNoteAsync(Note note)
    {
        if (_notes.TryGetValue(note.Id, out var existing) && existing.UserId == note.UserId)
        {
            note.UpdatedAt = DateTime.UtcNow;
            _notes[note.Id] = note;
            return Task.FromResult<Note?>(note);
        }
        
        return Task.FromResult<Note?>(null);
    }

    public Task<bool> DeleteNoteAsync(string id, string userId)
    {
        if (_notes.TryGetValue(id, out var note) && note.UserId == userId)
        {
            _notes.TryRemove(id, out _);
            
            // Delete associated flashcards
            var flashcardsToDelete = _flashcards.Values.Where(f => f.NoteId == id).ToList();
            foreach (var flashcard in flashcardsToDelete)
            {
                _flashcards.TryRemove(flashcard.Id, out _);
            }
            
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    #endregion

    #region Flashcards

    public Task<IEnumerable<Flashcard>> GetFlashcardsAsync(string userId)
    {
        // Get flashcards for notes owned by the user
        var userNoteIds = _notes.Values
            .Where(n => n.UserId == userId)
            .Select(n => n.Id)
            .ToHashSet();

        var flashcards = _flashcards.Values
            .Where(f => userNoteIds.Contains(f.NoteId))
            .OrderByDescending(f => f.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(flashcards);
    }

    public Task<IEnumerable<Flashcard>> GetFlashcardsByNoteIdAsync(string noteId)
    {
        var flashcards = _flashcards.Values
            .Where(f => f.NoteId == noteId)
            .OrderByDescending(f => f.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(flashcards);
    }

    public Task<Flashcard> CreateFlashcardAsync(Flashcard flashcard)
    {
        _flashcards[flashcard.Id] = flashcard;
        return Task.FromResult(flashcard);
    }

    public Task<bool> DeleteFlashcardAsync(string id)
    {
        return Task.FromResult(_flashcards.TryRemove(id, out _));
    }

    public Task<Flashcard?> GetFlashcardByIdAsync(string id)
    {
        if (_flashcards.TryGetValue(id, out var flashcard))
        {
            return Task.FromResult<Flashcard?>(flashcard);
        }
        
        return Task.FromResult<Flashcard?>(null);
    }

    public Task<Flashcard?> UpdateFlashcardAsync(Flashcard flashcard)
    {
        if (_flashcards.ContainsKey(flashcard.Id))
        {
            _flashcards[flashcard.Id] = flashcard;
            return Task.FromResult<Flashcard?>(flashcard);
        }
        
        return Task.FromResult<Flashcard?>(null);
    }

    #endregion
}