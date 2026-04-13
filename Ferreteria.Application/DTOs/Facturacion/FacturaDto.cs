using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.DTOs.Facturacion
{
    public class FacturaDetalleDto
    {
        public int ProductoId { get; set; }
        public string ProductoSku { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal Descuento { get; set; }
        public decimal ImpuestoMonto { get; set; }
        public decimal TotalNeto => Subtotal - Descuento + ImpuestoMonto;
    }

    public class NuevaFacturaDto
    {
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public string MetodoPago { get; set; } = "Efectivo";

        public List<FacturaDetalleDto> Detalles { get; set; } = new List<FacturaDetalleDto>();
    }

    public class VistaFacturaDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteIdentificacion { get; set; } = string.Empty;
        public string VendedorNombre { get; set; } = string.Empty;
        
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestosTotal { get; set; }
        public decimal TotalGeneral { get; set; }
        
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public List<FacturaDetalleDto> Detalles { get; set; } = new List<FacturaDetalleDto>();
    }
}
