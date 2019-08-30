namespace Cotracosan.Models.Cotracosan
{
    using System;
    public partial class AbonosWS
    {
        public int Id { get; set; }
        public int CreditoId { get; set; }
        public DateTime FechaDeAbono { get; set; }
        public string CodigoAbono { get; set; }
        public decimal MontoDeAbono { get; set; }
    }
}
