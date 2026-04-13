using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.DTOs.Productos
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        public string Nombre { get; set; } = string.Empty;
        
        public int CantidadProductos { get; set; }
    }
}
