namespace LanHouse.Reservas.DTO
{
    // Contratos dos outros microsserviços (cada serviço mantém sua própria cópia, sem compartilhar código).

    public class ClienteExternoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
    }

    public class ComputadorExternoDTO
    {
        public int Id { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Hardware { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CobrancaExternaDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ReservaId { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public static class ContratoStatus
    {
        public const string ClienteAtivo = "Ativo";
        public const string ComputadorLivre = "Livre";
        public const string ComputadorEmUso = "EmUso";
    }
}
