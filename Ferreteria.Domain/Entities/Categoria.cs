using Ferreteria.Domain.Common;
using System.Collections.Generic;

namespace Ferreteria.Domain.Entities
{
    public class Categoria : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
