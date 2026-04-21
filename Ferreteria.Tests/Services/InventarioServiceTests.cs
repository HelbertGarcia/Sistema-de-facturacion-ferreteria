using Ferreteria.Application.DTOs.Inventario;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class InventarioServiceTests
    {
        [Fact]
        public async Task RegistrarEntradaAsync_DebeAumentarInventarioEnCasoDeEntrada()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Cat" });
            context.Productos.Add(new Producto { Id = 1, Nombre = "Taladro", StockActual = 10, Sku="A", Descripcion="", Marca="", CodigoBarra="", UnidadMedida="", CategoriaId=1 });
            context.SaveChanges();

            var service = new InventarioService(context);
            var dto = new MovimientoDto 
            { 
                ProductoId = 1, Cantidad = 5, TipoMovimiento = "Entrada", 
                Motivo = "Compra a proovedor", CostoUnitario = 1500, ProveedorOReferencia = "Prueba"
            };

            // Act
            var res = await service.RegistrarEntradaAsync(dto, "Admin");

            // Assert
            Assert.True(res);
            Assert.Equal(15, context.Productos.Find(1).StockActual);
            Assert.Equal(1500, context.Productos.Find(1).PrecioCosto); // Entrada updates cost average
        }

        [Fact]
        public async Task RegistrarAjusteAsync_DebeFallarSiSalidaEsMayorAStock()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Cat" });
            context.Productos.Add(new Producto { Id = 1, Nombre = "Taladro", StockActual = 10, Sku="A", Descripcion="", Marca="", CodigoBarra="", UnidadMedida="", CategoriaId=1 });
            context.SaveChanges();

            var service = new InventarioService(context);
            var dto = new MovimientoDto 
            { 
                ProductoId = 1, Cantidad = 15, TipoMovimiento = "Salida", 
                Motivo = "Mercancia Dañada"
            };

            // Act
            var res = await service.RegistrarAjusteAsync(dto, "Admin");

            // Assert
            Assert.False(res);
            Assert.Equal(10, context.Productos.Find(1).StockActual); // Must not change
        }

        [Fact]
        public async Task ObtenerDashboardInvetarioAsync_CalculaMetricasCorrectamente()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            // Producto en buen estado
            context.Productos.Add(new Producto { Id = 1, Nombre = "A", StockActual = 10, StockMinimo = 5, PrecioCosto = 100, Estado = "Activo" });
            // Producto en stock bajo
            context.Productos.Add(new Producto { Id = 2, Nombre = "B", StockActual = 3, StockMinimo = 5, PrecioCosto = 200, Estado = "Activo" });
            // Producto Agotado
            context.Productos.Add(new Producto { Id = 3, Nombre = "C", StockActual = 0, StockMinimo = 5, PrecioCosto = 300, Estado = "Activo" });
            context.SaveChanges();

            var service = new InventarioService(context);

            // Act
            var dashboard = await service.ObtenerDashboardInvetarioAsync();

            // Assert
            Assert.Equal(3, dashboard.TotalProductos);
            Assert.Equal(10 * 100 + 3 * 200, dashboard.TotalValor); // 1000 + 600 = 1600. (The 0 stock doesn't add value)
            Assert.Equal(1, dashboard.ProductosStockBajo); // Producto B
            Assert.Equal(1, dashboard.ProductosAgotados); // Producto C
        }

        [Fact]
        public async Task ObtenerReporteValorizacionAsync_MarcaProductoComoEstancadoSiAplica()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "General" });
            
            // Producto no se ha vendido nunca
            context.Productos.Add(new Producto { Id = 1, Nombre = "Estancado", StockActual = 5, PrecioCosto=10, CategoriaId=1, Estado="Activo" });
            
            // Producto que se vendio recien ayer (No estancado)
            context.Productos.Add(new Producto { Id = 2, Nombre = "Fresco", StockActual = 5, PrecioCosto=10, CategoriaId=1, Estado="Activo" });
            var fac = new Factura { Estado = "Completada", FechaEmision = DateTime.UtcNow.AddDays(-1) };
            context.Facturas.Add(fac);
            context.FacturaDetalles.Add(new FacturaDetalle { FacturaId = fac.Id, ProductoId = 2 });
            context.SaveChanges();

            var service = new InventarioService(context);

            // Act
            var reporte = await service.ObtenerReporteValorizacionAsync();

            // Assert
            var pEstancado = reporte.Productos.First(p => p.ProductoId == 1);
            var pFresco = reporte.Productos.First(p => p.ProductoId == 2);
            
            Assert.True(pEstancado.EstaInactivo);
            Assert.False(pFresco.EstaInactivo);
        }
    }
}
