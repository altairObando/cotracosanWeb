namespace Cotracosan.Models.Cotracosan
{
    using System;
    [Serializable]
    public class AbonosWS
    {
        public int Id { get; set; }
        public int CreditoId { get; set; }
        public DateTime FechaDeAbono { get; set; }
        public string CodigoAbono { get; set; }
        public decimal MontoDeAbono { get; set; }
    }
}
