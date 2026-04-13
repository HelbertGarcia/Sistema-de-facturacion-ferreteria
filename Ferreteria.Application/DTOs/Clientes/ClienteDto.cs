using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.DTOs.Clientes
{
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre o razón social es requerido.")]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es requerido.")]
        public string TipoIdentificacion { get; set; } = string.Empty; // Cédula o RNC

        [Required(ErrorMessage = "El número de identificación es requerido.")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "La cédula debe tener 11 dígitos y el RNC 9 dígitos.")]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;
        
        [Phone]
        public string Telefono { get; set; } = string.Empty;
        
        public string Estado { get; set; } = "Activo";
    }
}
