using Ferreteria.Application.DTOs.Inventario;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using Ferreteria.Tests.Services;
using System.Threading.Tasks;

class Program {
    static async Task Main() {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        context.Productos.Add(new Producto { Id = 1, Nombre = "Taladro", StockActual = 10, Sku="A", Descripcion="", Marca="", CodigoBarra="", UnidadMedida="" });
        context.SaveChanges();

        var dto = new MovimientoDto 
        { 
            ProductoId = 1, Cantidad = 5, TipoMovimiento = "Entrada", 
            Motivo = "Compra a proovedor", CostoUnitario = 1500 
        };

        try
        {
            var producto = await context.Productos.FindAsync(dto.ProductoId);
            producto.StockActual += dto.Cantidad;
            producto.PrecioCosto = dto.CostoUnitario;
            producto.PrecioVenta = producto.PrecioCosto + (producto.PrecioCosto * (producto.MargenGanancia / 100));

            context.Productos.Update(producto);

            var movimiento = new MovimientoInventario
            {
                ProductoId = dto.ProductoId,
                TipoMovimiento = "Entrada",
                Motivo = "Compra",
                Cantidad = dto.Cantidad,
                CostoUnitario = dto.CostoUnitario,
                ProveedorOReferencia = dto.ProveedorOReferencia ?? "",
                UsuarioCreacion = "Admin",
                FechaCreacion = DateTime.UtcNow
            };

            context.MovimientosInventario.Add(movimiento);
            await context.SaveChangesAsync();
            Console.WriteLine("Success");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.ToString());
        }
    }
}
