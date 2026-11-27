using LanHouse.Clientes.DTO;
using LanHouse.Clientes.Entidades;
using LanHouse.Clientes.Infra;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Clientes.Servicos
{
    public class ServCliente
    {
        private readonly DataContext _dataContext;

        public ServCliente()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // RF01
        public ClienteDTO Cadastrar(ClienteDTO dto)
        {
            var nome = (dto.Nome ?? string.Empty).Trim();
            var telefone = (dto.Telefone ?? string.Empty).Trim();

            if (nome.Length == 0) throw new Exception("Informe o nome do cliente.");
            if (telefone.Length == 0) throw new Exception("Informe o telefone do cliente.");

            var situacao = string.IsNullOrWhiteSpace(dto.Situacao)
                ? SituacaoCliente.Ativo
                : SituacaoCliente.Normalizar(dto.Situacao);

            if (_dataContext.Clientes.Any(c => c.Telefone == telefone))
                throw new Exception("Já existe um cliente cadastrado com este telefone.");

            var cliente = new Cliente { Nome = nome, Telefone = telefone, Situacao = situacao };
            _dataContext.Clientes.Add(cliente);
            _dataContext.SaveChanges();

            return Mapear(cliente);
        }

        // RF02
        public List<ClienteDTO> Consultar(string? nome, string? telefone)
        {
            var consulta = _dataContext.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var n = nome.Trim();
                consulta = consulta.Where(c => c.Nome.Contains(n));
            }

            if (!string.IsNullOrWhiteSpace(telefone))
            {
                var t = telefone.Trim();
                consulta = consulta.Where(c => c.Telefone.Contains(t));
            }

            return consulta.OrderBy(c => c.Nome).ToList().Select(Mapear).ToList();
        }

        // Usado também pelo microsserviço de Reservas (verifica se o cliente está ativo)
        public ClienteDTO Obter(int id)
        {
            var cliente = _dataContext.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Cliente {id} não encontrado.");
            return Mapear(cliente);
        }

        public ClienteDTO AlterarSituacao(int id, string situacao)
        {
            var cliente = _dataContext.Clientes.Find(id)
                ?? throw new KeyNotFoundException($"Cliente {id} não encontrado.");

            cliente.Situacao = SituacaoCliente.Normalizar(situacao);
            _dataContext.SaveChanges();
            return Mapear(cliente);
        }

        // Integração (busca): consulta o saldo devedor no microsserviço de Cobrança
        public async Task<SaldoDevedorDTO> SaldoDevedor(int id)
        {
            Obter(id); // garante que o cliente existe
            return await new CobrancaClient().ObterSaldoDevedor(id);
        }

        private static ClienteDTO Mapear(Cliente c) => new()
        {
            Id = c.Id,
            Nome = c.Nome,
            Telefone = c.Telefone,
            Situacao = c.Situacao
        };
    }
}
