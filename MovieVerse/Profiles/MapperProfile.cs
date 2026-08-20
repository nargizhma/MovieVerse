using AutoMapper;
using MovieVerse.Dtos.Auth;
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
        }
    }
}
