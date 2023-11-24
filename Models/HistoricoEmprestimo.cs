using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaMvc.Models
{
    public class HistoricoEmprestimo
    {
        public int IdUsuario { get; set; }
        public List<HistoricoEmprestimo> ?ListaDeEmprestimo { get; set; }
    }
}