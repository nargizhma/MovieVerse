using MovieVerse.Models.Common;

namespace MovieVerse.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Models;


public class AppDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    // Users
    public DbSet<UserProfile> UserProfiles { get; set; }

    // User activity
    public DbSet<WatchlistItem> WatchlistItems { get; set; }
    public DbSet<WatchHistoryItem> WatchHistoryItems { get; set; }
    // Movies
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieDetail> MovieDetails { get; set; }

    // People
    public DbSet<Actor> Actors { get; set; }
    public DbSet<ActorDetail> ActorDetails { get; set; }

    public DbSet<Director> Directors { get; set; }
    public DbSet<DirectorDetail> DirectorDetails { get; set; }

    public DbSet<Writer> Writers { get; set; }
    public DbSet<WriterDetail> WriterDetails { get; set; }

    // Genres
    public DbSet<Genre> Genres { get; set; }

    // Movie relationships
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieDirector> MovieDirectors { get; set; }
    public DbSet<MovieWriter> MovieWriters { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }

    // Movie reviews
    public DbSet<Review> Reviews { get; set; }

    // TV Shows
    public DbSet<TVShow> TVShows { get; set; }
    public DbSet<TVShowDetail> TVShowDetails { get; set; }

    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    // TV Show relationships
    public DbSet<TVShowActor> TVShowActors { get; set; }
    public DbSet<TVShowGenre> TVShowGenres { get; set; }

    // TV Show reviews
    public DbSet<TVShowReview> TVShowReviews { get; set; }

    // Episode relationships
    public DbSet<EpisodeActor> EpisodeActors { get; set; }
    public DbSet<EpisodeDirector> EpisodeDirectors { get; set; }
    public DbSet<EpisodeWriter> EpisodeWriters { get; set; }

    // Episode reviews
    public DbSet<EpisodeReview> EpisodeReviews { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // -------------------------
        // GUID generation
        // -------------------------

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Id))
                    .HasDefaultValueSql("NEWSEQUENTIALID()");
            }
        }
    }
}