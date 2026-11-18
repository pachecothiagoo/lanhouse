using LanHouse.Computadores.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Computadores
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Computador> Computadores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Computador>().HasKey(p => p.Id);
            modelBuilder.Entity<Computador>().HasIndex(p => p.Identificacao).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
