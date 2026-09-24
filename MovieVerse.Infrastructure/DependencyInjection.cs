using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieVerse.Abstractions.AI;
using MovieVerse.Abstractions.Email;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Abstractions.Media;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Abstractions.Reports;
using MovieVerse.Data;
using MovieVerse.Infrastructure.AI;
using MovieVerse.Infrastructure.Email;
using MovieVerse.Infrastructure.Identity;
using MovieVerse.Infrastructure.Media;
using MovieVerse.Infrastructure.Persistence;
using MovieVerse.Infrastructure.Reports;
using MovieVerse.Models;
using MovieVerse.Repositories;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services;
using MovieVerse.Services.Interfaces;
using MovieVerse.Settings;
using QuestPDF.Infrastructure;

namespace MovieVerse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string webRootPath)
    {
        services.AddDbContext<AppDbContext>(
            options =>
                options.UseSqlServer(
                    configuration
                        .GetConnectionString(
                            "DefaultConnection")));

        services
            .AddIdentityCore<AppUser>(
                options =>
                {
                    options.SignIn
                        .RequireConfirmedEmail =
                        true;
                })
            .AddRoles<
                IdentityRole<Guid>>()
            .AddEntityFrameworkStores<
                AppDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));

        services.Configure<GeminiSettings>(
            configuration.GetSection(
                GeminiSettings.SectionName));

        services.AddHttpClient<
            IFilmAiClient,
            GeminiFilmAiClient>();

        services.AddScoped<
            IJwtService,
            JwtService>();

        services.AddScoped<
            IIdentityService,
            IdentityService>();

        services.AddScoped(
            typeof(
                IGenericRepository<>),
            typeof(
                GenericRepository<>));


        services.AddScoped<
            IUserProfileRepository,
            UserProfileRepository>();

        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();

        services.AddSingleton<IFileStorage>(
            new LocalFileStorage(
                webRootPath));

        services.Configure<EmailSettings>(
            configuration.GetSection(
                EmailSettings.SectionName));

        services.Configure<
            DataProtectionTokenProviderOptions>(
            options =>
            {
                options.TokenLifespan =
                    TimeSpan
                        .FromMinutes(30);
            });

        services.AddScoped<
            IEmailService,
            SmtpEmailService>();

        QuestPDF.Settings.License =
            LicenseType.Community;


        services.AddSingleton<
            IPdfReportGenerator,
            QuestPdfReportGenerator>();


        return services;
    }
}