using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaMvc.Models
{
    public class Livro
    {
        public int IdLivro { get; set;}
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool EstaDisponivel { get; set; } 
    }
}