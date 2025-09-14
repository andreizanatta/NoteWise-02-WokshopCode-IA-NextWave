using NotesWise.API.Models;

namespace NotesWise.API.Services;

public interface IAIService
{
    Task<string> GenerateSummaryAsync(string content);
    Task<List<FlashcardData>> GenerateFlashcardsAsync(string content);
    Task<string> GenerateAudioAsync(string text, string voice = "burt");
    Task<GenerateFlashcardAudioResponse> GenerateFlashcardAudioAsync(Flashcard flashcard, string voice = "burt", string type = "both");
}