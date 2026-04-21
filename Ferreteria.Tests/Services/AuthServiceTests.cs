using Ferreteria.Application.DTOs.Auth;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task AuthenticateAsync_DebeRetornarUsuarioSiCredencialesSonCorrectas()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Roles.Add(new Rol { Id = 1, Nombre = "Admin" });
            context.Usuarios.Add(new Usuario { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), NombreCompleto = "A", RolId = 1, Estado = "Activo" });
            context.SaveChanges();

            var service = new AuthService(context);

            // Act
            var dto = new LoginDto { Username = "admin", Password = "admin123" };
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Usuario);
            Assert.Equal("admin", result.Usuario.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_DebeRetornarNullSiContraseñaIncorrecta()
        {
            using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            context.Roles.Add(new Rol { Id = 1, Nombre = "Admin" });
            context.Usuarios.Add(new Usuario { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), NombreCompleto = "A", RolId = 1, Estado = "Activo" });
            context.SaveChanges();

            var service = new AuthService(context);

            // Act
            var dto = new LoginDto { Username = "admin", Password = "wrongpassword" };
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Usuario);
        }
    }
}
