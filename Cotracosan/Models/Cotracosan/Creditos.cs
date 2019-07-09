namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Spatial;
    using System.Globalization;

    public partial class Creditos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Creditos()
        {
            Abonos = new HashSet<Abonos>();
            DetallesDeCreditos = new HashSet<DetallesDeCreditos>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Credito")]
        public string CodigoCredito { get; set; }
        [Display(Name = "Fecha")]
        public DateTime FechaDeCredito { get; set; }
        [Display(Name = "Monto Total")]
        public decimal MontoTotal { get; set; }
        [Display(Name = "Estado")]
        public bool EstadoDeCredito { get; set; }

        [Required]
        [Display(Name = "Anulado")]
        public bool CreditoAnulado { get; set; }
        [Display(Name = "Vehiculo")]
        public int VehiculoId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Abonos> Abonos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetallesDeCreditos> DetallesDeCreditos { get; set; }

        public virtual Vehiculos Vehiculos { get; set; }

        [NotMapped]
        public string InfoCredito { get {
                return this.CodigoCredito + " " + Vehiculos.Placa + " " + string.Format("{0:C2}", MontoTotal);
            } }
        [NotMapped]
        public bool Cancelado
        {
            get
            {
                return Abonos.Where(y => y.Estado).Sum(x => x.MontoDeAbono) >= MontoTotal;
            }
        }

        [NotMapped]
        public string NombreMes
        {
            get
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(FechaDeCredito.Month);
            }
        }
    }
}
