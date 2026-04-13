using Ferreteria.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace Ferreteria.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginDto dto);
    }
}
