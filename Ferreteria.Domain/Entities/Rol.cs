using Ferreteria.Domain.Common;
using System.Collections.Generic;

namespace Ferreteria.Domain.Entities
{
    public class Rol : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
