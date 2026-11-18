using Microsoft.EntityFrameworkCore;

namespace LanHouse.Cobranca.Infra
{
    public static class GeradorDeServicos
    {
        public static ServiceProvider ServiceProvider = default!;
        public static IConfiguration Configuracao = default!;

        public static DataContext CarregarContexto()
        {
            // Um contexto novo por chamada: o DbContext não é thread-safe
            // e não deve ser compartilhado entre requisições simultâneas.
            var opcoes = ServiceProvider.GetRequiredService<DbContextOptions<DataContext>>();
            return new DataContext(opcoes);
        }
    }
}
