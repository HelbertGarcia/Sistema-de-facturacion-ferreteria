using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Ferreteria.Application.Interfaces.IFacturacionService _facturacionService;
        private readonly Ferreteria.Application.Interfaces.IClienteService _clienteService;
        private readonly Ferreteria.Application.Interfaces.IInventarioService _inventarioService;
        private readonly Ferreteria.Application.Interfaces.ICategoriaService _categoriaService;

        public HomeController(
            Ferreteria.Application.Interfaces.IFacturacionService facturacionService,
            Ferreteria.Application.Interfaces.IClienteService clienteService,
            Ferreteria.Application.Interfaces.IInventarioService inventarioService,
            Ferreteria.Application.Interfaces.ICategoriaService categoriaService)
        {
            _facturacionService = facturacionService;
            _clienteService = clienteService;
            _inventarioService = inventarioService;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            // Ventas de Hoy
            var reporte = await _facturacionService.ObtenerReporteCajaAsync(DateTime.UtcNow.ToLocalTime().Date);
            ViewBag.VentasDeHoy = reporte.TotalVendido;

            // Clientes Nuevos
            ViewBag.ClientesNuevos = await _clienteService.ObtenerNuevosClientesHoyAsync();

            // Alertas de Stock
            var invDashboard = await _inventarioService.ObtenerDashboardInvetarioAsync();
            ViewBag.AlertasStock = invDashboard.ProductosStockBajo + invDashboard.ProductosAgotados;

            // Tendencias y Distribucion
            ViewBag.Tendencias = await _facturacionService.ObtenerTendenciaVentasUltimaSemanaAsync();

            return View();
        }

        public async Task<IActionResult> Reportes(DateTime? fecha = null)
        {
            DateTime filterDate = fecha ?? DateTime.UtcNow.ToLocalTime().Date;
            ViewBag.FechaReporte = filterDate;
            
            var reporte = await _facturacionService.ObtenerReporteCajaAsync(filterDate);
            return View(reporte);
        }

        public async Task<IActionResult> TopSellers(int limite = 10, DateTime? desde = null, DateTime? hasta = null)
        {
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Limite = limite;
            
            var top = await _facturacionService.ObtenerTopProductosVendidosAsync(limite, desde, hasta);
            return View(top);
        }

        public async Task<IActionResult> InventarioValorizado(int? categoriaId = null)
        {
            ViewBag.CategoriaId = categoriaId;
            ViewBag.Categorias = await _categoriaService.GetAllAsync();
            var reporte = await _inventarioService.ObtenerReporteValorizacionAsync(categoriaId);
            return View(reporte);
        }
    }
}
