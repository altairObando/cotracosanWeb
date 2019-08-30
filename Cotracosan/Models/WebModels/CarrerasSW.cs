using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotracosan.Models.WebModels
{
    public class CarrerasSW
    {
        public int Id { get; set; }
        public string CodigoCarrera { get; set; }
        public DateTime FechaDeCarrera { get; set; }
        public TimeSpan HoraRealDeLlegada { get; set; }
        public bool CarreraAnulada { get; set; }
        public decimal MontoRecaudado { get; set; }
        public decimal Multa { get; set; }
        public string Vehiculo { get; set; }
        public string Conductor { get; set; }
        public string Turno { get; set; }
        public string LugarFinalDeRecorrido { get; set; }
        public bool Estado { get; set; }
    }
}