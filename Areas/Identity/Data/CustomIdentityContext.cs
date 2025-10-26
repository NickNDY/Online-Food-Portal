using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Data;

public class CustomIdentityContext : IdentityDbContext<IdentityUserModel>
{
    public CustomIdentityContext(DbContextOptions<CustomIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Set Primary Key to UserName
        builder.Entity<IdentityUserModel>().HasIndex(u => u.UserName).IsUnique();
        builder.Entity<IdentityUserModel>().HasKey(u => u.UserName);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "User", NormalizedName = "User".ToUpper() },
            new IdentityRole { Id = "2", Name = "Kitchen", NormalizedName = "Kitchen".ToUpper() },
            new IdentityRole { Id = "3", Name = "Administrator", NormalizedName = "Administrator".ToUpper() }
        );

    }
}
