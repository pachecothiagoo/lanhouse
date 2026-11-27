using LanHouse.Reservas.DTO;

namespace LanHouse.Reservas.Infra
{
    /// <summary>Integração (alteração): Reservas -> Cobrança ("ao reservar, gera cobrança").</summary>
    public class CobrancaClient : ServicoExternoBase
    {
        public CobrancaClient() : base("Cobranca") { }

        public Task<CobrancaExternaDTO> Criar(int clienteId, int reservaId, string descricao, decimal valor) =>
            EnviarAsync<CobrancaExternaDTO>(HttpMethod.Post, "/api/cobrancas",
                new { clienteId, reservaId, descricao, valor });

        public Task Cancelar(int cobrancaId) =>
            EnviarAsync(HttpMethod.Put, $"/api/cobrancas/{cobrancaId}/cancelar");
    }
}
