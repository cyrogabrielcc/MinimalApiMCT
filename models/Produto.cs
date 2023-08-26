using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinimalApi.models
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set;}
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public DateTime DatacCompra { get; set; }
        public int Estoque { get; set; }

        // propriedades de navegacao
        public int CategoriaId { get; set; }
        
        [JsonIgnore]
        public Categoria Categoria{ get; set; }
    }
}