namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LugaresFinalesDelosRecorridos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LugaresFinalesDelosRecorridos()
        {
            Carreras = new HashSet<Carreras>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name = "Codigo")]
        public string CodigoDeLugar { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string NombreDeLugar { get; set; }

        public bool Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carreras> Carreras { get; set; }
    }
}
