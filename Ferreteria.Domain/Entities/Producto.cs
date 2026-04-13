using Ferreteria.Domain.Common;

namespace Ferreteria.Domain.Entities
{
    public class Producto : BaseEntity
    {
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string CodigoBarra { get; set; } = string.Empty;
        
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        
        public decimal PrecioCosto { get; set; }
        public decimal MargenGanancia { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Itbis { get; set; } // % o monto de ITBIS
        public bool TieneItbis { get; set; }

        public string UnidadMedida { get; set; } = string.Empty;
        
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
