using Ferreteria.Application.DTOs.Facturacion;
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
    public class FacturacionService : IFacturacionService
    {
        private readonly FerreteriaDbContext _context;

        public FacturacionService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<VistaFacturaDto> ProcesarFacturaAsync(NuevaFacturaDto facturaDto, int vendedorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ahora = DateTime.UtcNow;

                // Generar No. Factura (Ordinario interno)
                var lastFactura = await _context.Facturas.OrderByDescending(f => f.Id).FirstOrDefaultAsync();
                int nextId = (lastFactura?.Id ?? 0) + 1;
                string numeroFactura = $"FAC-{ahora.Year}{ahora.Month:D2}-{nextId:D6}";

                var cliente = await _context.Clientes.FindAsync(facturaDto.ClienteId);
                var vendedor = await _context.Usuarios.FindAsync(vendedorId);

                var factura = new Factura
                {
                    NumeroFactura = numeroFactura,
                    ClienteId = facturaDto.ClienteId,
                    VendedorId = vendedorId,
                    MetodoPago = facturaDto.MetodoPago,
                    Subtotal = 0,
                    DescuentoTotal = 0,
                    ImpuestosTotal = 0,
                    TotalGeneral = 0,
                    FechaEmision = ahora,
                    UsuarioCreacion = vendedor?.Username ?? "Sistema",
                    FechaCreacion = ahora,
                    Estado = "Completada"
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync(); // Para obtener el Id de Factura

                foreach (var det in facturaDto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(det.ProductoId);
                    if (producto == null || producto.StockActual < det.Cantidad)
                    {
                        throw new Exception($"Stock insuficiente o inválido para producto: {producto?.Nombre ?? det.ProductoId.ToString()}");
                    }

                    // Reducir Stock
                    producto.StockActual -= det.Cantidad;
                    _context.Productos.Update(producto);

                    // Registrar movimiento de salida
                    _context.MovimientosInventario.Add(new MovimientoInventario
                    {
                        ProductoId = producto.Id,
                        TipoMovimiento = "Salida",
                        Motivo = "Venta Factura " + numeroFactura,
                        Cantidad = det.Cantidad,
                        CostoUnitario = producto.PrecioCosto,
                        ProveedorOReferencia = "Factura Nro " + numeroFactura,
                        UsuarioCreacion = vendedor?.Username ?? "Sistema",
                        FechaCreacion = ahora
                    });

                    // Añadir detalle de factura (Validamos cálculos del lado del servidor para seguridad)
                    decimal precio = producto.PrecioVenta;
                    decimal desc = det.Descuento;
                    decimal sub = precio * det.Cantidad;
                    decimal impuestoMonto = producto.TieneItbis ? ((sub - desc) * 0.18m) : 0;

                    var detalle = new FacturaDetalle
                    {
                        FacturaId = factura.Id,
                        ProductoId = producto.Id,
                        Cantidad = det.Cantidad,
                        PrecioUnitario = precio,
                        Subtotal = sub,
                        Descuento = desc,
                        ImpuestoPorcentaje = producto.TieneItbis ? 18 : 0,
                        ImpuestoMonto = impuestoMonto,
                        TotalLinea = sub - desc + impuestoMonto,
                        UsuarioCreacion = vendedor?.Username ?? "Sistema",
                        FechaCreacion = ahora
                    };

                    _context.FacturaDetalles.Add(detalle);

                    factura.Subtotal += detalle.Subtotal;
                    factura.DescuentoTotal += detalle.Descuento;
                    factura.ImpuestosTotal += detalle.ImpuestoMonto;
                }

                factura.TotalGeneral = factura.Subtotal - factura.DescuentoTotal + factura.ImpuestosTotal;
                _context.Facturas.Update(factura);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await ObtenerFacturaPorIdAsync(factura.Id) ?? throw new Exception("Error retornando factura");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VistaFacturaDto?> ObtenerFacturaPorIdAsync(int id)
        {
            var f = await _context.Facturas
                .Include(x => x.Cliente)
                .Include(x => x.Vendedor)
                .Include(x => x.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (f == null) return null;

            return MapToVistaFactura(f);
        }

        public async Task<IEnumerable<VistaFacturaDto>> ObtenerHistorialAsync(string search = "")
        {
            var query = _context.Facturas
                .Include(x => x.Cliente)
                .Include(x => x.Vendedor)
                .Include(x => x.Detalles).ThenInclude(d => d.Producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.NumeroFactura.Contains(search) || f.Cliente.NombreRazonSocial.Contains(search));
            }

            var resultados = await query.OrderByDescending(x => x.FechaEmision).Take(100).ToListAsync();
            return resultados.Select(MapToVistaFactura);
        }

        public async Task<bool> AnularFacturaAsync(int id, string adminUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var factura = await _context.Facturas
                    .Include(f => f.Detalles)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (factura == null || factura.Estado == "Anulada") return false;

                factura.Estado = "Anulada";
                _context.Facturas.Update(factura);

                // Devolver stock
                foreach (var det in factura.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(det.ProductoId);
                    if (producto != null)
                    {
                        producto.StockActual += det.Cantidad;
                        _context.Productos.Update(producto);

                        _context.MovimientosInventario.Add(new MovimientoInventario
                        {
                            ProductoId = producto.Id,
                            TipoMovimiento = "Entrada",
                            Motivo = "Anulación Factura " + factura.NumeroFactura,
                            Cantidad = det.Cantidad,
                            CostoUnitario = producto.PrecioCosto,
                            UsuarioCreacion = adminUser,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                }

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

        private VistaFacturaDto MapToVistaFactura(Factura f)
        {
            return new VistaFacturaDto
            {
                Id = f.Id,
                NumeroFactura = f.NumeroFactura,
                FechaEmision = f.FechaEmision,
                ClienteNombre = f.Cliente?.NombreRazonSocial ?? "Desconocido",
                ClienteIdentificacion = f.Cliente?.NumeroIdentificacion ?? "-",
                VendedorNombre = f.Vendedor?.NombreCompleto ?? "Sistema",
                Subtotal = f.Subtotal,
                DescuentoTotal = f.DescuentoTotal,
                ImpuestosTotal = f.ImpuestosTotal,
                TotalGeneral = f.TotalGeneral,
                MetodoPago = f.MetodoPago,
                Estado = f.Estado,
                Detalles = f.Detalles.Select(d => new FacturaDetalleDto
                {
                    ProductoId = d.ProductoId,
                    ProductoSku = d.Producto?.Sku ?? "-",
                    ProductoNombre = d.Producto?.Nombre ?? "N/A",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = d.Descuento,
                    ImpuestoMonto = d.ImpuestoMonto
                }).ToList()
            };
        }

        public async Task<ReporteCajaDto> ObtenerReporteCajaAsync(DateTime fechaVenta)
        {
            var facturasDia = await _context.Facturas
                .Where(f => f.FechaEmision.Date == fechaVenta.Date && f.Estado == "Completada")
                .ToListAsync();

            var anuladas = await _context.Facturas
                .Where(f => f.FechaEmision.Date == fechaVenta.Date && f.Estado == "Anulada")
                .CountAsync();

            return new ReporteCajaDto
            {
                TotalVendido = facturasDia.Sum(f => f.TotalGeneral),
                CantidadFacturas = facturasDia.Count,
                CantidadAnuladas = anuladas,
                TotalEfectivo = facturasDia.Where(f => f.MetodoPago == "Efectivo").Sum(f => f.TotalGeneral),
                TotalTarjeta = facturasDia.Where(f => f.MetodoPago == "Tarjeta").Sum(f => f.TotalGeneral),
                TotalTransferencia = facturasDia.Where(f => f.MetodoPago == "Transferencia").Sum(f => f.TotalGeneral)
            };
        }
    }
}
