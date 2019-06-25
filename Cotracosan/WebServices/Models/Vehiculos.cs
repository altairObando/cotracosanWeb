using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.WebServices.Models
{
    public class VehiculosASMX
    {
        public int Id { get; set; }
        public string Placa { get; set; }
    }
    public class VehiculosMontosSocio
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public decimal MontoRecaudado { get; set; }
    }
}