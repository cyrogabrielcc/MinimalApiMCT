using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        {}
    }
}