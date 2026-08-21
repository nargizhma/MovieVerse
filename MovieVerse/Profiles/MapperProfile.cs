using AutoMapper;
using MovieVerse.Dtos.Auth;
using MovieVerse.Dtos.Genres;
using MovieVerse.Models;

namespace MovieVerse.Profiles
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDto, AppUser>();

            CreateMap<RegisterDto, UserProfile>()
                .ForMember(
                    dest => dest.AppUserId,
                    opt => opt.Ignore()
                );
            CreateMap<GenreCreateDto, Genre>();
            CreateMap<GenreUpdateDto, Genre>();
            CreateMap<Genre, GenreReturnDto>();
        }
    }
}
