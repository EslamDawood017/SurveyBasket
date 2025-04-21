using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Extentions;
using SurveyBasket.Api.HealthChecks;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;
using SurveyBasket.Api.Settings;
using SurveyBasket.Authentication.Filters;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class DependancyInjections
{
    public static IServiceCollection AddDependancies(this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("DefualtConnection");

        services.AddControllers();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));


        services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));

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


        // Configure Swagger to use JWT Bearer Token
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            // Enable JWT in Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer {your token here}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
        });

        services.AddDistributedMemoryCache();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailSender, MailService>();
        services.AddScoped<IRoleSerivce, RoleService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IDistributedCashService, DistributedCashService>();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddOptions<MailSettings>()
            .BindConfiguration(nameof(MailSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

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

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

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
                ValidIssuer = jwtOptions?.Issuer,
                ValidAudience = jwtOptions?.Audience,
            };
        });


        //Adding Mapster 
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddFluentValidationAutoValidation();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy("IPLimiter", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )

            );

            rateLimiterOptions.AddPolicy("UserLimiter", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )

            );


            rateLimiterOptions.AddConcurrencyLimiter("Concurrency", options =>
            {
                options.PermitLimit = 1000;
                options.QueueLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            //2
            //rateLimiterOptions.AddTokenBucketLimiter("tokens", options =>
            //{
            //    options.TokenLimit = 2;
            //    options.QueueLimit = 1;
            //    options.TokensPerPeriod = 2;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //    options.AutoReplenishment = true;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            //3
            //rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.Window = TimeSpan.FromSeconds(20);

            //});

            //4
            //rateLimiterOptions.AddSlidingWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.QueueLimit = 1;
            //    options.SegmentsPerWindow = 2;   
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.Window = TimeSpan.FromSeconds(20);

            //});
        });

        services.AddHealthChecks()
            .AddSqlServer(name: "data base", connectionString: configuration.GetConnectionString("DefualtConnection")!)
            .AddCheck<MailProviderHealthCheck>(name: "Mail provider");

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1);

            options.ReportApiVersions = true;

            options.ApiVersionReader = new HeaderApiVersionReader("api-version");

        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });



        return services;
    }
}
