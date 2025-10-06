using Microsoft.EntityFrameworkCore;
using BoticaOnlineAPI.Models;

namespace BoticaOnlineAPI.Data
{
    public class BoticaDbContext : DbContext
    {
        public BoticaDbContext(DbContextOptions<BoticaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Prescripcion> Prescripciones { get; set; }
    }
}