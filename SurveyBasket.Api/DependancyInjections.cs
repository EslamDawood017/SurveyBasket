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
using SurveyBasket.Api.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;

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
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService,ResultService>();
        services.AddScoped<IEmailSender,MailService>();

        services.AddScoped<IDistributedCashService , DistributedCashService>();

        services.AddSingleton<IJwtProvider , JwtProvider>();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = false;
        });

        services.AddHttpContextAccessor();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

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
