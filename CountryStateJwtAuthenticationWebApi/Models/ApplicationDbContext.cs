using CountryStateJwtAuthenticationWebApi.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CountryStateJwtAuthenticationWebApi.Models
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<CountryAspSubhajit> Countries { get; set; }
        public DbSet<SatateAspSubhajit> States { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("AspNetCoreRegistration_Subhajit");
            builder.Entity<IdentityRole>().ToTable("MyApp_Roles_AspSubhajit");
            builder.Entity<IdentityUserRole<string>>().ToTable("MyApp_UserRoles_AspSubhajit");
            builder.Entity<IdentityUserClaim<string>>().ToTable("MyApp_UserClaims_AspSubhajit");
            builder.Entity<IdentityUserLogin<string>>().ToTable("MyApp_UserLogins_AspSubhajit");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("MyApp_RoleClaims_AspSubhajit");
            builder.Entity<IdentityUserToken<string>>().ToTable("MyApp_UserTokens_AspSubhajit");

            builder.Entity<CountryAspSubhajit>().ToTable("Country_AspSubhajit");
            builder.Entity<SatateAspSubhajit>().ToTable("State_AspSubhajit");
        }
    }
}
