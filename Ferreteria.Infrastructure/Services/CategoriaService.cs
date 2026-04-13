using Ferreteria.Application.DTOs.Productos;
using Ferreteria.Application.Interfaces;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Infrastructure.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly FerreteriaDbContext _context;

        public CategoriaService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            return await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    CantidadProductos = _context.Productos.Count(p => p.CategoriaId == c.Id && p.Estado == "Activo")
                })
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<CategoriaDto?> GetByIdAsync(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return null;

            return new CategoriaDto { Id = c.Id, Nombre = c.Nombre };
        }

        public async Task<bool> AddAsync(CategoriaDto dto)
        {
            _context.Categorias.Add(new Categoria { Nombre = dto.Nombre });
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(CategoriaDto dto)
        {
            var cat = await _context.Categorias.FindAsync(dto.Id);
            if (cat == null) return false;
            
            cat.Nombre = dto.Nombre;
            _context.Categorias.Update(cat);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.CategoriaId == id && p.Estado != "Eliminado");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await HasProductsAsync(id)) return false;

            var cat = await _context.Categorias.FindAsync(id);
            if (cat == null) return false;

            _context.Categorias.Remove(cat);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
