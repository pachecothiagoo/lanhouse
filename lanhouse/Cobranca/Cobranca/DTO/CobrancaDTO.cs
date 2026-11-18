namespace LanHouse.Cobranca.DTO
{
    public class CobrancaDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ReservaId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CriadaEm { get; set; }
    }

    public class CobrancaCriarDTO
    {
        public int ClienteId { get; set; }
        public int ReservaId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class SaldoDevedorDTO
    {
        public int ClienteId { get; set; }
        public decimal SaldoDevedor { get; set; }
    }
}
