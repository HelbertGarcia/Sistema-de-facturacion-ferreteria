using System.Collections.Generic;

namespace Ferreteria.Application.DTOs.Inventario
{
    public class ProductoValorizadoDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal PrecioVenta { get; set; }
        
        public decimal CostoTotal => StockActual * CostoUnitario;
        public decimal ValorVentaTotal => StockActual * PrecioVenta;
        public decimal GananciaPotencial => ValorVentaTotal - CostoTotal;
        public bool EstaInactivo { get; set; } // Más de 60 días sin ventas
    }

    public class ReporteValorizacionDto
    {
        public decimal TotalCostoInvertido { get; set; }
        public decimal TotalValorVenta { get; set; }
        public decimal TotalGananciaPotencial { get; set; }
        public List<ProductoValorizadoDto> Productos { get; set; } = new List<ProductoValorizadoDto>();
    }
}
