namespace LanHouse.Computadores.Entidades
{
    public class Computador
    {
        public int Id { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Hardware { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public string Status { get; set; } = StatusComputador.Livre;
    }

    public static class StatusComputador
    {
        public const string Livre = "Livre";
        public const string Reservado = "Reservado";
        public const string EmUso = "EmUso";

        public static string Normalizar(string? status)
        {
            var s = status?.Trim();
            foreach (var valido in new[] { Livre, Reservado, EmUso })
                if (string.Equals(s, valido, StringComparison.OrdinalIgnoreCase)) return valido;
            throw new Exception("Status inválido. Use 'Livre', 'Reservado' ou 'EmUso'.");
        }
    }
}
