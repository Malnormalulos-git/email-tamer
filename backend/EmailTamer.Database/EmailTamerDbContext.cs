using EmailTamer.Database.Entities;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database;

public class EmailTamerDbContext(
    DbContextOptions<EmailTamerDbContext> options,
    TimeProvider timeProvider)
    : IdentityDbContext<EmailTamerUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmailTamerUser>()
            .HasIndex(b => b.Email)
            .IsUnique();
        
        modelBuilder.SeedRoles();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        
        foreach (var entry in entries)
            if (entry.Entity is IDateAuditableEntity entity)
            {
                var now = timeProvider.GetUtcNow().UtcDateTime;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entity.ModifiedAt = now;
                        break;
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.ModifiedAt = now;
                        break;
                }
            }
    }
}