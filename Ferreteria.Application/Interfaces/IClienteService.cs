using Ferreteria.Application.DTOs.Clientes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllAsync(string search = "");
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(ClienteDto clienteDto);
        Task<bool> UpdateAsync(ClienteDto clienteDto);
        Task<bool> DeleteLogicalAsync(int id);
        Task<bool> ExistsIdentificacionAsync(string numeroIdentificacion, int excludeId = 0);
    }
}
