using Ferreteria.Application.DTOs.Facturacion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface IFacturacionService
    {
        Task<VistaFacturaDto> ProcesarFacturaAsync(NuevaFacturaDto facturaDto, int vendedorId);
        Task<VistaFacturaDto?> ObtenerFacturaPorIdAsync(int id);
        Task<IEnumerable<VistaFacturaDto>> ObtenerHistorialAsync(string search = "");
        Task<bool> AnularFacturaAsync(int id, string adminUser);
        Task<ReporteCajaDto> ObtenerReporteCajaAsync(DateTime fechaVenta);
    }
}
