using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Data.EntitiesConfig;
using System.Reflection;


namespace SurveyBasket.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser , ApplicationRole , int>
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
