using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Auth;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Dtos.Genres;
using MovieVerse.Dtos.Movies;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Dtos.Seasons;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Dtos.Writers;
using MovieVerse.Models;
using MovieVerse.Profiles.Resolvers;

namespace MovieVerse.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // AUTH
        CreateMap<RegisterDto, AppUser>();

        CreateMap<RegisterDto, UserProfile>()
            .ForMember(
                dest => dest.AppUserId,
                opt => opt.Ignore()
            );


        // GENRE
        CreateMap<GenreCreateDto, Genre>();

        CreateMap<GenreUpdateDto, Genre>();

        CreateMap<Genre, GenreReturnDto>();


        // ACTOR CREATE
        CreateMap<ActorCreateDto, Actor>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ActorDetail,
                opt => opt.MapFrom(src => src));

        CreateMap<ActorCreateDto, ActorDetail>()
             .ForMember(
                 dest => dest.ActorId,
                 opt => opt.Ignore())
             .ForMember(
                 dest => dest.Actor,
                 opt => opt.Ignore());


        // ACTOR UPDATE
        CreateMap<ActorUpdateDto, Actor>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ActorDetail,
                opt => opt.Ignore());

        CreateMap<ActorUpdateDto, ActorDetail>()
            .ForMember(
                dest => dest.ActorId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Actor,
                opt => opt.Ignore());


        // ACTOR RETURN
        CreateMap<Actor, ActorReturnDto>()
            .ForMember(
                dest => dest.DeathDate,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.DeathDate
                        : null))
            .ForMember(
                dest => dest.DeathPlace,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.DeathPlace
                        : null))
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.MapFrom<ActorImageUrlResolver>())
            .ForMember(
                dest => dest.Biography,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Biography
                        : null))
            .ForMember(
                dest => dest.BirthDate,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.BirthDate
                        : null))
            .ForMember(
                dest => dest.BirthPlace,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.BirthPlace
                        : null))
            .ForMember(
                dest => dest.HeightInMeters,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.HeightInMeters
                        : null))
            .ForMember(
                dest => dest.AlternativeName,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.AlternativeName
                        : null))
            .ForMember(
                dest => dest.Nickname,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Nickname
                        : null))
            .ForMember(
                dest => dest.Spouse,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Spouse
                        : null))
            .ForMember(
                dest => dest.Children,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Children
                        : null))
            .ForMember(
                dest => dest.Parents,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Parents
                        : null))
            .ForMember(
                dest => dest.Relatives,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Relatives
                        : null))
            .ForMember(
                dest => dest.OtherWorks,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.OtherWorks
                        : null))
            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Trivia
                        : null))
            .ForMember(
                dest => dest.Quote,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Quote
                        : null))
            .ForMember(
                dest => dest.Trademark,
                opt => opt.MapFrom(src =>
                    src.ActorDetail != null
                        ? src.ActorDetail.Trademark
                        : null));

        // DIRECTOR CREATE
        CreateMap<DirectorCreateDto, Director>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.DirectorDetail,
                opt => opt.MapFrom(src => src));

        CreateMap<DirectorCreateDto, DirectorDetail>()
            .ForMember(
                dest => dest.DirectorId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Director,
                opt => opt.Ignore());


        // DIRECTOR UPDATE
        CreateMap<DirectorUpdateDto, Director>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.DirectorDetail,
                opt => opt.Ignore());

        CreateMap<DirectorUpdateDto, DirectorDetail>()
            .ForMember(
                dest => dest.DirectorId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Director,
                opt => opt.Ignore());


        // DIRECTOR RETURN
        CreateMap<Director, DirectorReturnDto>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.MapFrom<DirectorImageUrlResolver>())
            .ForMember(
                dest => dest.Biography,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Biography
                        : null))
            .ForMember(
                dest => dest.BirthDate,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.BirthDate
                        : null))
            .ForMember(
                dest => dest.BirthPlace,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.BirthPlace
                        : null))
            .ForMember(
                dest => dest.DeathDate,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.DeathDate
                        : null))
            .ForMember(
                dest => dest.DeathPlace,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.DeathPlace
                        : null))
            .ForMember(
                dest => dest.HeightInMeters,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.HeightInMeters
                        : null))
            .ForMember(
                dest => dest.AlternativeName,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.AlternativeName
                        : null))
            .ForMember(
                dest => dest.Nickname,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Nickname
                        : null))
            .ForMember(
                dest => dest.Spouse,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Spouse
                        : null))
            .ForMember(
                dest => dest.Children,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Children
                        : null))
            .ForMember(
                dest => dest.Parents,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Parents
                        : null))
            .ForMember(
                dest => dest.Relatives,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Relatives
                        : null))
            .ForMember(
                dest => dest.OtherWorks,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.OtherWorks
                        : null))
            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Trivia
                        : null))
            .ForMember(
                dest => dest.Quote,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Quote
                        : null))
            .ForMember(
                dest => dest.Trademark,
                opt => opt.MapFrom(src =>
                    src.DirectorDetail != null
                        ? src.DirectorDetail.Trademark
                        : null));

        // WRITER CREATE
        CreateMap<WriterCreateDto, Writer>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.WriterDetail,
                opt => opt.MapFrom(src => src));

        CreateMap<WriterCreateDto, WriterDetail>()
            .ForMember(
                dest => dest.WriterId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Writer,
                opt => opt.Ignore());


        // WRITER UPDATE
        CreateMap<WriterUpdateDto, Writer>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.WriterDetail,
                opt => opt.Ignore());

        CreateMap<WriterUpdateDto, WriterDetail>()
            .ForMember(
                dest => dest.WriterId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Writer,
                opt => opt.Ignore());


        // WRITER RETURN
        CreateMap<Writer, WriterReturnDto>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.MapFrom<WriterImageUrlResolver>())
            .ForMember(
                dest => dest.Biography,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Biography
                        : null))
            .ForMember(
                dest => dest.BirthDate,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.BirthDate
                        : null))
            .ForMember(
                dest => dest.BirthPlace,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.BirthPlace
                        : null))
            .ForMember(
                dest => dest.DeathDate,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.DeathDate
                        : null))
            .ForMember(
                dest => dest.DeathPlace,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.DeathPlace
                        : null))
            .ForMember(
                dest => dest.HeightInMeters,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.HeightInMeters
                        : null))
            .ForMember(
                dest => dest.AlternativeName,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.AlternativeName
                        : null))
            .ForMember(
                dest => dest.Nickname,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Nickname
                        : null))
            .ForMember(
                dest => dest.Spouse,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Spouse
                        : null))
            .ForMember(
                dest => dest.Children,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Children
                        : null))
            .ForMember(
                dest => dest.Parents,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Parents
                        : null))
            .ForMember(
                dest => dest.Relatives,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Relatives
                        : null))
            .ForMember(
                dest => dest.OtherWorks,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.OtherWorks
                        : null))
            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Trivia
                        : null))
            .ForMember(
                dest => dest.Quote,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Quote
                        : null))
            .ForMember(
                dest => dest.Trademark,
                opt => opt.MapFrom(src =>
                    src.WriterDetail != null
                        ? src.WriterDetail.Trademark
                        : null));
        // movie create
        CreateMap<MovieCreateDto, Movie>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieDetail,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieGenres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieDirectors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieWriters,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore());

        CreateMap<MovieCreateDto, MovieDetail>()
            .ForMember(
                dest => dest.MovieId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Movie,
                opt => opt.Ignore());

        // movie update
        CreateMap<MovieUpdateDto, Movie>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieDetail,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieGenres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieDirectors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.MovieWriters,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore());

        CreateMap<MovieUpdateDto, MovieDetail>()
            .ForMember(
                dest => dest.MovieId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Movie,
                opt => opt.Ignore());

        // simple return
        CreateMap<Movie, MovieReturnDto>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.MapFrom<
                    MoviePosterUrlResolver<MovieReturnDto>>())
            .ForMember(
                dest => dest.Genres,
                opt => opt.MapFrom(src =>
                    src.MovieGenres.Select(x => x.Genre)))
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews.Average(x => x.Rating)
                        : (decimal?)null))
            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));

        // detailed return
        CreateMap<Movie, MovieDetailsDto>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.MapFrom<
                    MoviePosterUrlResolver<MovieDetailsDto>>())
            .ForMember(
                dest => dest.Storyline,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.Storyline
                        : null))
            .ForMember(
                dest => dest.Tagline,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.Tagline
                        : null))
            .ForMember(
                dest => dest.OriginalLanguage,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.OriginalLanguage
                        : null))
            .ForMember(
                dest => dest.CountryOfOrigin,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.CountryOfOrigin
                        : null))
            .ForMember(
                dest => dest.FilmingLocation,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.FilmingLocation
                        : null))
            .ForMember(
                dest => dest.ProductionCompany,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.ProductionCompany
                        : null))
            .ForMember(
                dest => dest.Budget,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.Budget
                        : null))
            .ForMember(
                dest => dest.GrossWorldwide,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.GrossWorldwide
                        : null))
            .ForMember(
                dest => dest.Color,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.Color
                        : null))
            .ForMember(
                dest => dest.SoundMix,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.SoundMix
                        : null))
            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src =>
                    src.MovieDetail != null
                        ? src.MovieDetail.Trivia
                        : null))
            .ForMember(
                dest => dest.Genres,
                opt => opt.MapFrom(src =>
                    src.MovieGenres.Select(x => x.Genre)))
            .ForMember(
                dest => dest.Cast,
                opt => opt.MapFrom(src =>
                    src.MovieActors
                        .OrderBy(x => x.CastOrder)))
            .ForMember(
                dest => dest.Directors,
                opt => opt.MapFrom(src =>
                    src.MovieDirectors))
            .ForMember(
                dest => dest.Writers,
                opt => opt.MapFrom(src =>
                    src.MovieWriters))
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews.Average(x => x.Rating)
                        : (decimal?)null))
            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));

        // relationship mapping
        CreateMap<MovieActor, MovieCastReturnDto>()
            .ForMember(
                dest => dest.ActorId,
                opt => opt.MapFrom(src =>
                    src.ActorId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Actor.FullName));

        CreateMap<MovieDirector, MovieCrewReturnDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src =>
                    src.DirectorId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Director.FullName));

        CreateMap<MovieWriter, MovieCrewReturnDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src =>
                    src.WriterId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Writer.FullName));
        // TV SHOW CREATE
        CreateMap<TVShowCreateDto, TVShow>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowDetail,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Seasons,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowGenres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore());

        CreateMap<TVShowCreateDto, TVShowDetail>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore());


        // TV SHOW UPDATE
        CreateMap<TVShowUpdateDto, TVShow>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowDetail,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Seasons,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowGenres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShowActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore());

        CreateMap<TVShowUpdateDto, TVShowDetail>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore());


        // TV SHOW RETURN
        CreateMap<TVShow, TVShowReturnDto>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.MapFrom<
                    TVShowPosterUrlResolver<
                        TVShowReturnDto>>())
            .ForMember(
                dest => dest.Genres,
                opt => opt.MapFrom(src =>
                    src.TVShowGenres
                        .Select(x => x.Genre)))
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews
                            .Average(x => x.Rating)
                        : (decimal?)null))
            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));


        CreateMap<TVShow, TVShowDetailsDto>()
            .ForMember(
                dest => dest.PosterUrl,
                opt => opt.MapFrom<
                    TVShowPosterUrlResolver<
                        TVShowDetailsDto>>())

            .ForMember(
                dest => dest.Storyline,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.Storyline
                        : null))

            .ForMember(
                dest => dest.OriginalLanguage,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.OriginalLanguage
                        : null))

            .ForMember(
                dest => dest.CountryOfOrigin,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.CountryOfOrigin
                        : null))

            .ForMember(
                dest => dest.ProductionCompany,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.ProductionCompany
                        : null))

            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.Trivia
                        : null))

            .ForMember(
                dest => dest.Color,
                opt => opt.MapFrom(src =>
                    src.TVShowDetail != null
                        ? src.TVShowDetail.Color
                        : null))

            .ForMember(
                dest => dest.Genres,
                opt => opt.MapFrom(src =>
                    src.TVShowGenres
                        .Select(x => x.Genre)))

            .ForMember(
                dest => dest.Cast,
                opt => opt.MapFrom(src =>
                    src.TVShowActors
                        .OrderBy(x =>
                            x.CastOrder)))

            .ForMember(
                dest => dest.Seasons,
                opt => opt.MapFrom(src =>
                    src.Seasons
                        .OrderBy(x =>
                            x.SeasonNumber)))

            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews
                            .Average(x => x.Rating)
                        : (decimal?)null))

            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));


        CreateMap<
            TVShowActor,
            TVShowCastReturnDto>()
            .ForMember(
                dest => dest.ActorId,
                opt => opt.MapFrom(src =>
                    src.ActorId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Actor.FullName));


        // SEASON
        CreateMap<SeasonCreateDto, Season>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Episodes,
                opt => opt.Ignore());

        CreateMap<SeasonUpdateDto, Season>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Episodes,
                opt => opt.Ignore());

        CreateMap<Season, SeasonReturnDto>()
            .ForMember(
                dest => dest.EpisodeCount,
                opt => opt.MapFrom(src =>
                    src.Episodes.Count));


        // EPISODE CREATE
        CreateMap<EpisodeCreateDto, Episode>()
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.SeasonId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Season,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeDirectors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeWriters,
                opt => opt.Ignore());


        // EPISODE UPDATE
        CreateMap<EpisodeUpdateDto, Episode>()
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.SeasonId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Season,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Reviews,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeActors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeDirectors,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.EpisodeWriters,
                opt => opt.Ignore());


        // EPISODE RETURN
        CreateMap<Episode, EpisodeReturnDto>()
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom<
                    EpisodeImageUrlResolver<
                        EpisodeReturnDto>>())
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews
                            .Average(x => x.Rating)
                        : (decimal?)null))
            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));


        CreateMap<Episode, EpisodeDetailsDto>()
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom<
                    EpisodeImageUrlResolver<
                        EpisodeDetailsDto>>())

            .ForMember(
                dest => dest.SeasonNumber,
                opt => opt.MapFrom(src =>
                    src.Season.SeasonNumber))

            .ForMember(
                dest => dest.TVShowId,
                opt => opt.MapFrom(src =>
                    src.Season.TVShowId))

            .ForMember(
                dest => dest.TVShowTitle,
                opt => opt.MapFrom(src =>
                    src.Season.TVShow.Title))

            .ForMember(
                dest => dest.Cast,
                opt => opt.MapFrom(src =>
                    src.EpisodeActors
                        .OrderBy(x =>
                            x.CastOrder)))

            .ForMember(
                dest => dest.Directors,
                opt => opt.MapFrom(src =>
                    src.EpisodeDirectors))

            .ForMember(
                dest => dest.Writers,
                opt => opt.MapFrom(src =>
                    src.EpisodeWriters))

            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count != 0
                        ? src.Reviews
                            .Average(x => x.Rating)
                        : (decimal?)null))

            .ForMember(
                dest => dest.ReviewCount,
                opt => opt.MapFrom(src =>
                    src.Reviews.Count));


        CreateMap<
            EpisodeActor,
            EpisodeCastReturnDto>()
            .ForMember(
                dest => dest.ActorId,
                opt => opt.MapFrom(src =>
                    src.ActorId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Actor.FullName));


        CreateMap<
            EpisodeDirector,
            EpisodeCrewReturnDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src =>
                    src.DirectorId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Director.FullName));


        CreateMap<
            EpisodeWriter,
            EpisodeCrewReturnDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src =>
                    src.WriterId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.Writer.FullName));
        // REVIEWS

        CreateMap<ReviewCreateDto, MovieReview>()
            .ForMember(
                dest => dest.MovieId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Movie,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<ReviewUpdateDto, MovieReview>()
            .ForMember(
                dest => dest.MovieId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Movie,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<MovieReview, ReviewReturnDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src =>
                    src.User.UserName))
            .ForMember(
                dest => dest.DisplayName,
                opt => opt.MapFrom(src =>
                    src.User.Profile != null
                        ? src.User.Profile.DisplayName
                        : null));


        CreateMap<ReviewCreateDto, TVShowReview>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<ReviewUpdateDto, TVShowReview>()
            .ForMember(
                dest => dest.TVShowId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TVShow,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<TVShowReview, ReviewReturnDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src =>
                    src.User.UserName))
            .ForMember(
                dest => dest.DisplayName,
                opt => opt.MapFrom(src =>
                    src.User.Profile != null
                        ? src.User.Profile.DisplayName
                        : null));


        CreateMap<ReviewCreateDto, EpisodeReview>()
            .ForMember(
                dest => dest.EpisodeId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Episode,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<ReviewUpdateDto, EpisodeReview>()
            .ForMember(
                dest => dest.EpisodeId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Episode,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());

        CreateMap<EpisodeReview, ReviewReturnDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src =>
                    src.User.UserName))
            .ForMember(
                dest => dest.DisplayName,
                opt => opt.MapFrom(src =>
                    src.User.Profile != null
                        ? src.User.Profile.DisplayName
                        : null));
        CreateMap<Actor, ActorDetailsDto>()
            .IncludeBase<Actor, ActorReturnDto>()
            .ForMember(
                dest => dest.Filmography,
                opt => opt.Ignore());

        CreateMap<Director, DirectorDetailsDto>()
            .IncludeBase<Director, DirectorReturnDto>()
            .ForMember(
                dest => dest.Filmography,
                opt => opt.Ignore());
    }
}