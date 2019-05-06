namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Articulos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Articulos()
        {
            DetallesDeCreditos = new HashSet<DetallesDeCreditos>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name ="Codigo Articulo")]
        public string CodigoDeArticulo { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string DescripcionDeArticulo { get; set; }

        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        public bool Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetallesDeCreditos> DetallesDeCreditos { get; set; }
    }
}
