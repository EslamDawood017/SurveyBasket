using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Data.EntitiesConfig;
using System.Reflection;


namespace SurveyBasket.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Poll> Polls { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
       
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
        base.OnModelCreating(modelBuilder);
    }
}
