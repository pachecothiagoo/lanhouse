using LanHouse.Reservas.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LanHouse.Reservas
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Reserva> Reservas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reserva>().HasKey(p => p.Id);
            modelBuilder.Entity<Reserva>().HasIndex(p => p.ClienteId);
            modelBuilder.Entity<Reserva>().HasIndex(p => p.ComputadorId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
