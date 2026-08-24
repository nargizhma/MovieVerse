using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Auth;
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
    }
}