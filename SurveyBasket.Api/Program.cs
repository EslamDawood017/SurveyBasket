using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;
using Serilog;
using Hangfire;
using HangfireBasicAuthenticationFilter;

namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

         
            var ConnectionString = builder.Configuration.GetConnectionString("DefualtConnection");
           
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            builder.Services.AddDependancies(builder.Configuration);
       
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
             
            builder.Services.AddDistributedMemoryCache();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Configure Swagger to use JWT Bearer Token
            builder.Services.AddSwaggerGen(c =>
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangfireSettings:username"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:password")
                    }

                ],
                DashboardTitle = "Survey Basket Dashboard"
            });

         
            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var Scope = scopeFactory.CreateScope();
            var notificationService = Scope.ServiceProvider.GetService<INotificationService>();

            RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            //app.UseExceptionHandler();

            app.Run();

            
        }
    }
}
