using MovieVerse.Dtos.People;

namespace MovieVerse.Dtos.Writers;

public class WriterDetailsDto : WriterReturnDto
{
    public List<FilmographyItemDto> Filmography { get; set; } = [];
}