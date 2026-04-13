using Ferreteria.Application.DTOs.Clientes;
using Ferreteria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ferreteria.Web.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            ViewData["Search"] = search;
            var clientes = await _clienteService.GetAllAsync(search);
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var success = await _clienteService.AddAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("NumeroIdentificacion", "Ya existe un cliente con esta identificación.");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null || cliente.NumeroIdentificacion == "00000000000") // Cannot edit final consumer
                return RedirectToAction(nameof(Index));

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClienteDto dto)
        {
            // Simple check
            if (dto.NumeroIdentificacion == "00000000000")
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _clienteService.UpdateAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("NumeroIdentificacion", "La identificación ya existe o no se pudo actualizar.");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteService.DeleteLogicalAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
