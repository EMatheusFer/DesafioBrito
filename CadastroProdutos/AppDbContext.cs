using Microsoft.EntityFrameworkCore;

namespace CadastroProdutos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CadastroProdutosDB;Username=seu_user;Password=sua_senha");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Descricao)
                .IsUnique();

            
            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.SKU)
                .IsUnique();
            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.Nome)
                .IsUnique();

           
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId);
        }
    }
}