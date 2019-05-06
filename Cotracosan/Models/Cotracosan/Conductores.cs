namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Conductores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Conductores()
        {
            Carreras = new HashSet<Carreras>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        public string Licencia { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Primer Apellido")]
        public string Apellido1Conductor { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Segundo Apellido")]
        public string Apellido2Conductor { get; set; }

        public bool Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carreras> Carreras { get; set; }
    }
}
