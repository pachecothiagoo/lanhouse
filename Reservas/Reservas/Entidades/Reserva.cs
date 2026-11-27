namespace LanHouse.Reservas.Entidades
{
    public class Reserva
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ComputadorId { get; set; }
        public int? CobrancaId { get; set; }
        public DateTime Inicio { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = StatusReserva.Agendada;
        public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
        public DateTime? SessaoIniciadaEm { get; set; }
        public DateTime? SessaoFinalizadaEm { get; set; }
    }

    public static class StatusReserva
    {
        public const string Agendada = "Agendada";
        public const string EmAndamento = "EmAndamento";
        public const string Finalizada = "Finalizada";
        public const string Cancelada = "Cancelada";
    }
}
