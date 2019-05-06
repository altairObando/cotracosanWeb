namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Socios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Socios()
        {
            Vehiculos = new HashSet<Vehiculos>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        [Display(Name = "Codigo Socio")]
        public string CodigoSocio { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Primer Apellido")]
        public string Apellido1Socio { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Segundo Apellido")]
        public string Apellido2Socio { get; set; }

        public bool Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vehiculos> Vehiculos { get; set; }

        [NotMapped]
        public string SocioNombre
        {
            get
            {
                return this.CodigoSocio + " " + this.Nombres + " " + this.Apellido1Socio;
            }
        }
    }
}
