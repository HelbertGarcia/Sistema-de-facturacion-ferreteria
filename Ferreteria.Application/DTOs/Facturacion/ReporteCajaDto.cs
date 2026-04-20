namespace Ferreteria.Application.DTOs.Facturacion
{
    public class ReporteCajaDto
    {
        public decimal TotalBruto { get; set; }
        public decimal TotalItbis { get; set; }
        public decimal TotalVendido { get; set; }
        public int CantidadFacturas { get; set; }
        public int CantidadAnuladas { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
        public System.Collections.Generic.List<VistaFacturaDto> Transacciones { get; set; } = new System.Collections.Generic.List<VistaFacturaDto>();
    }
}
