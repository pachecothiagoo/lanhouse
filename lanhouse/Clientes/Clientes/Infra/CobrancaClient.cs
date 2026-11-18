using LanHouse.Clientes.DTO;

namespace LanHouse.Clientes.Infra
{
    /// <summary>Integração (busca): Clientes -> Cobrança.</summary>
    public class CobrancaClient : ServicoExternoBase
    {
        public CobrancaClient() : base("Cobranca") { }

        public Task<SaldoDevedorDTO> ObterSaldoDevedor(int clienteId) =>
            EnviarAsync<SaldoDevedorDTO>(HttpMethod.Get, $"/api/cobrancas/cliente/{clienteId}/saldo");
    }
}
