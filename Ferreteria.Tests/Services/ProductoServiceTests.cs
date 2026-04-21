using Ferreteria.Application.DTOs.Productos;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class ProductoServiceTests
    {
        [Fact]
        public async Task AddAsync_DebeCalcularPrecioDeVentaCorrectamente()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Cat 1" });
            context.SaveChanges();

            var service = new ProductoService(context);
            var dto = new ProductoDto
            {
                Nombre = "Pala",
                CategoriaId = 1,
                PrecioCosto = 100,
                MargenGanancia = 20, // 20%
                TieneItbis = true,
                UnidadMedida = "Pieza",
                StockMinimo = 5
            };

            // Act
            await service.AddAsync(dto);

            // Assert
            var p = context.Productos.First();
            Assert.Equal(120, p.PrecioVenta); // 100 + 20% = 120
            Assert.Equal(18, p.Itbis); // If TieneItbis is true -> 18%
            Assert.Equal(0, p.StockActual); // Starts with 0 stock
            Assert.StartsWith("PROD-", p.Sku); // Sku automatically generated
            Assert.Equal("Activo", p.Estado);
        }

        [Fact]
        public async Task ChangeStatusAsync_DebeCambiarEstado()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var p = new Producto { 
                Sku = "S", Nombre = "N", Descripcion = "", Marca = "", CodigoBarra = "", UnidadMedida = "U",
                Estado = "Activo" 
            };
            context.Productos.Add(p);
            context.SaveChanges();

            var service = new ProductoService(context);

            // Act
            var res = await service.ChangeStatusAsync(p.Id, "Descontinuado");

            // Assert
            Assert.True(res);
            var pUpdated = context.Productos.Find(p.Id);
            Assert.Equal("Descontinuado", pUpdated.Estado);
        }
        
        [Fact]
        public async Task GetAllAsync_DebeRespetarFiltroExcluyendoEliminados()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.Add(new Categoria { Id = 1, Nombre = "Cat" });
            context.Productos.Add(new Producto { Id = 1, Sku = "1", Nombre = "A", CategoriaId = 1, Estado = "Activo", Descripcion = "", Marca = "", CodigoBarra = "", UnidadMedida = "" });
            context.Productos.Add(new Producto { Id = 2, Sku = "2", Nombre = "B", CategoriaId = 1, Estado = "Eliminado", Descripcion = "", Marca = "", CodigoBarra = "", UnidadMedida = "" });
            context.SaveChanges();

            var service = new ProductoService(context);

            // Act
            var res = await service.GetAllAsync();

            // Assert
            Assert.Single(res);
            Assert.Equal("A", res.First().Nombre);
        }
    }
}
