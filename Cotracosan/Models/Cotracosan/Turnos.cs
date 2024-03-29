namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Turnos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Turnos()
        {
            Carreras = new HashSet<Carreras>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(4)]
        [Display (Name="Codigo de turno")]
        public string CodigoDeTurno { get; set; }
        [Display(Name = "Hora De Salida")]
        public TimeSpan HoraDeSalida { get; set; }
        [Display(Name = "Hora De Llegada")]
        public TimeSpan HoraDeLlegada { get; set; }
        public bool Estado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carreras> Carreras { get; set; }
        [NotMapped]
        public string HorasTurno { get {

                string salida = string.Format("{0}:{1} {2}", (HoraDeSalida.Hours % 12), HoraDeSalida.ToString(@"mm"), HoraDeSalida.Hours > 11 ? "PM" : "AM");
                string llegada = string.Format("{0}:{1} {2}", (HoraDeLlegada.Hours % 12), HoraDeLlegada.ToString(@"mm"), HoraDeLlegada.Hours > 11 ? "PM" : "AM");

                return salida + " - " + llegada;
            } }
    }
}
