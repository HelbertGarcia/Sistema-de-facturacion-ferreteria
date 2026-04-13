using Ferreteria.Domain.Common;

namespace Ferreteria.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string TipoIdentificacion { get; set; } = string.Empty; // Cédula o RNC
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
