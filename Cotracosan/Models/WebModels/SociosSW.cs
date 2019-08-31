using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.Models.WebModels
{
    public class SociosSW
    {
        public bool AbonoAnulado { get; set; }
        public string CodigoAbono { get; set; }
        public int CreditoId { get;  set; }
        public string FechaDeAbono { get; set; }
        public int IdAbono { get; set; }
        public decimal MontoDeAbono { get;  set; }
        public string Placa { get; set; }
        public int VehiculoId { get; set; }
    }
}