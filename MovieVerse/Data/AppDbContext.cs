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


        // =========================
        // ONE-TO-ONE DETAILS
        // =========================

        modelBuilder.Entity<MovieDetail>()
            .HasOne(x => x.Movie)
            .WithOne(x => x.MovieDetail)
            .HasForeignKey<MovieDetail>(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActorDetail>()
            .HasOne(x => x.Actor)
            .WithOne(x => x.ActorDetail)
            .HasForeignKey<ActorDetail>(x => x.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DirectorDetail>()
            .HasOne(x => x.Director)
            .WithOne(x => x.DirectorDetail)
            .HasForeignKey<DirectorDetail>(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WriterDetail>()
            .HasOne(x => x.Writer)
            .WithOne(x => x.WriterDetail)
            .HasForeignKey<WriterDetail>(x => x.WriterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TVShowDetail>()
            .HasOne(x => x.TVShow)
            .WithOne(x => x.TVShowDetail)
            .HasForeignKey<TVShowDetail>(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);


        // =========================
        // MOVIE RELATIONSHIPS
        // =========================

        modelBuilder.Entity<MovieActor>()
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasOne(x => x.Actor)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<MovieDirector>()
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieDirectors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieDirector>()
            .HasOne(x => x.Director)
            .WithMany(x => x.MovieDirectors)
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<MovieWriter>()
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieWriters)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieWriter>()
            .HasOne(x => x.Writer)
            .WithMany(x => x.MovieWriters)
            .HasForeignKey(x => x.WriterId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<MovieGenre>()
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieGenres)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.MovieGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);


        // Prevent duplicates like:
        // Interstellar -> Christopher Nolan
        // Interstellar -> Christopher Nolan again

        modelBuilder.Entity<MovieDirector>()
            .HasIndex(x => new { x.MovieId, x.DirectorId })
            .IsUnique();

        modelBuilder.Entity<MovieWriter>()
            .HasIndex(x => new { x.MovieId, x.WriterId })
            .IsUnique();

        modelBuilder.Entity<MovieActor>()
            .HasIndex(x => new { x.MovieId, x.ActorId })
            .IsUnique();

        modelBuilder.Entity<MovieGenre>()
            .HasIndex(x => new { x.MovieId, x.GenreId })
            .IsUnique();


        // =========================
        // MOVIE REVIEWS
        // =========================

        modelBuilder.Entity<Review>()
            .HasOne(x => x.Movie)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(x => x.User)
            .WithMany(x => x.MovieReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TVShowReview>()
            .HasOne(x => x.User)
            .WithMany(x => x.TVShowReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EpisodeReview>()
            .HasOne(x => x.User)
            .WithMany(x => x.EpisodeReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One review/rating per user per movie
        modelBuilder.Entity<Review>()
            .HasIndex(x => new { x.MovieId, x.UserId })
            .IsUnique();


        // =========================
        // TV SHOW -> SEASON
        // =========================

        modelBuilder.Entity<Season>()
            .HasOne(x => x.TVShow)
            .WithMany(x => x.Seasons)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        // A show cannot have two Season 1s
        modelBuilder.Entity<Season>()
            .HasIndex(x => new { x.TVShowId, x.SeasonNumber })
            .IsUnique();


        // =========================
        // SEASON -> EPISODE
        // =========================

        modelBuilder.Entity<Episode>()
            .HasOne(x => x.Season)
            .WithMany(x => x.Episodes)
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        // A season cannot have two Episode 3s
        modelBuilder.Entity<Episode>()
            .HasIndex(x => new { x.SeasonId, x.EpisodeNumber })
            .IsUnique();


        // =========================
        // TV SHOW ACTORS
        // =========================

        modelBuilder.Entity<TVShowActor>()
            .HasOne(x => x.TVShow)
            .WithMany(x => x.TVShowActors)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TVShowActor>()
            .HasOne(x => x.Actor)
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TVShowActor>()
            .HasIndex(x => new { x.TVShowId, x.ActorId })
            .IsUnique();


        // =========================
        // TV SHOW GENRES
        // =========================

        modelBuilder.Entity<TVShowGenre>()
            .HasOne(x => x.TVShow)
            .WithMany(x => x.TVShowGenres)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TVShowGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.TVShowGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TVShowGenre>()
            .HasIndex(x => new { x.TVShowId, x.GenreId })
            .IsUnique();


        // =========================
        // TV SHOW REVIEWS
        // =========================

        modelBuilder.Entity<TVShowReview>()
            .HasOne(x => x.TVShow)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<TVShowReview>()
            .HasIndex(x => new { x.TVShowId, x.UserId })
            .IsUnique();


        // =========================
        // EPISODE ACTORS
        // =========================

        modelBuilder.Entity<EpisodeActor>()
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeActors)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EpisodeActor>()
            .HasOne(x => x.Actor)
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EpisodeActor>()
            .HasIndex(x => new { x.EpisodeId, x.ActorId })
            .IsUnique();


        // =========================
        // EPISODE DIRECTORS
        // =========================

        modelBuilder.Entity<EpisodeDirector>()
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeDirectors)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EpisodeDirector>()
            .HasOne(x => x.Director)
            .WithMany()
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EpisodeDirector>()
            .HasIndex(x => new { x.EpisodeId, x.DirectorId })
            .IsUnique();


        // =========================
        // EPISODE WRITERS
        // =========================

        modelBuilder.Entity<EpisodeWriter>()
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeWriters)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EpisodeWriter>()
            .HasOne(x => x.Writer)
            .WithMany()
            .HasForeignKey(x => x.WriterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EpisodeWriter>()
            .HasIndex(x => new { x.EpisodeId, x.WriterId })
            .IsUnique();


        // =========================
        // EPISODE REVIEWS
        // =========================

        modelBuilder.Entity<EpisodeReview>()
            .HasOne(x => x.Episode)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EpisodeReview>()
            .HasIndex(x => new { x.EpisodeId, x.UserId })
            .IsUnique();
        // decimal props
        modelBuilder.Entity<ActorDetail>()
    .Property(x => x.HeightInMeters)
    .HasPrecision(3, 2);

        modelBuilder.Entity<DirectorDetail>()
            .Property(x => x.HeightInMeters)
            .HasPrecision(3, 2);

        modelBuilder.Entity<WriterDetail>()
            .Property(x => x.HeightInMeters)
            .HasPrecision(3, 2);

        modelBuilder.Entity<MovieDetail>()
            .Property(x => x.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MovieDetail>()
            .Property(x => x.GrossWorldwide)
            .HasPrecision(18, 2);

    }
}