using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotesWise.API.Models;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("title")]
    public required string Title { get; set; }
    
    [BsonElement("content")]
    public required string Content { get; set; }
    
    [BsonElement("summary")]
    public string? Summary { get; set; }
    
    [BsonElement("audioUrl")]
    public string? AudioUrl { get; set; }
    
    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CategoryId { get; set; }
    
    [BsonElement("userId")]
    public required string UserId { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateNoteRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Summary { get; set; }
    public string? AudioUrl { get; set; }
    public string? CategoryId { get; set; }
}

public class UpdateNoteRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public string? AudioUrl { get; set; }
    public string? CategoryId { get; set; }
}