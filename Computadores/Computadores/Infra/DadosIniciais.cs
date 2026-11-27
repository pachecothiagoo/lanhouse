using LanHouse.Computadores.Entidades;

namespace LanHouse.Computadores.Infra
{
    // Dados de demonstração (o SQLite do plano Free do Render é recriado a cada reinício).
    public static class DadosIniciais
    {
        public static void Semear(DataContext db)
        {
            if (db.Computadores.Any()) return;

            db.Computadores.AddRange(
                new Computador { Identificacao = "PC-01", Tipo = "Gamer", Hardware = "Ryzen 5 5600 / RTX 3060 / 16GB RAM / SSD 480GB", ValorHora = 10m },
                new Computador { Identificacao = "PC-02", Tipo = "Gamer", Hardware = "Ryzen 5 5600 / RTX 3060 / 16GB RAM / SSD 480GB", ValorHora = 10m },
                new Computador { Identificacao = "PC-03", Tipo = "Padrao", Hardware = "Core i3 12100 / GTX 1650 / 8GB RAM / SSD 240GB", ValorHora = 6m },
                new Computador { Identificacao = "PC-04", Tipo = "Padrao", Hardware = "Core i3 12100 / GTX 1650 / 8GB RAM / SSD 240GB", ValorHora = 6m });
            db.SaveChanges();
        }
    }
}
