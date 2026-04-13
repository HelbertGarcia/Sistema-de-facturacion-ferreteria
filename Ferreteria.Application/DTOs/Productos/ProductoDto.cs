using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.DTOs.Productos
{
    public class ProductoDto
    {
        public int Id { get; set; }

        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = string.Empty;
        
        public string Descripcion { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string CodigoBarra { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;

        [Required]
        public decimal PrecioCosto { get; set; }
        
        [Required]
        public decimal MargenGanancia { get; set; } // Percentage

        public decimal PrecioVenta { get; set; }
        
        public bool TieneItbis { get; set; }
        public decimal Itbis { get; set; } // Usually 18%

        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        public string UnidadMedida { get; set; } = string.Empty;

        public int StockActual { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        public string ImagenUrl { get; set; } = string.Empty;
        
        public string Estado { get; set; } = "Activo";
        
        public bool RequiereReposicion => StockActual <= StockMinimo;
    }
}
