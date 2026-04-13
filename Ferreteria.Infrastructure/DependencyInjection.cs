using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ferreteria.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FerreteriaDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(FerreteriaDbContext).Assembly.FullName)));

            services.AddScoped<Ferreteria.Application.Interfaces.IAuthService, Ferreteria.Infrastructure.Services.AuthService>();
            services.AddScoped<Ferreteria.Application.Interfaces.IClienteService, Ferreteria.Infrastructure.Services.ClienteService>();
            services.AddScoped<Ferreteria.Application.Interfaces.ICategoriaService, Ferreteria.Infrastructure.Services.CategoriaService>();
            services.AddScoped<Ferreteria.Application.Interfaces.IProductoService, Ferreteria.Infrastructure.Services.ProductoService>();
            services.AddScoped<Ferreteria.Application.Interfaces.IInventarioService, Ferreteria.Infrastructure.Services.InventarioService>();
            services.AddScoped<Ferreteria.Application.Interfaces.IFacturacionService, Ferreteria.Infrastructure.Services.FacturacionService>();

            return services;
        }
    }
}
