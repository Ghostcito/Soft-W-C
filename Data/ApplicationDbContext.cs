using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftWC.Models;

namespace SoftWC.Data;

public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SoftWC.Models.Cliente> Cliente { get; set; }
    public DbSet<SoftWC.Models.Sede> Sede { get; set; }
    public DbSet<SoftWC.Models.Servicio> Servicio { get; set; }
    public DbSet<SoftWC.Models.Tareo> Tareo { get; set; }
    public DbSet<SoftWC.Models.Geolocalizacion_asistencia> Geo_asis { get; set; }
    public DbSet<SoftWC.Models.Asistencia> Asistencia { get; set; }
    public DbSet<SoftWC.Models.Usuario> Usuario { get; set; }
    public DbSet<SoftWC.Models.Supervision> Supervision { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configuración de la relación entre Usuario y Supervision
        modelBuilder.Entity<Supervision>()
            .HasOne(s => s.Supervisor)
            .WithMany(u => u.EmpleadosSupervisados)
            .HasForeignKey(s => s.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Supervision>()
            .HasOne(s => s.Empleado)
            .WithMany(u => u.SupervisoresAsignados)
            .HasForeignKey(s => s.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
