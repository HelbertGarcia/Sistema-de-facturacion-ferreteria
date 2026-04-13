using Ferreteria.Domain.Common;
using System;

namespace Ferreteria.Domain.Entities
{
    public class MovimientoInventario : BaseEntity
    {
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"
        public string Motivo { get; set; } = string.Empty; // "Compra", "Ajuste manual", "Venta", "Devolucion"
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public string ProveedorOReferencia { get; set; } = string.Empty;
    }
}
