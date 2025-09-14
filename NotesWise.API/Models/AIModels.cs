using System.Text.Json.Serialization;

namespace NotesWise.API.Models;

// Request/Response models for AI services
public class GenerateSummaryRequest
{
    public required string Content { get; set; }
}

public class GenerateSummaryResponse
{
    public required string Summary { get; set; }
}

public class GenerateFlashcardsRequest
{
    public required string Content { get; set; }
}

public class GenerateFlashcardsResponse
{
    public required List<FlashcardData> Flashcards { get; set; }
}

public class FlashcardData
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

public class GenerateAudioRequest
{
    public required string Text { get; set; }
    public string Voice { get; set; } = "alloy";
}

public class GenerateAudioResponse
{
    public required string AudioContent { get; set; } // Base64 encoded audio
}

public class GenerateFlashcardAudioRequest
{
    public string Voice { get; set; } = "alloy";
    public string Type { get; set; } = "question"; // "question", "answer", or "both"
}

public class GenerateFlashcardAudioResponse
{
    public string? QuestionAudioContent { get; set; } // Base64 encoded audio for question
    public string? AnswerAudioContent { get; set; } // Base64 encoded audio for answer
}

// OpenAI API models
public class OpenAIRequest
{
    public required string Model { get; set; }
    public required List<OpenAIMessage> Input { get; set; }
    public int? MaxOutputTokens { get; set; } = null;
}

public class OpenAIMessage
{
    public required string Role { get; set; }
    public required List<OpenAIContent> Content { get; set; }
}

public class OpenAIContent
{
    public required string Type { get; set; }
    public required string Text { get; set; }
}


 public class OpenAIResponse
{
    public List<OpenAiOutput>? Output { get; set; }
}

public class OpenAiOutput
{
    public string Type { get; set; } = "";
    public List<OpenAiContent>? Content { get; set; }
}

public class OpenAiContent
{
    public string? Text { get; set; }
}

// ElevenLabs API models
public class ElevenLabsRequest
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }
    [JsonPropertyName("model_id")]
    public string Model_Id { get; set; } = "eleven_monolingual_v1";
    [JsonPropertyName("voice_settings")]
    public ElevenLabsVoiceSettings Voice_Settings { get; set; } = new();
}

public class ElevenLabsVoiceSettings
{
    [JsonPropertyName("stability")]
    public double Stability { get; set; } = 0.5;
    [JsonPropertyName("similarity_boost")]
    public double Similarity_Boost { get; set; } = 0.5;
}