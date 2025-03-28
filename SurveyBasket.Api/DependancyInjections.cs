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

namespace SurveyBasket.Api;

public static class DependancyInjections
{
    public static IServiceCollection AddDependancies(this IServiceCollection services)
    {
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider , JwtProvider>();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UbRHQ0TEUHszoa9BQ3EeDUCFW0DMPAhM")),
                ValidIssuer = "SurveyBasketApp" ,
                ValidAudience = "SurveyBasketApp Users"
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
