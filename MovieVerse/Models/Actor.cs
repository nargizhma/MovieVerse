using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Actor : BaseEntity
{
    // REQUIRED
    public string FullName { get; set; } = null!;

    // OPTIONAL — an actor may not have an uploaded image
    public string? ProfileImageUrl { get; set; }

    // OPTIONAL one-to-one
    // You may create the Actor before filling all profile details
    public ActorDetail? ActorDetail { get; set; }

    // REQUIRED collection, but can be empty
    public List<MovieActor> MovieActors { get; set; } = [];
}