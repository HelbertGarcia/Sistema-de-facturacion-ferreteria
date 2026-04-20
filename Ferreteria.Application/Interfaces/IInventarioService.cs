using Ferreteria.Application.DTOs.Inventario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface IInventarioService
    {
        Task<bool> RegistrarEntradaAsync(MovimientoDto dto, string usuario);
        Task<bool> RegistrarAjusteAsync(MovimientoDto dto, string usuario);
        Task<IEnumerable<MovimientoDto>> ObtenerKardexAsync(int? productoId = null);
        Task<DashboardInventarioDto> ObtenerDashboardInvetarioAsync();
        Task<ReporteValorizacionDto> ObtenerReporteValorizacionAsync(int? categoriaId = null);
    }
}
