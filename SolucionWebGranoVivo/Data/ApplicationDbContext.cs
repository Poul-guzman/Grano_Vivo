using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        
        public DbSet<GuiaSalida> GuiaSalida { get; set; }
        public DbSet<DetalleGuiaSalida> DetalleGuiaSalida { get; set; }

        public DbSet<GuiaEntrada> GuiasEntrada { get; set; }
        public DbSet<DetalleGuiaEntrada> DetallesGuiasEntrada { get; set; }

        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<DetalleCotizacion> DetallesCotizacion { get; set; }

        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<DetalleOrdenCompra> DetallesOrdenCompra { get; set; }

        public DbSet<PedidoCompra> PedidosCompra { get; set; }
        public DbSet<DetallePedidoCompra> DetallesPedidoCompra { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            
            builder.HasDefaultSchema("Identity");
            builder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "AspNetUsers"); });
            builder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "AspNetRoles"); });

            
            builder.Entity<Producto>().Property(p => p.Precio).HasPrecision(10, 2);
            builder.Entity<Producto>().Property(p => p.Peso).HasPrecision(10, 2);

            builder.Entity<Cotizacion>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

            builder.Entity<OrdenCompra>()
            .HasIndex(o => o.Codigo)
            .IsUnique();

            builder.Entity<PedidoCompra>()
            .HasIndex(p => p.Codigo)
            .IsUnique();
        }
    }
}