using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Auth;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Genres;
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
    }
}