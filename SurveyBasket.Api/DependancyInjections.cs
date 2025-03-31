using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api;

public static class DependancyInjections
{
   

    public static IServiceCollection AddDependancies(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider , JwtProvider>();

        //services.AddExceptionHandler<GlobalExceptionHandler>();
        //services.AddProblemDetails();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        Console.WriteLine($"Key: {jwtOptions.Key} => Issuer : {jwtOptions.Issuer} => aduiense {jwtOptions.Audience}");
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key)),
                ValidIssuer = jwtOptions?.Issuer ,
                ValidAudience = jwtOptions?.Audience ,
            };
        });
        //Adding Mapster 
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddFluentValidationAutoValidation();
        return services;
    }
}
