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
        public int VehiculoId { get; set; }
        public string Placa { get; set; }
        public bool AbonoAnulado { get; set; }
    }
}
