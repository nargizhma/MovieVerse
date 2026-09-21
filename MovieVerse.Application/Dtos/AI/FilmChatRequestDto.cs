namespace MovieVerse.Dtos.AI;

public class FilmChatRequestDto
{
    public string Message { get; set; } = string.Empty;

    public List<FilmChatMessageDto> History { get; set; } = [];
}