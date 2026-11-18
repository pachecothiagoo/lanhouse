using LanHouse.Clientes.Entidades;

namespace LanHouse.Clientes.DTO
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Situacao { get; set; } = SituacaoCliente.Ativo;
    }

    public class SituacaoDTO
    {
        public string Situacao { get; set; } = string.Empty;
    }
}
