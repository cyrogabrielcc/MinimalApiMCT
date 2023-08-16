using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Models
{
    public class Tarefa
    {
        public int TarefaId { get; set; }
        public string Nome { get; set; }
        public bool IsConcluida { get; set; }
    }
}