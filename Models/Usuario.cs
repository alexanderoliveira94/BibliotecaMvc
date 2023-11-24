using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaMvc.Models
{
    public class Usuario
    {
        
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; } = DateTime.Now; 
    }
}