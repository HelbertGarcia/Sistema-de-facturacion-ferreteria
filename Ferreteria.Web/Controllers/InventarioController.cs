using Ferreteria.Application.DTOs.Inventario;
using Ferreteria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        private readonly IInventarioService _inventarioService;
        private readonly IProductoService _productoService;

        public InventarioController(IInventarioService inventarioService, IProductoService productoService)
        {
            _inventarioService = inventarioService;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index(int? productoId = null)
        {
            var movimientos = await _inventarioService.ObtenerKardexAsync(productoId);
            
            var productos = await _productoService.GetAllAsync();
            ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre", productoId);
            
            ViewBag.DashboardInfo = await _inventarioService.ObtenerDashboardInvetarioAsync();

            return View(movimientos);
        }

        [HttpGet]
        public async Task<IActionResult> Entrada()
        {
            var productos = await _productoService.GetAllAsync();
            ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre");
            return View(new MovimientoDto { TipoMovimiento = "Entrada", Motivo = "Compra" });
        }

        [HttpPost]
        public async Task<IActionResult> Entrada(MovimientoDto dto)
        {
            if (dto.ProductoId <= 0)
                ModelState.AddModelError("ProductoId", "Debe seleccionar un producto.");

            if (!ModelState.IsValid)
            {
                var productos = await _productoService.GetAllAsync();
                ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre", dto.ProductoId);
                return View(dto);
            }

            dto.TipoMovimiento = "Entrada";
            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            var result = await _inventarioService.RegistrarEntradaAsync(dto, usuario);
            if (!result)
            {
                ModelState.AddModelError("", "Error al registrar la entrada. Verifique los datos o si el producto existe.");
                var productos = await _productoService.GetAllAsync();
                ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre", dto.ProductoId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Ajuste()
        {
            var productos = await _productoService.GetAllAsync();
            ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre");
            return View(new MovimientoDto { TipoMovimiento = "Salida", Motivo = "Ajuste / Merma" });
        }

        [HttpPost]
        public async Task<IActionResult> Ajuste(MovimientoDto dto)
        {
             if (dto.ProductoId <= 0)
                ModelState.AddModelError("ProductoId", "Debe seleccionar un producto.");

            if (!ModelState.IsValid)
            {
                var productos = await _productoService.GetAllAsync();
                ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre", dto.ProductoId);
                return View(dto);
            }

            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            var result = await _inventarioService.RegistrarAjusteAsync(dto, usuario);
            if (!result)
            {
                ModelState.AddModelError("", "Error al registrar el ajuste. Stock insuficiente o producto inválido.");
                var productos = await _productoService.GetAllAsync();
                ViewBag.ProductosList = new SelectList(productos, "Id", "Nombre", dto.ProductoId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
