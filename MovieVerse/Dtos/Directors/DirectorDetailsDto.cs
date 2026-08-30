using MovieVerse.Dtos.People;

namespace MovieVerse.Dtos.Directors;

public class DirectorDetailsDto : DirectorReturnDto
{
    public List<FilmographyItemDto> Filmography { get; set; } = [];
}
