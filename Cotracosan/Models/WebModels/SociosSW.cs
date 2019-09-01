using Cotracosan.Models.Cotracosan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.Models.WebModels
{
    public class SociosSW
    {
        public int Id { get; set; }
        public string CodigoSocio { get; set; }
        public string Nombres { get; set; }
        public string Apellido1Socio { get; set; }
        public string Apellido2Socio { get; set; }
        public bool Estado { get; set; }
        public override string ToString()
        {
            return this.Nombres + " " + this.Apellido1Socio + " " +Apellido2Socio;
        }
    }
}