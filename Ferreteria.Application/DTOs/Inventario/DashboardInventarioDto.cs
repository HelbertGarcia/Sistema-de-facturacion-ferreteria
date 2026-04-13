namespace Ferreteria.Application.DTOs.Inventario
{
    public class DashboardInventarioDto
    {
        public decimal TotalValor { get; set; }
        public int TotalProductos { get; set; }
        public int ProductosStockBajo { get; set; }
        public int ProductosAgotados { get; set; }
    }
}
