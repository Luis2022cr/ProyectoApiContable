using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProyectoApiContable.Entities
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<CatalogoCuentas> CatalogoCuentas { get; set; }
        public DbSet<PartidasContables> PartidasContables { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<IdentityUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("users_roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("users_logins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");

            // Configuración de la relación entre CatalogoCuentas y PartidasContables
            modelBuilder.Entity<PartidasContables>()
                .HasOne(p => p.CuentaDebito)
                .WithMany(c => c.PartidasContablesDebito)
                .HasForeignKey(p => p.CuentaDebitoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidasContables>()
                .HasOne(p => p.CuentaCredito)
                .WithMany(c => c.PartidasContablesCredito)
                .HasForeignKey(p => p.CuentaCreditoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
