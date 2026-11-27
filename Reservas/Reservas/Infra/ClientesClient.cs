using LanHouse.Reservas.DTO;

namespace LanHouse.Reservas.Infra
{
    /// <summary>Integração (busca): Reservas -> Clientes ("cliente ativo ou não").</summary>
    public class ClientesClient : ServicoExternoBase
    {
        public ClientesClient() : base("Clientes") { }

        public Task<ClienteExternoDTO> Obter(int id) =>
            EnviarAsync<ClienteExternoDTO>(HttpMethod.Get, $"/api/clientes/{id}");
    }
}
