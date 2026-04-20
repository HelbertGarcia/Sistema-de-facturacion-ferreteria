using System;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.DTOs.Inventario
{
    public class MovimientoDto
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSku { get; set; } = string.Empty;

        public string TipoMovimiento { get; set; } = string.Empty; // Entrada, Salida
        
        [Required(ErrorMessage = "Debe especificar un motivo.")]
        public string Motivo { get; set; } = string.Empty; // Compra, Ajuste, Venta, etc.
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        public decimal CostoUnitario { get; set; }
        public string? ProveedorOReferencia { get; set; }
    }
}
