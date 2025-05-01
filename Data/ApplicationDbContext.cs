using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soft_W_C.Models;

namespace Soft_W_C.Data;

public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Soft_W_C.Models.Cliente> Cliente { get; set; }
    public DbSet<Soft_W_C.Models.Sede> Sede { get; set; }
    public DbSet<Soft_W_C.Models.Servicio> Servicio { get; set; }
    public DbSet<Soft_W_C.Models.Tareo> Tareo { get; set; }
    public DbSet<Soft_W_C.Models.Geolocalizacion_asistencia> Geo_asis { get; set; }
    public DbSet<Soft_W_C.Models.Asistencia> Asistencia { get; set; }
    public DbSet<Soft_W_C.Models.Usuario> Usuario { get; set; }
}
