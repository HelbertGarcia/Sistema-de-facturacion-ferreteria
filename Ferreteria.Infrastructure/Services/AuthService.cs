using Ferreteria.Application.DTOs.Auth;
using Ferreteria.Application.Interfaces;
using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ferreteria.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly FerreteriaDbContext _context;

        public AuthService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos."
                };
            }

            if (user.Estado != "Activo")
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "El usuario se encuentra inactivo."
                };
            }

            return new LoginResultDto
            {
                Success = true,
                Usuario = user
            };
        }
    }
}
