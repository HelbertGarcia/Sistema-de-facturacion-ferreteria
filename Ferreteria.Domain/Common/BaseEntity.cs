using System;

namespace Ferreteria.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo";
    }
}
