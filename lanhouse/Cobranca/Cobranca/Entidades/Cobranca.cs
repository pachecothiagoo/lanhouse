namespace LanHouse.Cobranca.Entidades
{
    public class Cobranca
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ReservaId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Status { get; set; } = StatusCobranca.Pendente;
        public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    }

    public static class StatusCobranca
    {
        public const string Pendente = "Pendente";
        public const string Paga = "Paga";
        public const string Cancelada = "Cancelada";
    }
}
