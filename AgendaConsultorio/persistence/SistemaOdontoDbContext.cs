using Microsoft.EntityFrameworkCore;
using sistemaodonto;

namespace SistemaOdonto.Persistence
{
    public class SistemaOdontoDbContext : DbContext
    {
        public SistemaOdontoDbContext(DbContextOptions<SistemaOdontoDbContext> options) 
            : base(options)
        { }

        // DbSets (tabelas do banco de dados)
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Consulta> Consultas { get; set; }

        // Configuração adicional, se necessário
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Adicione mais configurações de mapeamento se necessário
        }
    }
}
