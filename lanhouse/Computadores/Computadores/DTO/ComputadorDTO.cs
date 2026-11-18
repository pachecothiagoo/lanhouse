using LanHouse.Computadores.Entidades;

namespace LanHouse.Computadores.DTO
{
    public class ComputadorDTO
    {
        public int Id { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Hardware { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public string Status { get; set; } = StatusComputador.Livre;
    }

    public class StatusDTO
    {
        public string Status { get; set; } = string.Empty;
    }
}
