using Ferreteria.Application.DTOs.Productos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync(string search = "", int? categoriaId = null);
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(ProductoDto dto);
        Task<bool> UpdateAsync(ProductoDto dto);
        Task<bool> ChangeStatusAsync(int id, string estatus);
    }
}
