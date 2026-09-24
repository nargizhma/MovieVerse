using MovieVerse.Abstractions.Reports;
using MovieVerse.Dtos.Reports;
using MovieVerse.Dtos.UserLibrary;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MovieVerse.Infrastructure.Reports;

public class QuestPdfReportGenerator
    : IPdfReportGenerator
{
    public byte[] GenerateMyMovieVerseReport(
        MyMovieVerseReportDto report)
    {
        var document =
            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(
                        PageSizes.A4);

                    page.Margin(36);

                    page.DefaultTextStyle(
                        style =>
                            style
                                .FontSize(10)
                                .FontColor(
                                    Colors.Grey
                                        .Darken3));


                    page.Header()
                        .Element(container =>
                            ComposeHeader(
                                container,
                                report));


                    page.Content()
                        .PaddingVertical(20)
                        .Element(container =>
                            ComposeContent(
                                container,
                                report));


                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text
                                .Span(
                                    "MovieVerse • Page ")
                                .FontColor(
                                    Colors.Grey
                                        .Medium);

                            text
                                .CurrentPageNumber();

                            text
                                .Span(" of ");

                            text
                                .TotalPages();
                        });
                });
            });


        /*
         * GeneratePdf() returns the completed
         * PDF as byte[].
         */

        return document.GeneratePdf();
    }


    private static void ComposeHeader(
        IContainer container,
        MyMovieVerseReportDto report)
    {
        container
            .BorderBottom(2)
            .BorderColor(
                Colors.Blue.Darken2)
            .PaddingBottom(12)
            .Row(row =>
            {
                row.RelativeItem()
                    .Column(column =>
                    {
                        column
                            .Item()
                            .Text("MOVIEVERSE")
                            .FontSize(22)
                            .Bold()
                            .FontColor(
                                Colors.Blue
                                    .Darken2);


                        column
                            .Item()
                            .Text(
                                "Personal Activity Report")
                            .FontSize(13)
                            .SemiBold();
                    });


                row.RelativeItem()
                    .AlignRight()
                    .Column(column =>
                    {
                        column
                            .Item()
                            .AlignRight()
                            .Text(
                                "Generated")
                            .FontSize(8)
                            .FontColor(
                                Colors.Grey
                                    .Medium);


                        column
                            .Item()
                            .AlignRight()
                            .Text(
                                report.GeneratedAt
                                    .ToString(
                                        "dd MMMM yyyy"))
                            .SemiBold();


                        column
                            .Item()
                            .AlignRight()
                            .Text(
                                report.GeneratedAt
                                    .ToString(
                                        "HH:mm 'UTC'"))
                            .FontSize(8)
                            .FontColor(
                                Colors.Grey
                                    .Medium);
                    });
            });
    }


    private static void ComposeContent(
        IContainer container,
        MyMovieVerseReportDto report)
    {
        var profile =
            report.Profile;


        var displayName =
            !string.IsNullOrWhiteSpace(
                profile.DisplayName)
                ? profile.DisplayName
                : profile.UserName;


        var averageRating =
            report.Reviews.Count == 0
                ? (decimal?)null
                : report.Reviews
                    .Average(x =>
                        x.Rating);


        container.Column(column =>
        {
            column.Spacing(16);


            /*
             * PROFILE
             */

            column.Item()
                .Column(profileColumn =>
                {
                    profileColumn
                        .Item()
                        .Text(displayName)
                        .FontSize(26)
                        .Bold();


                    profileColumn
                        .Item()
                        .Text(
                            $"@{profile.UserName}")
                        .FontSize(11)
                        .FontColor(
                            Colors.Grey
                                .Medium);


                    if (!string.IsNullOrWhiteSpace(
                            profile.Bio))
                    {
                        profileColumn
                            .Item()
                            .PaddingTop(6)
                            .Text(profile.Bio)
                            .FontSize(10);
                    }
                });


            /*
             * OVERVIEW
             */

            column.Item()
                .Element(SectionHeader)
                .Text("Overview")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            column.Item()
                .Table(table =>
                {
                    table.ColumnsDefinition(
                        columns =>
                        {
                            columns
                                .RelativeColumn();

                            columns
                                .RelativeColumn();

                            columns
                                .RelativeColumn();
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    report
                                        .Reviews
                                        .Count
                                        .ToString())
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text("Reviews")
                                .FontSize(9);
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    report
                                        .WatchHistory
                                        .Count
                                        .ToString())
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text("Watched")
                                .FontSize(9);
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    averageRating
                                        ?.ToString(
                                            "0.0")
                                    ?? "-")
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text(
                                    "Average rating")
                                .FontSize(9);
                        });
                });


            column.Item()
                .Table(table =>
                {
                    table.ColumnsDefinition(
                        columns =>
                        {
                            columns
                                .RelativeColumn();

                            columns
                                .RelativeColumn();

                            columns
                                .RelativeColumn();
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    report
                                        .Watchlist
                                        .Count
                                        .ToString())
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text("Watchlist")
                                .FontSize(9);
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    profile
                                        .MovieReviewCount
                                        .ToString())
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text(
                                    "Movie reviews")
                                .FontSize(9);
                        });


                    table.Cell()
                        .Element(StatCard)
                        .Column(card =>
                        {
                            card.Item()
                                .Text(
                                    (
                                        profile
                                            .TVShowReviewCount
                                        +
                                        profile
                                            .EpisodeReviewCount
                                    )
                                    .ToString())
                                .FontSize(20)
                                .Bold()
                                .FontColor(
                                    Colors.Blue
                                        .Darken2);

                            card.Item()
                                .Text(
                                    "TV / episode reviews")
                                .FontSize(9);
                        });
                });


            /*
             * REVIEWS
             */

            column.Item()
                .Element(SectionHeader)
                .Text("Ratings & Reviews")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            if (report.Reviews.Count == 0)
            {
                column.Item()
                    .Text(
                        "No ratings or reviews yet.")
                    .FontColor(
                        Colors.Grey
                            .Medium);
            }
            else
            {
                column.Item()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(
                            columns =>
                            {
                                columns
                                    .RelativeColumn(4);

                                columns
                                    .RelativeColumn();

                                columns
                                    .RelativeColumn(2);
                            });


                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Title");

                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Rating");

                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Date");
                        });


                        foreach (
                            var review
                            in report.Reviews)
                        {
                            table.Cell()
                                .Element(TableCell)
                                .Column(reviewColumn =>
                                {
                                    reviewColumn
                                        .Item()
                                        .Text(
                                            GetReviewTitle(
                                                review))
                                        .SemiBold();


                                    reviewColumn
                                        .Item()
                                        .Text(
                                            review
                                                .ContentType)
                                        .FontSize(8)
                                        .FontColor(
                                            Colors.Grey
                                                .Medium);


                                    if (
                                        !string
                                            .IsNullOrWhiteSpace(
                                                review.Content))
                                    {
                                        reviewColumn
                                            .Item()
                                            .PaddingTop(3)
                                            .Text(
                                                review.Content)
                                            .FontSize(9);
                                    }
                                });


                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    $"{review.Rating:0.#}/10")
                                .SemiBold();


                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    review.ActivityAt
                                        .ToString(
                                            "dd MMM yyyy"));
                        }
                    });
            }


            /*
             * WATCHLIST
             */

            column.Item()
                .Element(SectionHeader)
                .Text("Watchlist")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            ComposeLibraryTable(
                column,
                report.Watchlist,
                "Your watchlist is empty.");


            /*
             * WATCH HISTORY
             */

            column.Item()
                .Element(SectionHeader)
                .Text("Watch History")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            ComposeLibraryTable(
                column,
                report.WatchHistory,
                "No watched titles yet.");
        });
    }


    private static void ComposeLibraryTable(
        ColumnDescriptor column,
        List<LibraryItemReturnDto> items,
        string emptyMessage)
    {
        if (items.Count == 0)
        {
            column.Item()
                .Text(emptyMessage)
                .FontColor(
                    Colors.Grey.Medium);

            return;
        }


        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(
                    columns =>
                    {
                        columns
                            .RelativeColumn(4);

                        columns
                            .RelativeColumn();

                        columns
                            .RelativeColumn(2);
                    });


                table.Header(header =>
                {
                    header.Cell()
                        .Element(
                            TableHeaderCell)
                        .Text("Title");

                    header.Cell()
                        .Element(
                            TableHeaderCell)
                        .Text("Type");

                    header.Cell()
                        .Element(
                            TableHeaderCell)
                        .Text("Activity");
                });


                foreach (var item in items)
                {
                    table.Cell()
                        .Element(TableCell)
                        .Column(titleColumn =>
                        {
                            titleColumn
                                .Item()
                                .Text(item.Title)
                                .SemiBold();


                            titleColumn
                                .Item()
                                .Text(
                                    item.ReleaseDate
                                        .Year
                                        .ToString())
                                .FontSize(8)
                                .FontColor(
                                    Colors.Grey
                                        .Medium);
                        });


                    table.Cell()
                        .Element(TableCell)
                        .Text(
                            item.ContentType);


                    table.Cell()
                        .Element(TableCell)
                        .Text(
                            item.ActivityAt
                                .ToString(
                                    "dd MMM yyyy"));
                }
            });
    }


    private static string GetReviewTitle(
        MovieVerse.Dtos.Profiles
            .ProfileActivityItemDto review)
    {
        if (
            review.ContentType
                .Equals(
                    "Episode",
                    StringComparison
                        .OrdinalIgnoreCase)
            &&
            !string.IsNullOrWhiteSpace(
                review.ParentTitle))
        {
            return
                $"{review.ParentTitle} - "
                +
                $"S{review.SeasonNumber:00}"
                +
                $"E{review.EpisodeNumber:00} "
                +
                review.Title;
        }


        return review.Title;
    }


    private static IContainer SectionHeader(
        IContainer container)
    {
        return container
            .PaddingTop(6)
            .PaddingBottom(5)
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey
                    .Lighten2);
    }


    private static IContainer StatCard(
        IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(
                Colors.Grey.Lighten2)
            .Background(
                Colors.Grey.Lighten5)
            .Padding(12)
            .MinHeight(65);
    }


    private static IContainer TableHeaderCell(
        IContainer container)
    {
        return container
            .Background(
                Colors.Blue.Darken2)
            .PaddingVertical(7)
            .PaddingHorizontal(8)
            .DefaultTextStyle(
                style =>
                    style
                        .Bold()
                        .FontSize(9)
                        .FontColor(
                            Colors.White));
    }


    private static IContainer TableCell(
        IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey.Lighten3)
            .PaddingVertical(7)
            .PaddingHorizontal(8);
    }
    public byte[] GeneratePersonReport(
    PersonReportDto report)
    {
        var document =
            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(
                        PageSizes.A4);

                    page.Margin(36);

                    page.DefaultTextStyle(
                        style =>
                            style
                                .FontSize(10)
                                .FontColor(
                                    Colors.Grey
                                        .Darken3));


                    page.Header()
                        .Element(container =>
                            ComposePersonHeader(
                                container,
                                report));


                    page.Content()
                        .PaddingVertical(20)
                        .Element(container =>
                            ComposePersonContent(
                                container,
                                report));


                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text
                                .Span(
                                    "MovieVerse • Page ")
                                .FontColor(
                                    Colors.Grey
                                        .Medium);

                            text
                                .CurrentPageNumber();

                            text
                                .Span(" of ");

                            text
                                .TotalPages();
                        });
                });
            });


        return document.GeneratePdf();
    }
    private static void ComposePersonHeader(
    IContainer container,
    PersonReportDto report)
    {
        container
            .BorderBottom(2)
            .BorderColor(
                Colors.Blue.Darken2)
            .PaddingBottom(12)
            .Row(row =>
            {
                row.RelativeItem()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("MOVIEVERSE")
                            .FontSize(22)
                            .Bold()
                            .FontColor(
                                Colors.Blue
                                    .Darken2);


                        column.Item()
                            .Text(
                                $"{report.PersonType} Report")
                            .FontSize(13)
                            .SemiBold();
                    });


                row.RelativeItem()
                    .AlignRight()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignRight()
                            .Text("Generated")
                            .FontSize(8)
                            .FontColor(
                                Colors.Grey
                                    .Medium);


                        column.Item()
                            .AlignRight()
                            .Text(
                                report.GeneratedAt
                                    .ToString(
                                        "dd MMMM yyyy"))
                            .SemiBold();
                    });
            });
    }


    private static void ComposePersonContent(
        IContainer container,
        PersonReportDto report)
    {
        container.Column(column =>
        {
            column.Spacing(16);


            /*
             * PERSON TITLE
             */

            column.Item()
                .Column(person =>
                {
                    person.Item()
                        .Text(
                            report.FullName)
                        .FontSize(27)
                        .Bold();


                    person.Item()
                        .Text(
                            report.PersonType)
                        .FontSize(12)
                        .FontColor(
                            Colors.Blue
                                .Darken2);


                    if (
                        !string.IsNullOrWhiteSpace(
                            report.Biography))
                    {
                        person.Item()
                            .PaddingTop(10)
                            .Text(
                                report.Biography)
                            .FontSize(10);
                    }
                });


            /*
             * BASIC INFORMATION
             */

            column.Item()
                .Element(SectionHeader)
                .Text(
                    "Personal Information")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            var details =
                GetPersonDetails(report);


            if (details.Count == 0)
            {
                column.Item()
                    .Text(
                        "No personal information is available.")
                    .FontColor(
                        Colors.Grey.Medium);
            }
            else
            {
                column.Item()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(
                            columns =>
                            {
                                columns
                                    .ConstantColumn(120);

                                columns
                                    .RelativeColumn();
                            });


                        foreach (
                            var detail
                            in details)
                        {
                            table.Cell()
                                .Element(
                                    PersonDetailLabel)
                                .Text(
                                    detail.Label);


                            table.Cell()
                                .Element(
                                    PersonDetailValue)
                                .Text(
                                    detail.Value);
                        }
                    });
            }


            /*
             * FILMOGRAPHY
             */

            column.Item()
                .Element(SectionHeader)
                .Text("Filmography")
                .FontSize(15)
                .SemiBold()
                .FontColor(
                    Colors.Blue
                        .Darken2);


            var filmography =
                report.Filmography
                    .OrderByDescending(
                        x => x.ReleaseYear)
                    .ThenBy(
                        x => x.Title)
                    .ToList();


            if (filmography.Count == 0)
            {
                column.Item()
                    .Text(
                        "No filmography is listed.")
                    .FontColor(
                        Colors.Grey
                            .Medium);
            }
            else
            {
                column.Item()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(
                            columns =>
                            {
                                columns
                                    .ConstantColumn(50);

                                columns
                                    .RelativeColumn(4);

                                columns
                                    .ConstantColumn(70);

                                columns
                                    .RelativeColumn(2);
                            });


                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Year");

                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Title");

                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Type");

                            header.Cell()
                                .Element(
                                    TableHeaderCell)
                                .Text("Credit");
                        });


                        foreach (
                            var item
                            in filmography)
                        {
                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    item.ReleaseYear
                                        .ToString());


                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    item.Title)
                                .SemiBold();


                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    item.ContentType
                                        == "TVShow"
                                        ? "TV Show"
                                        : item.ContentType);


                            var credit =
                                GetFilmographyCredit(
                                    report.PersonType,
                                    item);


                            table.Cell()
                                .Element(TableCell)
                                .Text(
                                    string.IsNullOrWhiteSpace(
                                        credit)
                                        ? "—"
                                        : credit);
                        }
                    });
            }


            /*
             * ADDITIONAL INFORMATION
             */

            var additional =
                GetAdditionalDetails(
                    report);


            if (additional.Count > 0)
            {
                column.Item()
                    .Element(SectionHeader)
                    .Text(
                        "Additional Information")
                    .FontSize(15)
                    .SemiBold()
                    .FontColor(
                        Colors.Blue
                            .Darken2);


                foreach (
                    var detail
                    in additional)
                {
                    column.Item()
                        .Column(item =>
                        {
                            item.Item()
                                .Text(
                                    detail.Label)
                                .SemiBold();


                            item.Item()
                                .PaddingTop(2)
                                .Text(
                                    detail.Value)
                                .FontSize(9);
                        });
                }
            }
        });
    }


    private static List<(
        string Label,
        string Value)>
        GetPersonDetails(
            PersonReportDto report)
    {
        var details =
            new List<(
                string Label,
                string Value)>();


        if (report.BirthDate.HasValue)
        {
            details.Add((
                "Born",
                report.BirthDate.Value
                    .ToString(
                        "dd MMMM yyyy")));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.BirthPlace))
        {
            details.Add((
                "Birth place",
                report.BirthPlace));
        }


        if (report.DeathDate.HasValue)
        {
            details.Add((
                "Died",
                report.DeathDate.Value
                    .ToString(
                        "dd MMMM yyyy")));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.DeathPlace))
        {
            details.Add((
                "Death place",
                report.DeathPlace));
        }


        if (
            report.HeightInMeters
                .HasValue)
        {
            details.Add((
                "Height",
                $"{report.HeightInMeters.Value:0.00} m"));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.AlternativeName))
        {
            details.Add((
                "Alternative name",
                report.AlternativeName));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.Nickname))
        {
            details.Add((
                "Nickname",
                report.Nickname));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.Spouse))
        {
            details.Add((
                "Spouse",
                report.Spouse));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.Children))
        {
            details.Add((
                "Children",
                report.Children));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.Parents))
        {
            details.Add((
                "Parents",
                report.Parents));
        }


        if (
            !string.IsNullOrWhiteSpace(
                report.Relatives))
        {
            details.Add((
                "Relatives",
                report.Relatives));
        }


        return details;
    }


    private static List<(
        string Label,
        string Value)>
        GetAdditionalDetails(
            PersonReportDto report)
    {
        var details =
            new List<(
                string Label,
                string Value)>();


        AddOptionalDetail(
            details,
            "Other works",
            report.OtherWorks);


        AddOptionalDetail(
            details,
            "Trivia",
            report.Trivia);


        AddOptionalDetail(
            details,
            "Quote",
            report.Quote);


        AddOptionalDetail(
            details,
            "Trademark",
            report.Trademark);


        return details;
    }


    private static void AddOptionalDetail(
        List<(string Label, string Value)>
            details,
        string label,
        string? value)
    {
        if (
            !string.IsNullOrWhiteSpace(
                value))
        {
            details.Add((
                label,
                value));
        }
    }


    private static string
        GetFilmographyCredit(
            string personType,
            MovieVerse.Dtos.People
                .FilmographyItemDto item)
    {
        var parts =
            new List<string>();


        if (
            personType.Equals(
                "Actor",
                StringComparison
                    .OrdinalIgnoreCase)
            &&
            !string.IsNullOrWhiteSpace(
                item.CharacterName))
        {
            parts.Add(
                $"as {item.CharacterName}");
        }


        if (
            item.EpisodeCount.HasValue
            &&
            item.EpisodeCount.Value > 0)
        {
            var episodeText =
                item.EpisodeCount.Value == 1
                    ? "episode"
                    : "episodes";


            parts.Add(
                $"{item.EpisodeCount.Value} {episodeText}");
        }


        return string.Join(
            " • ",
            parts);
    }


    private static IContainer
        PersonDetailLabel(
            IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey.Lighten3)
            .PaddingVertical(6)
            .PaddingRight(8)
            .DefaultTextStyle(
                style =>
                    style
                        .SemiBold()
                        .FontSize(9));
    }


    private static IContainer
        PersonDetailValue(
            IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey.Lighten3)
            .PaddingVertical(6)
            .PaddingLeft(8)
            .DefaultTextStyle(
                style =>
                    style.FontSize(9));
    }
}