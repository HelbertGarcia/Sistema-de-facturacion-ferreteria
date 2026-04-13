using Ferreteria.Domain.Common;

namespace Ferreteria.Domain.Entities
{
    public class FacturaDetalle : BaseEntity
    {
        public int FacturaId { get; set; }
        public Factura Factura { get; set; }
        
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal ImpuestoPorcentaje { get; set; }
        public decimal ImpuestoMonto { get; set; }
        public decimal TotalLinea { get; set; }
    }
}
