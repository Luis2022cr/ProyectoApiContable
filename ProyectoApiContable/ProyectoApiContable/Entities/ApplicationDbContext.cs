 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProyectoApiContable.Entities;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<FilasPartida> FilasPartidas { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        
        public DbSet<EstadoPartida> EstadosPartidas { get; set; }
        
        public DbSet<TipoCuenta> TiposCuentas { get; set; }

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
            
            //Que el codigo de cada cuenta sea unico
            modelBuilder.Entity<Cuenta>()
                .HasIndex(x => x.Codigo)
                .IsUnique(true);
          
            // Configuración de relaciones para Partida
            modelBuilder.Entity<Partida>()
                .HasOne(p => p.CreadoPor)
                .WithMany()
                .HasForeignKey(p => p.CreadoPorId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar la eliminación en cascada

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.RevisadoPor)
                .WithMany()
                .HasForeignKey(p => p.RevisadoPorId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar la eliminación en cascada


            modelBuilder.Entity<Partida>()
                .HasOne(p => p.EstadoPartida)
                .WithMany(ep => ep.Partidas)
                .HasForeignKey(p => p.EstadoPartidaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relaciones para FilasPartida
            modelBuilder.Entity<FilasPartida>()
                .HasOne(fp => fp.Cuenta)
                .WithMany(c => c.FilasPartida)
                .HasForeignKey(fp => fp.CuentaId);

            modelBuilder.Entity<FilasPartida>()
                .HasOne(fp => fp.Partida)
                .WithMany(p => p.FilasPartida)
                .HasForeignKey(fp => fp.PartidaId);

            // Configuración de relaciones para Cuenta y TipoCuenta
            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.TipoCuenta)
                .WithMany(tc => tc.Cuentas)
                .HasForeignKey(c => c.TipoCuentaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración específica para TipoCuenta
            modelBuilder.Entity<TipoCuenta>()
                .ToTable("tipos_cuentas");

            // Configuración específica para EstadoPartida
            modelBuilder.Entity<EstadoPartida>()
                .ToTable("estados_partidas");
            
            
        }
    };
