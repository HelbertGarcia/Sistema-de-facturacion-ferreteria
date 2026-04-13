using Ferreteria.Domain.Entities;

namespace Ferreteria.Application.DTOs.Auth
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }
    }
}
