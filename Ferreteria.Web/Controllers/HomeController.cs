using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Ferreteria.Application.Interfaces.IFacturacionService _facturacionService;

        public HomeController(Ferreteria.Application.Interfaces.IFacturacionService facturacionService)
        {
            _facturacionService = facturacionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Reportes(DateTime? fecha = null)
        {
            DateTime filterDate = fecha ?? DateTime.UtcNow.ToLocalTime().Date;
            ViewBag.FechaReporte = filterDate;
            
            var reporte = await _facturacionService.ObtenerReporteCajaAsync(filterDate);
            return View(reporte);
        }
    }
}
