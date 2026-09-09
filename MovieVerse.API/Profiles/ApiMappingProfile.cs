using AutoMapper;
using Microsoft.AspNetCore.Http;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Dtos.Movies;
using MovieVerse.Dtos.Profiles;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Dtos.Writers;
using MovieVerse.Requests.Actors;
using MovieVerse.Requests.Directors;
using MovieVerse.Requests.Episodes;
using MovieVerse.Requests.Movies;
using MovieVerse.Requests.Profiles;
using MovieVerse.Requests.TVShows;
using MovieVerse.Requests.Writers;

namespace MovieVerse.Profiles;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<IFormFile, UploadedFile>()
            .ConvertUsing(file =>
                new UploadedFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Length = file.Length,
                    Content = file.OpenReadStream()
                });

        CreateMap<ActorCreateDto, ActorCreateRequest>();
        CreateMap<ActorUpdateDto, ActorUpdateRequest>();

        CreateMap<DirectorCreateDto, DirectorCreateRequest>();
        CreateMap<DirectorUpdateDto, DirectorUpdateRequest>();

        CreateMap<WriterCreateDto, WriterCreateRequest>();
        CreateMap<WriterUpdateDto, WriterUpdateRequest>();

        CreateMap<MovieCreateDto, MovieCreateRequest>();
        CreateMap<MovieUpdateDto, MovieUpdateRequest>();

        CreateMap<TVShowCreateDto, TVShowCreateRequest>();
        CreateMap<TVShowUpdateDto, TVShowUpdateRequest>();

        CreateMap<EpisodeCreateDto, EpisodeCreateRequest>();
        CreateMap<EpisodeUpdateDto, EpisodeUpdateRequest>();

        CreateMap<ProfileUpdateDto, ProfileUpdateRequest>();
    }
}