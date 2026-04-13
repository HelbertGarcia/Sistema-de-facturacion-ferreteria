using Ferreteria.Domain.Common;
using System.Collections.Generic;

namespace Ferreteria.Domain.Entities
{
    public class Factura : BaseEntity
    {
        public string NumeroFactura { get; set; } = string.Empty;
        
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        
        public int VendedorId { get; set; }
        public Usuario Vendedor { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestosTotal { get; set; }
        public decimal TotalGeneral { get; set; }
        
        public DateTime FechaEmision { get; set; }
        
        public string MetodoPago { get; set; } = string.Empty; // "Efectivo", "Tarjeta", "Transferencia"
        public decimal MontoRecibido { get; set; }
        public decimal Cambio { get; set; }

        public string MotivoAnulacion { get; set; } = string.Empty;

        public ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
    }
}
