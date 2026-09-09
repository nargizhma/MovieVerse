using MovieVerse.Dtos.People;

namespace MovieVerse.Dtos.Actors;

public class ActorDetailsDto : ActorReturnDto
{
    public List<FilmographyItemDto> Filmography { get; set; } = [];
}
