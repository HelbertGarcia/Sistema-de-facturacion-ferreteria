using Ferreteria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Ferreteria.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FerreteriaDbContext context)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }

            if (!context.Roles.Any())
            {
                var adminRole = new Rol { Nombre = "Admin" };
                var vendedorRole = new Rol { Nombre = "Vendedor" };
                context.Roles.AddRange(adminRole, vendedorRole);
                context.SaveChanges();

                // Admin User
                var adminUser = new Usuario
                {
                    NombreCompleto = "Administrador",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RolId = adminRole.Id
                };
                context.Usuarios.Add(adminUser);
                context.SaveChanges();
            }

            if (!context.Clientes.Any())
            {
                var consumidorFinal = new Cliente 
                { 
                    NombreRazonSocial = "Consumidor Final", 
                    TipoIdentificacion = "Cédula", 
                    NumeroIdentificacion = "00000000000",
                    Direccion = "N/A",
                    Telefono = "0000000000"
                };
                
                context.Clientes.Add(consumidorFinal);
                
                // Add 5 more test clients
                context.Clientes.AddRange(
                    new Cliente { NombreRazonSocial = "Juan Perez", TipoIdentificacion = "Cédula", NumeroIdentificacion = "40212345678", Direccion = "Calle 1", Telefono = "8091112222" },
                    new Cliente { NombreRazonSocial = "Maria Ruiz", TipoIdentificacion = "Cédula", NumeroIdentificacion = "00165432101", Direccion = "Av. Siempre Viva", Telefono = "8093334444" },
                    new Cliente { NombreRazonSocial = "Ferreteria El Clavo", TipoIdentificacion = "RNC", NumeroIdentificacion = "131456789", Direccion = "Ensanche Naco", Telefono = "8295556666" },
                    new Cliente { NombreRazonSocial = "Construcciones SRL", TipoIdentificacion = "RNC", NumeroIdentificacion = "130112233", Direccion = "Bella Vista", Telefono = "8097778888" },
                    new Cliente { NombreRazonSocial = "Pedro Gomez", TipoIdentificacion = "Cédula", NumeroIdentificacion = "00100998877", Direccion = "Zona Colonial", Telefono = "8099990000" }
                );
                context.SaveChanges();
            }

            if (!context.Categorias.Any())
            {
                var catPlomeria = new Categoria { Nombre = "Plomería" };
                var catHerr = new Categoria { Nombre = "Herramientas" };
                var catElec = new Categoria { Nombre = "Electricidad" };
                var catConst = new Categoria { Nombre = "Construcción" };
                
                context.Categorias.AddRange(catPlomeria, catHerr, catElec, catConst);
                context.SaveChanges();

                if (!context.Productos.Any())
                {
                    context.Productos.AddRange(
                        new Producto { Sku = "PROD001", Nombre = "Tubo PVC 1/2", Descripcion = "Tubo PVC de media pulgada", Marca = "Amanco", UnidadMedida = "Tubo", CategoriaId = catPlomeria.Id, PrecioCosto = 100, MargenGanancia = 20, PrecioVenta = 120, TieneItbis = true, Itbis = 18, StockActual = 50, StockMinimo = 10 },
                        new Producto { Sku = "PROD002", Nombre = "Martillo de acero", Descripcion = "Martillo con mango de goma", Marca = "Stanley", UnidadMedida = "Pieza", CategoriaId = catHerr.Id, PrecioCosto = 300, MargenGanancia = 25, PrecioVenta = 375, TieneItbis = true, Itbis = 18, StockActual = 20, StockMinimo = 5 },
                        new Producto { Sku = "PROD003", Nombre = "Alicate", Descripcion = "Alicate de presión", Marca = "Stanley", UnidadMedida = "Pieza", CategoriaId = catHerr.Id, PrecioCosto = 200, MargenGanancia = 30, PrecioVenta = 260, TieneItbis = true, Itbis = 18, StockActual = 30, StockMinimo = 5 },
                        new Producto { Sku = "PROD004", Nombre = "Cable Eléctrico #12", Descripcion = "Cable AWG 12 THHN", Marca = "Inca", UnidadMedida = "Pie", CategoriaId = catElec.Id, PrecioCosto = 15, MargenGanancia = 40, PrecioVenta = 21, TieneItbis = true, Itbis = 18, StockActual = 1000, StockMinimo = 100 },
                        new Producto { Sku = "PROD005", Nombre = "Tomacorriente Doble", Descripcion = "Tomacorriente blanco 110V", Marca = "Leviton", UnidadMedida = "Pieza", CategoriaId = catElec.Id, PrecioCosto = 80, MargenGanancia = 25, PrecioVenta = 100, TieneItbis = true, Itbis = 18, StockActual = 150, StockMinimo = 20 },
                        new Producto { Sku = "PROD006", Nombre = "Pintura Acrílica Blanca", Descripcion = "Pintura de interior", Marca = "Tropical", UnidadMedida = "Galón", CategoriaId = catConst.Id, PrecioCosto = 800, MargenGanancia = 20, PrecioVenta = 960, TieneItbis = true, Itbis = 18, StockActual = 40, StockMinimo = 10 },
                        new Producto { Sku = "PROD007", Nombre = "Cemento Gris", Descripcion = "Funda de cemento gris 42.5kg", Marca = "Cemex", UnidadMedida = "Funda", CategoriaId = catConst.Id, PrecioCosto = 350, MargenGanancia = 15, PrecioVenta = 402.50m, TieneItbis = false, Itbis = 0, StockActual = 200, StockMinimo = 50 },
                        new Producto { Sku = "PROD008", Nombre = "Codo PVC 1/2", Descripcion = "Codo PVC liso a 90 grados", Marca = "Amanco", UnidadMedida = "Pieza", CategoriaId = catPlomeria.Id, PrecioCosto = 10, MargenGanancia = 50, PrecioVenta = 15, TieneItbis = true, Itbis = 18, StockActual = 500, StockMinimo = 100 },
                        new Producto { Sku = "PROD009", Nombre = "Varilla Corrugada 3/8", Descripcion = "Varilla para construcción", Marca = "Aceros", UnidadMedida = "Quintal", CategoriaId = catConst.Id, PrecioCosto = 2500, MargenGanancia = 10, PrecioVenta = 2750, TieneItbis = true, Itbis = 18, StockActual = 10, StockMinimo = 2 },
                        new Producto { Sku = "PROD010", Nombre = "Cinta Métrica 5m", Descripcion = "Cinta de medir retráctil", Marca = "Truper", UnidadMedida = "Pieza", CategoriaId = catHerr.Id, PrecioCosto = 200, MargenGanancia = 30, PrecioVenta = 260, TieneItbis = true, Itbis = 18, StockActual = 40, StockMinimo = 10 }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
