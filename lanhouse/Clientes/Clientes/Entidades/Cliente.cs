namespace LanHouse.Clientes.Entidades
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Situacao { get; set; } = SituacaoCliente.Ativo;
    }

    public static class SituacaoCliente
    {
        public const string Ativo = "Ativo";
        public const string Bloqueado = "Bloqueado";

        public static string Normalizar(string? situacao)
        {
            if (string.Equals(situacao?.Trim(), Ativo, StringComparison.OrdinalIgnoreCase)) return Ativo;
            if (string.Equals(situacao?.Trim(), Bloqueado, StringComparison.OrdinalIgnoreCase)) return Bloqueado;
            throw new Exception("Situação inválida. Use 'Ativo' ou 'Bloqueado'.");
        }
    }
}
