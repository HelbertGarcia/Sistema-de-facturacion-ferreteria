# FerreteríaPOS - Sistema de Facturación y Control de Inventario 🛠️

FerreteríaPOS es una aplicación robusta diseñada bajo los principios de **Clean Architecture** (Arquitectura Limpia) enfocada en solucionar la gestión comercial, facturación y control de inventario de una pequeña a mediana ferretería.

## 🚀 Tecnologías y Arquitectura

Este sistema fue desarrollado utilizando un stack tecnológico moderno centrado en la plataforma de Microsoft:
* **Backend:** ASP.NET Core 10 (MVC) / C#
* **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure, Web)
* **Base de Datos:** SQL Server / Entity Framework Core (Code-First)
* **Frontend:** Razor Views (HTML5/CSS3), JavaScript, jQuery, Fetch API.
* **Métricas y Gráficos:** Chart.js

### Estructura del Proyecto (Clean Architecture)
1. **Ferreteria.Domain**: Entidades de negocio base (Producto, Factura, Usuario, Cliente, etc). Completamente aislado de dependencias externas.
2. **Ferreteria.Application**: Casos de uso de la aplicación, interfaces de repositorios/servicios, y DTOs (Data Transfer Objects).
3. **Ferreteria.Infrastructure**: Implementación concreta de los accesos a datos usando **Entity Framework Core**. Contiene el DbContext y la lógica real de guardado hacia SQL Server.
4. **Ferreteria.Web**: Capa de presentación (MVC) construida con Controladores (Controllers) y Vistas de Razor (Views) que exponen la interfaz final al usuario e interactúan exclusivamente con la capa de *Application*.

---

## 📌 Funcionalidades Core (Épicas Desarrolladas)

### 1. Gestión de Catálogo e Inventario
* Mantenimiento de categorías y productos (CRUD Completo).
* Gestión de alertas de "Surtido Bajo", "Agotado" y stock saludable.
* Tracking estricto donde **cada entrada (compra/abastecimiento)** y **cada salida (ventas)** quedan registradas en una bitácora de Movimientos de Inventario.

### 2. Módulo de Puntos de Venta (POS) y Facturación
* Creación de nuevas ventas en tiempo real cargadas por lotes.
* Autocompletado, búsqueda instantánea de productos por nombre o SKU.
* Validaciones automáticas de stock (evitar pre-vender lo inexistente).
* Cálculo automático de Subtolates, ITBIS (18%) y descuentos.
* Soporte a facturas por *Efectivo, Tarjeta y Transferencia*.
* Generación de comprobante detallado (`/Factura/Detalle`).

### 3. Panel Gerencial (Dashboard interactivo)
* Observabilidad en Tiempo Real.
* Identificación inmediata de productos con alertas de desabastecimiento.
* Gráfico lineal de tendencia que evalúa la recaudación de los últimos 7 días.
* Gráfico de "Ventas por Categoría" (distribución tipo doughnut).

### 4. Entorno de Reportería Analítica 
Todas las representaciones y tablas están diseñadas para exportarse de manera **nativa y ultraligera** usando las virtudes de impresión del propio navegador.
* **Cierre Diario de Caja:** Subtotales brutos, impuestos y detalle de transacciones separadas por tipo de cobro del día actual o fecha seleccionada.
* **Top Más Vendidos:** Sistema que evalúa de mayor a menor qué producto genera mayor rentabilidad (`GROUP BY` avanzado).
* **Inventario Valorizado:** Evaluación del costo invertido de almacén versus precio de venta total, además de detector inteligente de mercancía "estancada" (más de 60 días sin actividad).

## 📥 Requisitos Previos e Instalación

1. Tener instalado el **SDK de .NET 10** o superior.
2. Servidor base de datos **SQL Server** o (SQL Express LocalDb).
3. Modificar la cadena de conexión de la Base de Datos (`ConnectionStrings:DefaultConnection`) en `Ferreteria.Web/appsettings.json` para que coincida con tu instancia local.

### Ejecución
```bash
# Navegar a la carpeta contenedora web
cd Ferreteria.Web

# Aplicar las migraciones hacia la base de datos vacía (Crear tablas)
dotnet ef database update --project ../Ferreteria.Infrastructure

# Ejecutar modo local
dotnet run
```
La aplicación correrá de normalidad en [http://localhost:5000](http://localhost:5000) o `5115` dependiendo la configuración local. Para iniciar sesión, utilice cualquier registro base del módulo User / Auth.
