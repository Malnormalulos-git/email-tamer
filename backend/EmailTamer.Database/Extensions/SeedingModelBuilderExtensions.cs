using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Extensions;

internal static class SeedingModelBuilderExtensions
{
    public static ModelBuilder SeedRoles(this ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "00000000-0000-0000-0000-000000000001", Name = "User", NormalizedName = "USER" },
            new IdentityRole { Id = "00000000-0000-0000-0000-000000000002", Name = "Admin", NormalizedName = "ADMIN" }
        );

        return builder;
    }
}