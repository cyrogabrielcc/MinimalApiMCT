using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set;}
        public string Descricao { get; set; }
        public ICollection<Produto> Produtos { get; set; }
    }
}