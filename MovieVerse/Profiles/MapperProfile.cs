using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Auth;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Genres;
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
                opt => opt.Ignore());


        // ACTOR UPDATE
        CreateMap<ActorUpdateDto, Actor>()
            .ForMember(
                dest => dest.ProfileImageUrl,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ActorDetail,
                opt => opt.Ignore());

        CreateMap<ActorUpdateDto, ActorDetail>();


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
                opt => opt.MapFrom(src => src.DirectorDetail!.Biography))
            .ForMember(
                dest => dest.BirthDate,
                opt => opt.MapFrom(src => src.DirectorDetail!.BirthDate))
            .ForMember(
                dest => dest.BirthPlace,
                opt => opt.MapFrom(src => src.DirectorDetail!.BirthPlace))
            .ForMember(
                dest => dest.DeathDate,
                opt => opt.MapFrom(src => src.DirectorDetail!.DeathDate))
            .ForMember(
                dest => dest.DeathPlace,
                opt => opt.MapFrom(src => src.DirectorDetail!.DeathPlace))
            .ForMember(
                dest => dest.HeightInMeters,
                opt => opt.MapFrom(src => src.DirectorDetail!.HeightInMeters))
            .ForMember(
                dest => dest.AlternativeName,
                opt => opt.MapFrom(src => src.DirectorDetail!.AlternativeName))
            .ForMember(
                dest => dest.Nickname,
                opt => opt.MapFrom(src => src.DirectorDetail!.Nickname))
            .ForMember(
                dest => dest.Spouse,
                opt => opt.MapFrom(src => src.DirectorDetail!.Spouse))
            .ForMember(
                dest => dest.Children,
                opt => opt.MapFrom(src => src.DirectorDetail!.Children))
            .ForMember(
                dest => dest.Parents,
                opt => opt.MapFrom(src => src.DirectorDetail!.Parents))
            .ForMember(
                dest => dest.Relatives,
                opt => opt.MapFrom(src => src.DirectorDetail!.Relatives))
            .ForMember(
                dest => dest.OtherWorks,
                opt => opt.MapFrom(src => src.DirectorDetail!.OtherWorks))
            .ForMember(
                dest => dest.Trivia,
                opt => opt.MapFrom(src => src.DirectorDetail!.Trivia))
            .ForMember(
                dest => dest.Quote,
                opt => opt.MapFrom(src => src.DirectorDetail!.Quote))
            .ForMember(
                dest => dest.Trademark,
                opt => opt.MapFrom(src => src.DirectorDetail!.Trademark));
    }
}