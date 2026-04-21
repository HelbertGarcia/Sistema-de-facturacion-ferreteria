using Ferreteria.Application.DTOs.Clientes;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class ClienteServiceTests
    {
        [Fact]
        public async Task AddAsync_DebeCrearSiDniEsUnico()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var service = new ClienteService(context);

            var dto = new ClienteDto { NombreRazonSocial = "Test", NumeroIdentificacion = "123456" };

            // Act
            var res = await service.AddAsync(dto);

            // Assert
            Assert.True(res);
            Assert.Equal(1, context.Clientes.Count());
        }

        [Fact]
        public async Task AddAsync_DebeFallarSiDniYaExiste()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Clientes.Add(new Cliente { NombreRazonSocial = "A", NumeroIdentificacion = "123456" });
            context.SaveChanges();

            var service = new ClienteService(context);

            // Act
            var dto = new ClienteDto { NombreRazonSocial = "B", NumeroIdentificacion = "123456" };
            
            // Assert expecting false since the method returns a boolean instead of throwing an exception
            var result = await service.AddAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteLogicalAsync_NoDebeEliminarConsumidorFinal()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Clientes.Add(new Cliente { Id = 1, NombreRazonSocial = "Consumidor Final", NumeroIdentificacion = "00000000000", Estado = "Activo" });
            context.SaveChanges();

            var service = new ClienteService(context);
            
            // Act
            var res = await service.DeleteLogicalAsync(1);

            // Assert
            Assert.False(res); // Should return false
            Assert.Equal("Activo", context.Clientes.Find(1).Estado); // State should not change
        }
    }
}
