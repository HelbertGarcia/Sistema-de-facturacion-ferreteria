using Ferreteria.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ferreteria.Tests.Web
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FerreteriaDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<FerreteriaDbContext>(options =>
                {
                    options.UseInMemoryDatabase(System.Guid.NewGuid().ToString());
                });
            });
        }
    }

    public class WebTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public WebTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Useful to test 302 redirects
            });
        }

        [Fact]
        public async Task Get_Endpoints_DebeFuncionarCorrectamenteYRuteosProtegidos()
        {
            // ACT 1: Login Page
            var responseLogin = await _client.GetAsync("/Auth/Login");

            // ASSERT 1
            responseLogin.EnsureSuccessStatusCode(); 
            var html = await responseLogin.Content.ReadAsStringAsync();
            Assert.Contains("<!DOCTYPE html>", html);
            Assert.Contains("Iniciar Sesión", html);
            Assert.Contains("<form", html);

            // ACT 2: Protected Route
            var responseProtected = await _client.GetAsync("/Home/Index");

            // ASSERT 2
            Assert.Equal(System.Net.HttpStatusCode.Redirect, responseProtected.StatusCode);
            Assert.Contains("/Auth/Login", responseProtected.Headers.Location.OriginalString);
        }
    }
}
