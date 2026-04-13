namespace Ferreteria.Application.DTOs.Facturacion
{
    public class ReporteCajaDto
    {
        public decimal TotalVendido { get; set; }
        public int CantidadFacturas { get; set; }
        public int CantidadAnuladas { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
    }
}
