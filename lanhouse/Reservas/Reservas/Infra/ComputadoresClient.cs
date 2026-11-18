using LanHouse.Reservas.DTO;

namespace LanHouse.Reservas.Infra
{
    /// <summary>Integração (busca e alteração): Reservas -> Computadores ("consulta e reserva").</summary>
    public class ComputadoresClient : ServicoExternoBase
    {
        public ComputadoresClient() : base("Computadores") { }

        public Task<ComputadorExternoDTO> Obter(int id) =>
            EnviarAsync<ComputadorExternoDTO>(HttpMethod.Get, $"/api/computadores/{id}");

        public Task Reservar(int id) =>
            EnviarAsync(HttpMethod.Post, $"/api/computadores/{id}/reservar");

        public Task AlterarStatus(int id, string status) =>
            EnviarAsync(HttpMethod.Put, $"/api/computadores/{id}/status", new { status });
    }
}
