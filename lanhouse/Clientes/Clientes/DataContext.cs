using LanHouse.Clientes.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Clientes
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasKey(p => p.Id);
            modelBuilder.Entity<Cliente>().HasIndex(p => p.Telefone).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
