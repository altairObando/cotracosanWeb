namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Abonos
    {
        public int Id { get; set; }

        public DateTime FechaDeAbono { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name ="Codigo Abono")]
        public string CodigoAbono { get; set; }
        [Display(Name = "Monto")]
        public decimal MontoDeAbono { get; set; }
        [Display(Name = "Id Credito")]
        public int CreditoId { get; set; }

        public bool Estado { get; set; }

        public virtual Creditos Creditos { get; set; }
    }
}
