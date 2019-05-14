namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Carreras
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Codigo de Carrera")]
        public string CodigoCarrera { get; set; }
        [Display(Name = "Fecha")]
        public DateTime FechaDeCarrera { get; set; }
        [Display(Name = "Hora Llegada")]
        public TimeSpan HoraRealDeLlegada { get; set; }
        [Display(Name = "Anulada")]
        public bool CarreraAnulada { get; set; }
        [Display(Name = "Monto")]
        public decimal MontoRecaudado { get; set; }
        [Display(Name = "Multa")]
        public decimal Multa { get; set; }
        [Display(Name = "Vehiculo")]
        public int VehiculoId { get; set; }
        [Display(Name = "Conductor")]
        public int ConductorId { get; set; }
        [Display(Name = "Penalización")]
        public int PenalizacionId { get; set; }
        [Display(Name = "Turno")]
        public int TurnoId { get; set; }
        [Display(Name = "Recorrido")]
        public int LugarFinalDeRecorridoId { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public virtual Conductores Conductores { get; set; }

        public virtual LugaresFinalesDelosRecorridos LugaresFinalesDelosRecorridos { get; set; }

        public virtual Penalizaciones Penalizaciones { get; set; }

        public virtual Turnos Turnos { get; set; }

        public virtual Vehiculos Vehiculos { get; set; }
    }
}
