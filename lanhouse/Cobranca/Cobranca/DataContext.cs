using LanHouse.Cobranca.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Cobranca
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Entidades.Cobranca> Cobrancas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entidades.Cobranca>().HasKey(p => p.Id);
            modelBuilder.Entity<Entidades.Cobranca>().HasIndex(p => p.ClienteId);
            modelBuilder.Entity<Entidades.Cobranca>().HasIndex(p => p.ReservaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
