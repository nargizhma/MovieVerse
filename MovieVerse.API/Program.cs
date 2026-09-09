using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MovieVerse.Abstractions.Media;
using MovieVerse.Application;
using MovieVerse.Data.Seed;
using MovieVerse.Handlers;
using MovieVerse.Infrastructure;
using MovieVerse.Profiles;
using MovieVerse.Services;
using MovieVerse.Settings;

var builder =
    WebApplication.CreateBuilder(args);


builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory =
            context =>
            {
                var errors =
                    context.ModelState
                        .Where(x =>
                            x.Value?.Errors.Count > 0)
                        .SelectMany(x =>
                            x.Value!.Errors.Select(
                                error =>
                                    string.IsNullOrWhiteSpace(
                                        error.ErrorMessage)
                                        ? $"Invalid value for {x.Key}."
                                        : error.ErrorMessage))
                        .Distinct()
                        .ToArray();

                return new BadRequestObjectResult(
                    new ProblemDetails
                    {
                        Status =
                            StatusCodes
                                .Status400BadRequest,

                        Title =
                            "Bad Request",

                        Detail =
                            errors.Length > 0
                                ? string.Join(
                                    " ",
                                    errors)
                                : "The submitted data is invalid."
                    });
            };
    });

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddApplication();

var webRootPath =
    builder.Environment.WebRootPath
    ?? Path.Combine(
        builder.Environment.ContentRootPath,
        "wwwroot");

builder.Services.AddInfrastructure(
    builder.Configuration,
    webRootPath);


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    IMediaUrlBuilder,
    HttpMediaUrlBuilder>();


builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MapperProfile),
    typeof(ApiMappingProfile));


builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Frontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5500",
                    "http://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi
            .OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                Microsoft.OpenApi
                    .SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In =
                Microsoft.OpenApi
                    .ParameterLocation.Header,

            Description =
                "JWT Authorization header using the Bearer scheme."
        });

    c.AddSecurityRequirement(
        document =>
            new Microsoft.OpenApi
                .OpenApiSecurityRequirement
            {
                [
                    new Microsoft.OpenApi
                        .OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)
                ] = []
            });
});


var jwtSettings =
    builder.Configuration
        .GetSection(
            JwtSettings.SectionName)
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JWT settings are missing.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults
                .AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults
                .AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSettings.Issuer,

                ValidAudience =
                    jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                ClockSkew =
                    TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();


// ==================================================
// EXCEPTION HANDLING
// ==================================================

builder.Services
    .AddExceptionHandler<
        GlobalExceptionHandler>();

builder.Services.AddProblemDetails();


var app = builder.Build();


app.UseExceptionHandler();

using (var scope =
       app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(
        scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();