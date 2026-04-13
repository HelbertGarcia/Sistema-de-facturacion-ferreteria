using Ferreteria.Application.DTOs.Facturacion;
using Ferreteria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class FacturaController : Controller
    {
        private readonly IFacturacionService _facturacionService;
        private readonly IProductoService _productoService;
        private readonly IClienteService _clienteService;

        public FacturaController(IFacturacionService facturacionService, IProductoService productoService, IClienteService clienteService)
        {
            _facturacionService = facturacionService;
            _productoService = productoService;
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            ViewData["Search"] = search;
            var historial = await _facturacionService.ObtenerHistorialAsync(search);
            return View(historial);
        }

        [HttpGet]
        public async Task<IActionResult> Pos()
        {
            // Cargar datos estáticos iniciales para el JS Interfaz (POS)
            ViewBag.Clientes = await _clienteService.GetAllAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoApi(string q)
        {
            if (string.IsNullOrEmpty(q)) return Json(new object[] { });

            var productos = await _productoService.GetAllAsync(q);
            var results = productos
                .Where(p => p.Estado == "Activo" && p.StockActual > 0)
                .Select(p => new
                {
                    id = p.Id,
                    sku = p.Sku,
                    nombre = p.Nombre,
                    precioVenta = p.PrecioVenta,
                    tieneItbis = p.TieneItbis,
                    stock = p.StockActual
                }).Take(10);

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarVentaApi([FromBody] NuevaFacturaDto dto)
        {
            if (dto == null || dto.ClienteId <= 0 || !dto.Detalles.Any())
            {
                return BadRequest(new { success = false, message = "Datos de factura inválidos." });
            }

            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out int userId);

                var factura = await _facturacionService.ProcesarFacturaAsync(dto, userId > 0 ? userId : 1);
                
                return Ok(new { success = true, facturaId = factura.Id, numeroFactura = factura.NumeroFactura });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var f = await _facturacionService.ObtenerFacturaPorIdAsync(id);
            if (f == null) return NotFound();

            return View(f);
        }

        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            var user = User.Identity?.Name ?? "Admin";
            var success = await _facturacionService.AnularFacturaAsync(id, user);

            if (!success)
            {
                TempData["Error"] = "No se pudo anular la factura. Verifique su estado.";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}
