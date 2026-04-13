using Ferreteria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Infrastructure.Data
{
    public class FerreteriaDbContext : DbContext
    {
        public FerreteriaDbContext(DbContextOptions<FerreteriaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API configurations
            
            // Decimal precision
            modelBuilder.Entity<Producto>().Property(x => x.PrecioCosto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>().Property(x => x.MargenGanancia).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>().Property(x => x.PrecioVenta).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>().Property(x => x.Itbis).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Factura>().Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Factura>().Property(x => x.DescuentoTotal).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Factura>().Property(x => x.ImpuestosTotal).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Factura>().Property(x => x.TotalGeneral).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Factura>().Property(x => x.MontoRecibido).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Factura>().Property(x => x.Cambio).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FacturaDetalle>().Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FacturaDetalle>().Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FacturaDetalle>().Property(x => x.Descuento).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FacturaDetalle>().Property(x => x.ImpuestoPorcentaje).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FacturaDetalle>().Property(x => x.ImpuestoMonto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FacturaDetalle>().Property(x => x.TotalLinea).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MovimientoInventario>().Property(x => x.CostoUnitario).HasColumnType("decimal(18,2)");

            // Unique constraints
            modelBuilder.Entity<Cliente>().HasIndex(x => x.NumeroIdentificacion).IsUnique();
            modelBuilder.Entity<Producto>().HasIndex(x => x.Sku).IsUnique();
        }
    }
}
