namespace LanHouse.Reservas.DTO
{
    public class ReservaCriarDTO
    {
        public int ClienteId { get; set; }
        public int ComputadorId { get; set; }
        /// <summary>Início do agendamento (opcional; padrão = agora).</summary>
        public DateTime? Inicio { get; set; }
        public int DuracaoMinutos { get; set; }
    }

    public class ReservaDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ComputadorId { get; set; }
        public int? CobrancaId { get; set; }
        public DateTime Inicio { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CriadaEm { get; set; }
        public DateTime? SessaoIniciadaEm { get; set; }
        public DateTime? SessaoFinalizadaEm { get; set; }
    }
}
