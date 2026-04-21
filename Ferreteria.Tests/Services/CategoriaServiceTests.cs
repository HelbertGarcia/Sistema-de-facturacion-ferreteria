using Ferreteria.Application.DTOs.Productos;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class CategoriaServiceTests
    {
        [Fact]
        public async Task AddAsync_DebeCrearCategoria()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var service = new CategoriaService(context);
            var dto = new CategoriaDto { Nombre = "Herramientas" };

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            Assert.True(result);
            var cat = context.Categorias.FirstOrDefault();
            Assert.NotNull(cat);
            Assert.Equal("Herramientas", cat.Nombre);
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarOrdenadoAlfabeticamente()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Categorias.AddRange(
                new Categoria { Nombre = "Zapatos" },
                new Categoria { Nombre = "Cables" },
                new Categoria { Nombre = "Alicates" }
            );
            context.SaveChanges();

            var service = new CategoriaService(context);

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.Equal(3, list.Count());
            Assert.Equal("Alicates", list.ElementAt(0).Nombre);
            Assert.Equal("Zapatos", list.ElementAt(2).Nombre);
        }

        [Fact]
        public async Task DeleteAsync_ConProductos_DebeFallarYNoBorrar()
        {
            // Arrange
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var cat = new Categoria { Nombre = "Herramientas" };
            context.Categorias.Add(cat);
            context.SaveChanges();

            context.Productos.Add(new Producto 
            { 
                Nombre = "Martillo", 
                CategoriaId = cat.Id, 
                Estado = "Activo",
                Sku = "MAR-1",
                Descripcion = "D",
                Marca = "M",
                CodigoBarra = "123",
                UnidadMedida = "Pieza"
            });
            context.SaveChanges();

            var service = new CategoriaService(context);

            // Act
            var result = await service.DeleteAsync(cat.Id);

            // Assert
            Assert.False(result); // Cannot delete because it has active products
            Assert.NotNull(context.Categorias.Find(cat.Id)); 
        }
    }
}
