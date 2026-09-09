using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Abstractions.Media;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Data;
using MovieVerse.Infrastructure.Identity;
using MovieVerse.Infrastructure.Media;
using MovieVerse.Infrastructure.Persistence;
using MovieVerse.Models;
using MovieVerse.Repositories;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services;
using MovieVerse.Services.Interfaces;
using MovieVerse.Settings;

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
            .AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));

        services.AddScoped<
            IJwtService,
            JwtService>();

        services.AddScoped<
            IIdentityService,
            IdentityService>();

        services.AddScoped(
            typeof(IGenericRepository<>),
            typeof(GenericRepository<>));

        services.AddScoped<
            IUserProfileRepository,
            UserProfileRepository>();

        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();

        services.AddSingleton<IFileStorage>(
            new LocalFileStorage(
                webRootPath));

        return services;
    }
}