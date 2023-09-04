using Microsoft.EntityFrameworkCore;
using MinimalApi.models;

namespace MinimalApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public  DbSet<Categoria> Categorias { get; set;}
        public  DbSet<Produto> Produtos { get; set;}

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // ------ configurando categoria
            mb.Entity<Categoria>().HasKey(c => c.CategoriaId);
            
            mb.Entity<Categoria>().Property(c=>c.Nome)
                                  .HasMaxLength(100)
                                  .IsRequired();

            mb.Entity<Categoria>().Property(c=>c.Descricao)
                                  .HasMaxLength(200)
                                  .IsRequired();


            // ------ configurando Produto
            mb.Entity<Produto>().HasKey(c => c.ProdutoId);
            
            mb.Entity<Produto>().Property(c=>c.Nome)
                                  .HasMaxLength(100)
                                  .IsRequired();

            mb.Entity<Produto>().Property(c=>c.Descricao)
                                  .HasMaxLength(200)
                                  .IsRequired();
            mb.Entity<Produto>().Property(c=>c.Preco).HasPrecision(14,2);

            // Relacinamento
            mb.Entity<Produto>().HasOne(c => c.Categoria)
                                .WithMany(p => p.Produtos)
                                .HasForeignKey(c => c.CategoriaId);
        }
    }
}