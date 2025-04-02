using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Data.EntitiesConfig;
using System.Reflection;
using System.Security.Claims;


namespace SurveyBasket.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser , ApplicationRole , int>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options , IHttpContextAccessor httpContextAccessor) : base(options)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var CascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where( fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach(var fk in CascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }    

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var CurrentUserID = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        int.TryParse(CurrentUserID, out var NewUserID);

        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if(entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedById).CurrentValue = NewUserID;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedById).CurrentValue = NewUserID;

                entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow; 
            }

        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
