namespace NotesWise.API.Services;

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public string CategoriesCollectionName { get; set; } = "categories";
    public string NotesCollectionName { get; set; } = "notes";
    public string FlashcardsCollectionName { get; set; } = "flashcards";
}