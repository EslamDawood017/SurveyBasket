using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;
using System.Reflection;

namespace SurveyBasket.Api;

public static class DependancyInjections
{
    public static IServiceCollection AddDependancies(this IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IPollService, PollService>();
        //Adding Mapster 
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
