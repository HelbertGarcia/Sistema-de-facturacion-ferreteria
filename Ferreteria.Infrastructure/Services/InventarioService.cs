using Ferreteria.Application.DTOs.Inventario;
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
    public class InventarioService : IInventarioService
    {
        private readonly FerreteriaDbContext _context;

        public InventarioService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegistrarEntradaAsync(MovimientoDto dto, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(dto.ProductoId);
                if (producto == null) return false;

                // Update product stock and update latest cost
                producto.StockActual += dto.Cantidad;
                producto.PrecioCosto = dto.CostoUnitario;
                
                // Recalculate selling price just in case cost changed
                producto.PrecioVenta = producto.PrecioCosto + (producto.PrecioCosto * (producto.MargenGanancia / 100));

                _context.Productos.Update(producto);

                var movimiento = new MovimientoInventario
                {
                    ProductoId = dto.ProductoId,
                    TipoMovimiento = "Entrada",
                    Motivo = "Compra",
                    Cantidad = dto.Cantidad,
                    CostoUnitario = dto.CostoUnitario,
                    ProveedorOReferencia = dto.ProveedorOReferencia,
                    UsuarioCreacion = usuario,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.MovimientosInventario.Add(movimiento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> RegistrarAjusteAsync(MovimientoDto dto, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(dto.ProductoId);
                if (producto == null) return false;

                if (dto.TipoMovimiento == "Salida" && producto.StockActual < dto.Cantidad)
                {
                    return false; // Can't adjust below 0 here for simplicity
                }

                if (dto.TipoMovimiento == "Entrada")
                    producto.StockActual += dto.Cantidad;
                else
                    producto.StockActual -= dto.Cantidad;

                _context.Productos.Update(producto);

                var movimiento = new MovimientoInventario
                {
                    ProductoId = dto.ProductoId,
                    TipoMovimiento = dto.TipoMovimiento,
                    Motivo = dto.Motivo,
                    Cantidad = dto.Cantidad,
                    CostoUnitario = producto.PrecioCosto,
                    ProveedorOReferencia = dto.ProveedorOReferencia,
                    UsuarioCreacion = usuario,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.MovimientosInventario.Add(movimiento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<MovimientoDto>> ObtenerKardexAsync(int? productoId = null)
        {
            var query = _context.MovimientosInventario.Include(m => m.Producto).AsQueryable();

            if (productoId.HasValue && productoId.Value > 0)
            {
                query = query.Where(m => m.ProductoId == productoId.Value);
            }

            return await query.OrderByDescending(m => m.FechaCreacion)
                .Select(m => new MovimientoDto
                {
                    Id = m.Id,
                    FechaCreacion = m.FechaCreacion,
                    UsuarioCreacion = m.UsuarioCreacion,
                    ProductoId = m.ProductoId,
                    ProductoNombre = m.Producto.Nombre,
                    ProductoSku = m.Producto.Sku,
                    TipoMovimiento = m.TipoMovimiento,
                    Motivo = m.Motivo,
                    Cantidad = m.Cantidad,
                    CostoUnitario = m.CostoUnitario,
                    ProveedorOReferencia = m.ProveedorOReferencia
                })
                .ToListAsync();
        }

        public async Task<DashboardInventarioDto> ObtenerDashboardInvetarioAsync()
        {
            var totalProductos = await _context.Productos.CountAsync(x => x.Estado == "Activo");
            var valorTotal = await _context.Productos.Where(x => x.Estado == "Activo")
                                    .SumAsync(x => x.StockActual * x.PrecioCosto);
                                    
            var stockBajo = await _context.Productos
                                    .CountAsync(p => p.Estado == "Activo" && p.StockActual <= p.StockMinimo && p.StockActual > 0);
                                    
            var agotados = await _context.Productos
                                    .CountAsync(p => p.Estado == "Activo" && p.StockActual <= 0);

            return new DashboardInventarioDto
            {
                TotalValor = valorTotal,
                TotalProductos = totalProductos,
                ProductosStockBajo = stockBajo,
                ProductosAgotados = agotados
            };
        }

        public async Task<ReporteValorizacionDto> ObtenerReporteValorizacionAsync(int? categoriaId = null)
        {
            var query = _context.Productos.Include(p => p.Categoria).Where(p => p.Estado == "Activo").AsQueryable();

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            var limitDate = DateTime.UtcNow.Date.AddDays(-60);

            var productosData = await query.Select(p => new
            {
                p.Id,
                p.Nombre,
                Categoria = p.Categoria.Nombre,
                p.StockActual,
                p.PrecioCosto,
                p.PrecioVenta,
                UltimaVenta = _context.FacturaDetalles
                    .Where(fd => fd.ProductoId == p.Id && fd.Factura.Estado == "Completada")
                    .OrderByDescending(fd => fd.Factura.FechaEmision)
                    .Select(fd => (DateTime?)fd.Factura.FechaEmision)
                    .FirstOrDefault()
            }).ToListAsync();

            var result = new ReporteValorizacionDto();

            foreach (var p in productosData)
            {
                bool estancado = p.StockActual > 0 && 
                    (!p.UltimaVenta.HasValue || p.UltimaVenta.Value.Date <= limitDate);

                var dto = new ProductoValorizadoDto
                {
                    ProductoId = p.Id,
                    ProductoNombre = p.Nombre,
                    CategoriaNombre = p.Categoria,
                    StockActual = p.StockActual,
                    CostoUnitario = p.PrecioCosto,
                    PrecioVenta = p.PrecioVenta,
                    EstaInactivo = estancado
                };

                result.Productos.Add(dto);
            }

            result.TotalCostoInvertido = result.Productos.Sum(x => x.CostoTotal);
            result.TotalValorVenta = result.Productos.Sum(x => x.ValorVentaTotal);
            result.TotalGananciaPotencial = result.TotalValorVenta - result.TotalCostoInvertido;

            return result;
        }
    }
}
