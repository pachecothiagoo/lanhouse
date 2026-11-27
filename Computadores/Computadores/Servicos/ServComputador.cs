using LanHouse.Computadores.DTO;
using LanHouse.Computadores.Entidades;
using LanHouse.Computadores.Infra;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Computadores.Servicos
{
    public class ServComputador
    {
        // Evita que duas reservas simultâneas peguem o mesmo computador livre.
        private static readonly object Trava = new();

        private readonly DataContext _dataContext;

        public ServComputador()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // RF03
        public ComputadorDTO Cadastrar(ComputadorDTO dto)
        {
            var identificacao = (dto.Identificacao ?? string.Empty).Trim();
            var tipo = (dto.Tipo ?? string.Empty).Trim();
            var hardware = (dto.Hardware ?? string.Empty).Trim();

            if (identificacao.Length == 0) throw new Exception("Informe a identificação do computador (ex.: PC-05).");
            if (tipo.Length == 0) throw new Exception("Informe o tipo do computador (ex.: Gamer, Padrao).");
            if (hardware.Length == 0) throw new Exception("Informe o hardware do computador.");
            if (dto.ValorHora <= 0) throw new Exception("Informe o valor da hora (maior que zero).");

            if (_dataContext.Computadores.Any(c => c.Identificacao == identificacao))
                throw new Exception($"Já existe um computador com a identificação '{identificacao}'.");

            var computador = new Computador
            {
                Identificacao = identificacao,
                Tipo = tipo,
                Hardware = hardware,
                ValorHora = dto.ValorHora,
                Status = StatusComputador.Livre
            };

            _dataContext.Computadores.Add(computador);
            _dataContext.SaveChanges();
            return Mapear(computador);
        }

        public List<ComputadorDTO> Listar(string? status)
        {
            var consulta = _dataContext.Computadores.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = StatusComputador.Normalizar(status);
                consulta = consulta.Where(c => c.Status == s);
            }

            return consulta.OrderBy(c => c.Identificacao).ToList().Select(Mapear).ToList();
        }

        // Integração (busca): usado pelo microsserviço de Reservas
        public ComputadorDTO Obter(int id)
        {
            var computador = _dataContext.Computadores.AsNoTracking().FirstOrDefault(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Computador {id} não encontrado.");
            return Mapear(computador);
        }

        // RF04
        public ComputadorDTO AlterarStatus(int id, string status)
        {
            lock (Trava)
            {
                var computador = _dataContext.Computadores.Find(id)
                    ?? throw new KeyNotFoundException($"Computador {id} não encontrado.");

                computador.Status = StatusComputador.Normalizar(status);
                _dataContext.SaveChanges();
                return Mapear(computador);
            }
        }

        // Integração (alteração): chamado pelo microsserviço de Reservas. Só reserva se estiver livre (RF06).
        public ComputadorDTO Reservar(int id)
        {
            lock (Trava)
            {
                var computador = _dataContext.Computadores.Find(id)
                    ?? throw new KeyNotFoundException($"Computador {id} não encontrado.");

                if (computador.Status != StatusComputador.Livre)
                    throw new Exception($"O computador {computador.Identificacao} não está livre (status atual: {computador.Status}).");

                computador.Status = StatusComputador.Reservado;
                _dataContext.SaveChanges();
                return Mapear(computador);
            }
        }

        private static ComputadorDTO Mapear(Computador c) => new()
        {
            Id = c.Id,
            Identificacao = c.Identificacao,
            Tipo = c.Tipo,
            Hardware = c.Hardware,
            ValorHora = c.ValorHora,
            Status = c.Status
        };
    }
}
