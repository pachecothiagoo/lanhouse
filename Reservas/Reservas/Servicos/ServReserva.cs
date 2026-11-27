using LanHouse.Reservas.DTO;
using LanHouse.Reservas.Entidades;
using LanHouse.Reservas.Infra;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Reservas.Servicos
{
    public class ServReserva
    {
        private readonly DataContext _dataContext;
        private readonly ClientesClient _clientes = new();
        private readonly ComputadoresClient _computadores = new();
        private readonly CobrancaClient _cobranca = new();

        public ServReserva()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // RF06 + fluxo do diagrama: Reservas -> Clientes (busca), Computadores (busca e alteração), Cobrança (alteração)
        public async Task<ReservaDTO> Criar(ReservaCriarDTO dto)
        {
            if (dto.DuracaoMinutos < 15 || dto.DuracaoMinutos > 720)
                throw new Exception("A duração deve estar entre 15 e 720 minutos.");

            // Integração 1 (busca): o cliente existe e está ativo?
            var cliente = await _clientes.Obter(dto.ClienteId);
            if (cliente.Situacao != ContratoStatus.ClienteAtivo)
                throw new Exception($"O cliente {cliente.Nome} está '{cliente.Situacao}' e não pode fazer reservas.");

            // Integração 2 (busca): o computador existe e está livre?
            var computador = await _computadores.Obter(dto.ComputadorId);
            if (computador.Status != ContratoStatus.ComputadorLivre)
                throw new Exception($"O computador {computador.Identificacao} não está livre (status atual: {computador.Status}).");

            var valor = Math.Round(computador.ValorHora * dto.DuracaoMinutos / 60m, 2);

            // Integração 3 (alteração): marca o computador como reservado (o serviço de Computadores valida de novo)
            await _computadores.Reservar(computador.Id);

            var reserva = new Reserva
            {
                ClienteId = cliente.Id,
                ComputadorId = computador.Id,
                Inicio = (dto.Inicio ?? DateTime.UtcNow).ToUniversalTime(),
                DuracaoMinutos = dto.DuracaoMinutos,
                ValorTotal = valor,
                Status = StatusReserva.Agendada,
                CriadaEm = DateTime.UtcNow
            };

            int? cobrancaId = null;
            try
            {
                _dataContext.Reservas.Add(reserva);
                _dataContext.SaveChanges();

                // Integração 4 (alteração): gera a cobrança no serviço de Cobrança
                var cobranca = await _cobranca.Criar(
                    cliente.Id,
                    reserva.Id,
                    $"Reserva {reserva.Id} - {computador.Identificacao} ({dto.DuracaoMinutos} min)",
                    valor);

                cobrancaId = cobranca.Id;
                reserva.CobrancaId = cobranca.Id;
                _dataContext.SaveChanges();
            }
            catch
            {
                // Sem transação distribuída, desfazemos manualmente o que já foi alterado.
                await Compensar(reserva, computador.Id, cobrancaId);
                throw;
            }

            return Mapear(reserva);
        }

        // RF05: iniciar sessão a partir de um agendamento, somente se o cliente estiver ativo
        public async Task<ReservaDTO> IniciarSessao(int id)
        {
            var reserva = Buscar(id);
            if (reserva.Status != StatusReserva.Agendada)
                throw new Exception($"Só é possível iniciar a sessão de uma reserva 'Agendada' (status atual: {reserva.Status}).");

            var cliente = await _clientes.Obter(reserva.ClienteId);
            if (cliente.Situacao != ContratoStatus.ClienteAtivo)
                throw new Exception($"O cliente {cliente.Nome} está '{cliente.Situacao}'. A sessão só pode ser iniciada para clientes ativos.");

            await _computadores.AlterarStatus(reserva.ComputadorId, ContratoStatus.ComputadorEmUso);

            reserva.Status = StatusReserva.EmAndamento;
            reserva.SessaoIniciadaEm = DateTime.UtcNow;
            _dataContext.SaveChanges();
            return Mapear(reserva);
        }

        public async Task<ReservaDTO> Finalizar(int id)
        {
            var reserva = Buscar(id);
            if (reserva.Status != StatusReserva.EmAndamento)
                throw new Exception($"Só é possível finalizar uma reserva 'EmAndamento' (status atual: {reserva.Status}).");

            await _computadores.AlterarStatus(reserva.ComputadorId, ContratoStatus.ComputadorLivre);

            reserva.Status = StatusReserva.Finalizada;
            reserva.SessaoFinalizadaEm = DateTime.UtcNow;
            _dataContext.SaveChanges();
            return Mapear(reserva);
        }

        public async Task<ReservaDTO> Cancelar(int id)
        {
            var reserva = Buscar(id);
            if (reserva.Status != StatusReserva.Agendada)
                throw new Exception($"Só é possível cancelar uma reserva 'Agendada' (status atual: {reserva.Status}). Para sessões em andamento, use finalizar.");

            await _computadores.AlterarStatus(reserva.ComputadorId, ContratoStatus.ComputadorLivre);
            if (reserva.CobrancaId.HasValue)
                await _cobranca.Cancelar(reserva.CobrancaId.Value);

            reserva.Status = StatusReserva.Cancelada;
            _dataContext.SaveChanges();
            return Mapear(reserva);
        }

        public ReservaDTO Obter(int id) => Mapear(Buscar(id));

        public List<ReservaDTO> Listar(int? clienteId, string? status)
        {
            var consulta = _dataContext.Reservas.AsNoTracking().AsQueryable();

            if (clienteId.HasValue)
                consulta = consulta.Where(r => r.ClienteId == clienteId.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                consulta = consulta.Where(r => r.Status == s);
            }

            return consulta.OrderByDescending(r => r.Id).ToList().Select(Mapear).ToList();
        }

        private Reserva Buscar(int id) =>
            _dataContext.Reservas.Find(id)
                ?? throw new KeyNotFoundException($"Reserva {id} não encontrada.");

        // Desfaz, em melhor esforço, as alterações feitas quando a criação da reserva falha no meio do caminho.
        private async Task Compensar(Reserva reserva, int computadorId, int? cobrancaId)
        {
            try { await _computadores.AlterarStatus(computadorId, ContratoStatus.ComputadorLivre); }
            catch { /* melhor esforço: a falha original é a que importa para quem chamou */ }

            if (cobrancaId.HasValue)
            {
                try { await _cobranca.Cancelar(cobrancaId.Value); }
                catch { /* melhor esforço */ }
            }

            try
            {
                if (reserva.Id > 0)
                {
                    _dataContext.Reservas.Remove(reserva);
                    _dataContext.SaveChanges();
                }
            }
            catch { /* melhor esforço */ }
        }

        private static ReservaDTO Mapear(Reserva r) => new()
        {
            Id = r.Id,
            ClienteId = r.ClienteId,
            ComputadorId = r.ComputadorId,
            CobrancaId = r.CobrancaId,
            Inicio = r.Inicio,
            DuracaoMinutos = r.DuracaoMinutos,
            ValorTotal = r.ValorTotal,
            Status = r.Status,
            CriadaEm = r.CriadaEm,
            SessaoIniciadaEm = r.SessaoIniciadaEm,
            SessaoFinalizadaEm = r.SessaoFinalizadaEm
        };
    }
}
