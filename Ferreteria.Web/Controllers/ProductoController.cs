using Ferreteria.Application.DTOs.Productos;
using Ferreteria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IWebHostEnvironment _env;

        public ProductoController(IProductoService productoService, ICategoriaService categoriaService, IWebHostEnvironment env)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _env = env;
        }

        public async Task<IActionResult> Index(string search = "", int? categoriaId = null)
        {
            ViewData["Search"] = search;
            ViewData["CategoriaId"] = categoriaId;

            var categorias = await _categoriaService.GetAllAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);

            var productos = await _productoService.GetAllAsync(search, categoriaId);
            return View(productos);
        }

        public async Task<IActionResult> Categorias()
        {
            var cats = await _categoriaService.GetAllAsync();
            return View(cats);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCategoria(CategoriaDto dto)
        {
            if (dto.Id == 0)
                await _categoriaService.AddAsync(dto);
            else
                await _categoriaService.UpdateAsync(dto);
                
            return RedirectToAction(nameof(Categorias));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriasViewBag();
            return View(new ProductoDto { TieneItbis = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoDto dto, Microsoft.AspNetCore.Http.IFormFile? ImagenUpload)
        {
            ClearImplicitValidationErrors();
            
            if (!ModelState.IsValid)
            {
                await PopulateCategoriasViewBag(dto.CategoriaId);
                return View(dto);
            }

            if (ImagenUpload != null && ImagenUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "productos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImagenUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenUpload.CopyToAsync(fileStream);
                }
                dto.ImagenUrl = "/images/productos/" + uniqueFileName;
            }

            await _productoService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _productoService.GetByIdAsync(id);
            if (p == null) return RedirectToAction(nameof(Index));

            await PopulateCategoriasViewBag(p.CategoriaId);
            return View("Create", p); // Reuse the create view for editing
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoDto dto, Microsoft.AspNetCore.Http.IFormFile? ImagenUpload)
        {
            ClearImplicitValidationErrors();
            
            if (!ModelState.IsValid)
            {
                await PopulateCategoriasViewBag(dto.CategoriaId);
                return View("Create", dto);
            }

            if (ImagenUpload != null && ImagenUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "productos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImagenUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenUpload.CopyToAsync(fileStream);
                }
                dto.ImagenUrl = "/images/productos/" + uniqueFileName;
            }

            await _productoService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Descontinuar(int id)
        {
            await _productoService.ChangeStatusAsync(id, "Descontinuado");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            await _productoService.ChangeStatusAsync(id, "Activo");
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCategoriasViewBag(int selectedId = 0)
        {
            var categorias = await _categoriaService.GetAllAsync();
            ViewBag.CategoriasList = new SelectList(categorias, "Id", "Nombre", selectedId);
        }

        private void ClearImplicitValidationErrors()
        {
            ModelState.Remove(nameof(ProductoDto.PrecioVenta));
            ModelState.Remove(nameof(ProductoDto.Itbis));
            ModelState.Remove(nameof(ProductoDto.StockActual));
        }
    }
}
