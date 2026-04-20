using System.Collections.Generic;

namespace Ferreteria.Application.DTOs.Facturacion
{
    public class DashboardTendenciasDto
    {
        public TendenciaData Tendencia { get; set; } = new TendenciaData();
        public TendenciaData Distribucion { get; set; } = new TendenciaData();
    }

    public class TendenciaData
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
    }
}
