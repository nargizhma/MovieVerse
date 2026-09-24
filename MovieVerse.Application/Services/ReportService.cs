using MovieVerse.Abstractions.Reports;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.Reports;
using MovieVerse.Dtos.Writers;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class ReportService(
    IUserProfileService profileService,
    IWatchlistService watchlistService,
    IWatchHistoryService watchHistoryService,
    IActorService actorService,
    IDirectorService directorService,
    IWriterService writerService,
    IPdfReportGenerator pdfReportGenerator)
    : IReportService
{
    public async Task<GeneratedReportDto>
        GenerateMyReportAsync(
            Guid userId)
    {
        var profile =
            await profileService
                .GetMineAsync(userId);

        var reviews =
            await profileService
                .GetMyActivityAsync(userId);

        var watchlist =
            await watchlistService
                .GetMineAsync(userId);

        var watchHistory =
            await watchHistoryService
                .GetMineAsync(userId);


        var reportData =
            new MyMovieVerseReportDto
            {
                Profile = profile,

                Reviews = reviews,

                Watchlist = watchlist,

                WatchHistory =
                    watchHistory,

                GeneratedAt =
                    DateTime.UtcNow
            };


        var pdf =
            pdfReportGenerator
                .GenerateMyMovieVerseReport(
                    reportData);


        var ownerName =
            !string.IsNullOrWhiteSpace(
                profile.DisplayName)
                ? profile.DisplayName
                : profile.UserName;


        return new GeneratedReportDto
        {
            Content = pdf,

            FileName =
                $"MovieVerse-{ToFileSafeName(ownerName)}-Report.pdf"
        };
    }


    public async Task<GeneratedReportDto>
        GeneratePersonReportAsync(
            string personType,
            Guid personId)
    {
        var type =
            personType
                .Trim()
                .ToLowerInvariant();


        PersonReportDto reportData;


        switch (type)
        {
            case "actor":
                {
                    var actor =
                        await actorService
                            .GetByIdAsync(personId);

                    reportData =
                        CreateActorReport(actor);

                    break;
                }


            case "director":
                {
                    var director =
                        await directorService
                            .GetByIdAsync(personId);

                    reportData =
                        CreateDirectorReport(
                            director);

                    break;
                }


            case "writer":
                {
                    var writer =
                        await writerService
                            .GetByIdAsync(personId);

                    reportData =
                        CreateWriterReport(
                            writer);

                    break;
                }


            default:
                throw new BadRequestException(
                    "Person type must be actor, director, or writer.");
        }


        reportData.GeneratedAt =
            DateTime.UtcNow;


        var pdf =
            pdfReportGenerator
                .GeneratePersonReport(
                    reportData);


        return new GeneratedReportDto
        {
            Content = pdf,

            FileName =
                $"MovieVerse-{ToFileSafeName(reportData.FullName)}-Report.pdf"
        };
    }


    private static PersonReportDto
        CreateActorReport(
            ActorDetailsDto actor)
    {
        return new PersonReportDto
        {
            PersonId = actor.Id,

            PersonType = "Actor",

            FullName = actor.FullName,

            ProfileImageUrl =
                actor.ProfileImageUrl,

            Biography =
                actor.Biography,

            BirthDate =
                actor.BirthDate,

            BirthPlace =
                actor.BirthPlace,

            DeathDate =
                actor.DeathDate,

            DeathPlace =
                actor.DeathPlace,

            HeightInMeters =
                actor.HeightInMeters,

            AlternativeName =
                actor.AlternativeName,

            Nickname =
                actor.Nickname,

            Spouse =
                actor.Spouse,

            Children =
                actor.Children,

            Parents =
                actor.Parents,

            Relatives =
                actor.Relatives,

            OtherWorks =
                actor.OtherWorks,

            Trivia =
                actor.Trivia,

            Quote =
                actor.Quote,

            Trademark =
                actor.Trademark,

            Filmography =
                actor.Filmography
        };
    }


    private static PersonReportDto
        CreateDirectorReport(
            DirectorDetailsDto director)
    {
        return new PersonReportDto
        {
            PersonId = director.Id,

            PersonType = "Director",

            FullName = director.FullName,

            ProfileImageUrl =
                director.ProfileImageUrl,

            Biography =
                director.Biography,

            BirthDate =
                director.BirthDate,

            BirthPlace =
                director.BirthPlace,

            DeathDate =
                director.DeathDate,

            DeathPlace =
                director.DeathPlace,

            HeightInMeters =
                director.HeightInMeters,

            AlternativeName =
                director.AlternativeName,

            Nickname =
                director.Nickname,

            Spouse =
                director.Spouse,

            Children =
                director.Children,

            Parents =
                director.Parents,

            Relatives =
                director.Relatives,

            OtherWorks =
                director.OtherWorks,

            Trivia =
                director.Trivia,

            Quote =
                director.Quote,

            Trademark =
                director.Trademark,

            Filmography =
                director.Filmography
        };
    }


    private static PersonReportDto
        CreateWriterReport(
            WriterDetailsDto writer)
    {
        return new PersonReportDto
        {
            PersonId = writer.Id,

            PersonType = "Writer",

            FullName = writer.FullName,

            ProfileImageUrl =
                writer.ProfileImageUrl,

            Biography =
                writer.Biography,

            BirthDate =
                writer.BirthDate,

            BirthPlace =
                writer.BirthPlace,

            DeathDate =
                writer.DeathDate,

            DeathPlace =
                writer.DeathPlace,

            HeightInMeters =
                writer.HeightInMeters,

            AlternativeName =
                writer.AlternativeName,

            Nickname =
                writer.Nickname,

            Spouse =
                writer.Spouse,

            Children =
                writer.Children,

            Parents =
                writer.Parents,

            Relatives =
                writer.Relatives,

            OtherWorks =
                writer.OtherWorks,

            Trivia =
                writer.Trivia,

            Quote =
                writer.Quote,

            Trademark =
                writer.Trademark,

            Filmography =
                writer.Filmography
        };
    }


    private static string ToFileSafeName(
        string value)
    {
        var characters =
            value
                .Trim()
                .Select(character =>
                    char.IsLetterOrDigit(character)
                        ? character
                        : '-')
                .ToArray();


        var result =
            new string(characters);


        while (
            result.Contains("--"))
        {
            result =
                result.Replace(
                    "--",
                    "-");
        }


        result =
            result.Trim('-');


        return string.IsNullOrWhiteSpace(
            result)
            ? "Report"
            : result;
    }
}