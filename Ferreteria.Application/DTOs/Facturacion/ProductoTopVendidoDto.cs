namespace Ferreteria.Application.DTOs.Facturacion
{
    public class ProductoTopVendidoDto
    {
        public int ProductoId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal MontoGenerado { get; set; }
    }
}
