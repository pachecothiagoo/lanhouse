using LanHouse.Clientes.Entidades;

namespace LanHouse.Clientes.Infra
{
    // Dados de demonstração (o SQLite do plano Free do Render é recriado a cada reinício).
    public static class DadosIniciais
    {
        public static void Semear(DataContext db)
        {
            if (db.Clientes.Any()) return;

            db.Clientes.AddRange(
                new Cliente { Nome = "Carlos Silva", Telefone = "48999990001", Situacao = SituacaoCliente.Ativo },
                new Cliente { Nome = "Ana Souza", Telefone = "48999990002", Situacao = SituacaoCliente.Ativo },
                new Cliente { Nome = "Pedro Lima", Telefone = "48999990003", Situacao = SituacaoCliente.Bloqueado });
            db.SaveChanges();
        }
    }
}
