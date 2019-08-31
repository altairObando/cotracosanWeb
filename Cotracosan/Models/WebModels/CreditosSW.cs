using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.Models.WebModels
{
    public class CreditosSW
    {
        public CreditosSW()
        {
            this.DetallesDeCreditos = new List<DetallesDeCreditosSW>();
        }
        public string CodigoCredito { get; set; }
        public bool CreditoAnulado { get; set; }
        public bool EstadoDeCredito { get; set; }
        public string Fecha { get; set; }
        public int Id { get; set; }
        public decimal MontoTotal { get; set; }
        public int NumeroAbonos { get; set; }
        public decimal TotalAbonado { get; set; }
        public List<DetallesDeCreditosSW> DetallesDeCreditos { get; set; }
        public int VehiculoId { get; set; }
    }
}