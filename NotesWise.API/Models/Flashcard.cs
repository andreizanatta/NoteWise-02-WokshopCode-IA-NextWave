using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotesWise.API.Models;

public class Flashcard
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("noteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string NoteId { get; set; }
    
    [BsonElement("question")]
    public required string Question { get; set; }
    
    [BsonElement("answer")]
    public required string Answer { get; set; }
    
    [BsonElement("questionAudioUrl")]
    public string? QuestionAudioUrl { get; set; }
    
    [BsonElement("answerAudioUrl")]
    public string? AnswerAudioUrl { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateFlashcardRequest
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

public class CreateFlashcardsRequest
{
    public required List<CreateFlashcardRequest> Flashcards { get; set; }
}