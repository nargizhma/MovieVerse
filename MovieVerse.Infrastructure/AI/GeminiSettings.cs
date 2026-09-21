namespace MovieVerse.Infrastructure.AI;

public class GeminiSettings
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; }
        = string.Empty;

    public string Model { get; set; }
        = "gemini-3.8-flash";
}