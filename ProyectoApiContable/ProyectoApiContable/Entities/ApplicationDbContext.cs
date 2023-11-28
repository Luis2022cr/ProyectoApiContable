 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProyectoApiContable.Entities;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<FilasPartida> FilasPartidas { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }


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

            modelBuilder.Entity<Partida>()
                .HasMany(p => p.FilasPartida)
                .WithOne(fp => fp.Partida)
                .HasForeignKey(fp => fp.PartidaId);

            modelBuilder.Entity<Cuenta>()
                .HasMany(c => c.FilasPartida)
                .WithOne(fp => fp.Cuenta)
                .HasForeignKey(fp => fp.CuentaId);
        }
    };
