using Microsoft.EntityFrameworkCore;

namespace CadastroProdutos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CadastroProdutosDB;Username=postgres;Password=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Descricao)
                .IsUnique();

            
            modelBuilder.Entity<Categoria>()
                .HasQueryFilter(c => c.Ativo);


            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.Nome)
                .IsUnique();

        
            modelBuilder.Entity<Produto>()
                .HasQueryFilter(p => p.Ativo);

   
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict); 
        }

        public IQueryable<Categoria> CategoriasInativas()
        {
            return Categorias.IgnoreQueryFilters().Where(c => !c.Ativo);
        }

        public IQueryable<Produto> ProdutosInativos()
        {
            return Produtos.IgnoreQueryFilters().Where(p => !p.Ativo);
        }
    }
}