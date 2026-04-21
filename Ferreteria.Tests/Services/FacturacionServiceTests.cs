using Ferreteria.Application.DTOs.Facturacion;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class FacturacionServiceTests
    {
        [Fact]
        public async Task ProcesarFacturaAsync_DebeDeducirInventarioYCrearFactura()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Cat" });
            var p = new Producto { Id = 1, Sku="1", Nombre="P1", StockActual = 10, PrecioVenta = 50, TieneItbis = true, Descripcion="", Marca="", CodigoBarra="", UnidadMedida="", CategoriaId = 1 };
            var p2 = new Producto { Id = 2, Sku="2", Nombre="P2", StockActual = 5, PrecioVenta = 100, TieneItbis = false, Descripcion="", Marca="", CodigoBarra="", UnidadMedida="", CategoriaId = 1 };
            var c = new Cliente { Id = 1, NombreRazonSocial = "C1", NumeroIdentificacion="", TipoIdentificacion="", Direccion="", Telefono="", Estado="Activo" };
            var u = new Usuario { Id = 99, Username = "test", PasswordHash="", NombreCompleto="T", RolId=1, Estado="Activo" };
            context.Productos.AddRange(p, p2);
            context.Clientes.Add(c);
            context.Usuarios.Add(u);
            context.SaveChanges();

            var service = new FacturacionService(context);
            var dto = new NuevaFacturaDto
            {
                ClienteId = 1,
                MetodoPago = "Tarjeta",
                Detalles = new List<FacturaDetalleDto>
                {
                    new FacturaDetalleDto { ProductoId = 1, Cantidad = 2, Descuento = 0 }, // Itbis true (50 * 2 = 100) -> 18 ITBIS
                    new FacturaDetalleDto { ProductoId = 2, Cantidad = 1, Descuento = 0 }  // Itbis false (100 * 1 = 100) -> 0 ITBIS
                }
            };

            // Act
            var res = await service.ProcesarFacturaAsync(dto, 99);

            // Assert
            var fac = context.Facturas.Include(f => f.Detalles).FirstOrDefault();
            Assert.NotNull(fac);
            Assert.Equal("Completada", fac.Estado);
            Assert.Equal(200, fac.Subtotal);
            Assert.Equal(18, fac.ImpuestosTotal);
            Assert.Equal(218, fac.TotalGeneral);

            // Check stock deduction
            Assert.Equal(8, context.Productos.Find(1).StockActual);
            Assert.Equal(4, context.Productos.Find(2).StockActual);

            // Check Inventory Movement record
            Assert.Equal(2, context.MovimientosInventario.Count());
            Assert.True(context.MovimientosInventario.All(t => t.TipoMovimiento == "Salida"));
        }

        [Fact]
        public async Task AnularFacturaAsync_DebeRevertirInventarioYCambiarEstado()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var p = new Producto { Id = 1, Sku="1", Nombre="P1", StockActual = 8, Descripcion="", Marca="", CodigoBarra="", UnidadMedida="", PrecioVenta=50 };
            context.Productos.Add(p);
            
            var f = new Factura 
            { 
                Id = 1, Estado = "Completada", NumeroFactura = "FAC", MetodoPago = "Efectivo", 
                Detalles = new List<FacturaDetalle> { new FacturaDetalle { ProductoId = 1, Cantidad = 2, TotalLinea = 100, PrecioUnitario = 50 } }
            };
            context.Facturas.Add(f);
            context.SaveChanges();

            var service = new FacturacionService(context);

            // Act
            var anulado = await service.AnularFacturaAsync(1, "Admin");

            // Assert
            Assert.True(anulado);
            Assert.Equal("Anulada", context.Facturas.Find(1).Estado);
            Assert.Equal(10, context.Productos.Find(1).StockActual); // Restored 2 units
        }

        [Fact]
        public async Task ProcesarFacturaAsync_FallaSiStockInsuficiente()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Herramientas" });
            context.Productos.Add(new Producto { Id = 1, Sku="1", Nombre="Martillo", StockActual = 2, PrecioVenta = 50, CategoriaId = 1 });
            context.Clientes.Add(new Cliente { Id = 1, NombreRazonSocial = "Juan", NumeroIdentificacion="111" });
            context.Usuarios.Add(new Usuario { Id = 1, Username = "test" });
            context.SaveChanges();

            var service = new FacturacionService(context);

            var dto = new NuevaFacturaDto
            {
                ClienteId = 1,
                MetodoPago = "Efectivo",
                Detalles = new List<FacturaDetalleDto>
                {
                    new FacturaDetalleDto { ProductoId = 1, Cantidad = 5 } // Trying to sell 5, but we only have 2
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.ProcesarFacturaAsync(dto, 1));
            Assert.Contains("Stock insuficiente o inválido para producto", ex.Message);
        }
    }
}
