using Ferreteria.Application.DTOs.Productos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetAllAsync();
        Task<CategoriaDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(CategoriaDto dto);
        Task<bool> UpdateAsync(CategoriaDto dto);
        Task<bool> HasProductsAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
