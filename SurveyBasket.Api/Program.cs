using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Data;

namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ConnectionString = builder.Configuration.GetConnectionString("DefualtConnection");

            builder.Services.AddDependancies();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(ConnectionString));
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
