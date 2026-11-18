using LanHouse.Cobranca.DTO;
using LanHouse.Cobranca.Entidades;
using LanHouse.Cobranca.Infra;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Cobranca.Servicos
{
    public class ServCobranca
    {
        private readonly DataContext _dataContext;

        public ServCobranca()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // Integração (alteração): chamado pelo microsserviço de Reservas ao reservar.
        public CobrancaDTO Criar(CobrancaCriarDTO dto)
        {
            if (dto.ClienteId <= 0) throw new Exception("Informe o cliente da cobrança.");
            if (dto.ReservaId <= 0) throw new Exception("Informe a reserva da cobrança.");
            if (dto.Valor <= 0) throw new Exception("O valor da cobrança deve ser maior que zero.");

            if (_dataContext.Cobrancas.Any(c => c.ReservaId == dto.ReservaId && c.Status != StatusCobranca.Cancelada))
                throw new Exception($"Já existe uma cobrança ativa para a reserva {dto.ReservaId}.");

            var cobranca = new Entidades.Cobranca
            {
                ClienteId = dto.ClienteId,
                ReservaId = dto.ReservaId,
                Descricao = (dto.Descricao ?? string.Empty).Trim(),
                Valor = Math.Round(dto.Valor, 2),
                Status = StatusCobranca.Pendente,
                CriadaEm = DateTime.UtcNow
            };

            _dataContext.Cobrancas.Add(cobranca);
            _dataContext.SaveChanges();
            return Mapear(cobranca);
        }

        public List<CobrancaDTO> Listar(int? clienteId, string? status)
        {
            var consulta = _dataContext.Cobrancas.AsNoTracking().AsQueryable();

            if (clienteId.HasValue)
                consulta = consulta.Where(c => c.ClienteId == clienteId.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                consulta = consulta.Where(c => c.Status == s);
            }

            return consulta.OrderByDescending(c => c.Id).ToList().Select(Mapear).ToList();
        }

        public CobrancaDTO Obter(int id) => Mapear(Buscar(id));

        public CobrancaDTO Pagar(int id)
        {
            var cobranca = Buscar(id);
            if (cobranca.Status != StatusCobranca.Pendente)
                throw new Exception($"Só é possível pagar cobranças pendentes (status atual: {cobranca.Status}).");

            cobranca.Status = StatusCobranca.Paga;
            _dataContext.SaveChanges();
            return Mapear(cobranca);
        }

        public CobrancaDTO Cancelar(int id)
        {
            var cobranca = Buscar(id);
            if (cobranca.Status == StatusCobranca.Paga)
                throw new Exception("Cobrança já paga não pode ser cancelada.");

            cobranca.Status = StatusCobranca.Cancelada;
            _dataContext.SaveChanges();
            return Mapear(cobranca);
        }

        // Integração (busca): usado pelo microsserviço de Clientes.
        public SaldoDevedorDTO SaldoDevedor(int clienteId)
        {
            // A soma é feita em memória: o provider SQLite do EF Core não suporta SUM sobre decimal.
            var valores = _dataContext.Cobrancas.AsNoTracking()
                .Where(c => c.ClienteId == clienteId && c.Status == StatusCobranca.Pendente)
                .Select(c => c.Valor)
                .ToList();

            return new SaldoDevedorDTO { ClienteId = clienteId, SaldoDevedor = valores.Sum() };
        }

        private Entidades.Cobranca Buscar(int id) =>
            _dataContext.Cobrancas.Find(id)
                ?? throw new KeyNotFoundException($"Cobrança {id} não encontrada.");

        private static CobrancaDTO Mapear(Entidades.Cobranca c) => new()
        {
            Id = c.Id,
            ClienteId = c.ClienteId,
            ReservaId = c.ReservaId,
            Descricao = c.Descricao,
            Valor = c.Valor,
            Status = c.Status,
            CriadaEm = c.CriadaEm
        };
    }
}
