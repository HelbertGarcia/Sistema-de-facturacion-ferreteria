using Ferreteria.Application.DTOs.Productos;
using Ferreteria.Application.Interfaces;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly FerreteriaDbContext _context;

        public ProductoService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync(string search = "", int? categoriaId = null)
        {
            var query = _context.Productos.Include(p => p.Categoria).Where(p => p.Estado != "Eliminado").AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nombre.Contains(search) || p.Sku.Contains(search));
            }

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            return await query.Select(p => new ProductoDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                CodigoBarra = p.CodigoBarra,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria.Nombre,
                PrecioCosto = p.PrecioCosto,
                MargenGanancia = p.MargenGanancia,
                PrecioVenta = p.PrecioVenta,
                TieneItbis = p.TieneItbis,
                Itbis = p.Itbis,
                UnidadMedida = p.UnidadMedida,
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo,
                ImagenUrl = p.ImagenUrl,
                Estado = p.Estado
            }).ToListAsync();
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var p = await _context.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;

            return new ProductoDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                CodigoBarra = p.CodigoBarra,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria.Nombre,
                PrecioCosto = p.PrecioCosto,
                MargenGanancia = p.MargenGanancia,
                PrecioVenta = p.PrecioVenta,
                TieneItbis = p.TieneItbis,
                Itbis = p.Itbis,
                UnidadMedida = p.UnidadMedida,
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo,
                ImagenUrl = p.ImagenUrl,
                Estado = p.Estado
            };
        }

        public async Task<bool> AddAsync(ProductoDto dto)
        {
            if (string.IsNullOrEmpty(dto.Sku))
            {
                dto.Sku = "PROD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            }

            // Calculation logic for Price
            dto.PrecioVenta = dto.PrecioCosto + (dto.PrecioCosto * (dto.MargenGanancia / 100));

            var producto = new Producto
            {
                Sku = dto.Sku,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion ?? string.Empty,
                Marca = dto.Marca ?? string.Empty,
                CodigoBarra = dto.CodigoBarra ?? string.Empty,
                CategoriaId = dto.CategoriaId,
                PrecioCosto = dto.PrecioCosto,
                MargenGanancia = dto.MargenGanancia,
                PrecioVenta = dto.PrecioVenta,
                TieneItbis = dto.TieneItbis,
                Itbis = dto.TieneItbis ? 18 : 0,
                UnidadMedida = dto.UnidadMedida,
                StockActual = 0, // Should be filled via inventory inputs
                StockMinimo = dto.StockMinimo,
                ImagenUrl = dto.ImagenUrl ?? string.Empty,
                Estado = "Activo"
            };

            _context.Productos.Add(producto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.Id);
            if (producto == null) return false;

            dto.PrecioVenta = dto.PrecioCosto + (dto.PrecioCosto * (dto.MargenGanancia / 100));

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion ?? string.Empty;
            producto.Marca = dto.Marca ?? string.Empty;
            producto.CodigoBarra = dto.CodigoBarra ?? string.Empty;
            producto.CategoriaId = dto.CategoriaId;
            producto.PrecioCosto = dto.PrecioCosto;
            producto.MargenGanancia = dto.MargenGanancia;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.TieneItbis = dto.TieneItbis;
            producto.Itbis = dto.TieneItbis ? 18 : 0;
            producto.UnidadMedida = dto.UnidadMedida;
            producto.StockMinimo = dto.StockMinimo;
            
            if (!string.IsNullOrEmpty(dto.ImagenUrl))
            {
                producto.ImagenUrl = dto.ImagenUrl;
            }

            _context.Productos.Update(producto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangeStatusAsync(int id, string estatus)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            producto.Estado = estatus;
            _context.Productos.Update(producto);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
