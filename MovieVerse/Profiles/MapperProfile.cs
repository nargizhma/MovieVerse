using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Auth;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Genres;
using MovieVerse.Dtos.Movies;
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
    }
}