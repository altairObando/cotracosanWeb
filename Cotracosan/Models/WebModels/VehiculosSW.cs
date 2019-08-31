using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.Models.WebModels
{
    public class VehiculosSW
    {
        public bool Estado { get; set; }
        public int Id { get; set; }
        public string Placa { get; set; }
        public int SocioId { get; set; }
    }
}