using System;

namespace Cotracosan.WebServices.Models
{
    public class CarrerasASMX
    {
        public int Id { get; set; }
        public string CodigoCarrera { get; set; }
        public DateTime FechaDeCarrera { get; set; }
        public TimeSpan HoraRealDeLlegada { get; set; }
        public decimal MontoRecaudado { get; set; }
        public decimal Multa { get; set; }
        public string Vehiculo { get; set; }
        public string Conductor { get; set; }
        public string Penalizacion { get; set; }
        public string Turno { get; set; }
        public string LugarFinalDeRecorrido { get; set; }
        public bool CarreraAnulada { get; set; }
        public bool Estado { get; set; }                
    }
}