using Ferreteria.Application.DTOs.Clientes;
using Ferreteria.Application.Interfaces;
using Ferreteria.Domain.Entities;
using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Infrastructure.Services
{
    public class ClienteService : IClienteService
    {
        private readonly FerreteriaDbContext _context;

        public ClienteService(FerreteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllAsync(string search = "")
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.NombreRazonSocial.Contains(search) || 
                                         c.NumeroIdentificacion.Contains(search));
            }

            return await query.OrderBy(x => x.NombreRazonSocial)
                              .Select(c => new ClienteDto
                              {
                                  Id = c.Id,
                                  NombreRazonSocial = c.NombreRazonSocial,
                                  TipoIdentificacion = c.TipoIdentificacion,
                                  NumeroIdentificacion = c.NumeroIdentificacion,
                                  Direccion = c.Direccion,
                                  Telefono = c.Telefono,
                                  Estado = c.Estado
                              }).ToListAsync();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDto
            {
                Id = c.Id,
                NombreRazonSocial = c.NombreRazonSocial,
                TipoIdentificacion = c.TipoIdentificacion,
                NumeroIdentificacion = c.NumeroIdentificacion,
                Direccion = c.Direccion,
                Telefono = c.Telefono,
                Estado = c.Estado
            };
        }

        public async Task<bool> AddAsync(ClienteDto dto)
        {
            if (await ExistsIdentificacionAsync(dto.NumeroIdentificacion))
                return false;

            var cliente = new Cliente
            {
                NombreRazonSocial = dto.NombreRazonSocial,
                TipoIdentificacion = dto.TipoIdentificacion,
                NumeroIdentificacion = dto.NumeroIdentificacion,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Estado = "Activo"
            };

            _context.Clientes.Add(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.Id);
            if (cliente == null) return false;

            if (cliente.NumeroIdentificacion != dto.NumeroIdentificacion && 
                await ExistsIdentificacionAsync(dto.NumeroIdentificacion, dto.Id))
                return false;

            cliente.NombreRazonSocial = dto.NombreRazonSocial;
            cliente.TipoIdentificacion = dto.TipoIdentificacion;
            cliente.NumeroIdentificacion = dto.NumeroIdentificacion;
            cliente.Direccion = dto.Direccion;
            cliente.Telefono = dto.Telefono;
            
            _context.Clientes.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLogicalAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            // Consumidor Final protection
            if (cliente.NumeroIdentificacion == "00000000000") return false;

            cliente.Estado = "Inactivo";
            _context.Clientes.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsIdentificacionAsync(string numeroIdentificacion, int excludeId = 0)
        {
            return await _context.Clientes.AnyAsync(c => c.NumeroIdentificacion == numeroIdentificacion && c.Id != excludeId);
        }

        public async Task<int> ObtenerNuevosClientesHoyAsync()
        {
            var hoy = System.DateTime.UtcNow.ToLocalTime().Date;
            return await _context.Clientes.CountAsync(c => c.FechaCreacion.Date == hoy);
        }
    }
}
