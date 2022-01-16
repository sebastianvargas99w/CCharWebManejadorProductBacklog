using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPP.Models
{
    public class UsuarioRol
    {
        public List<persona> ListaPeronas { get; set; }
        public List<AspNetRoles> ListaRoles { get; set; }
    }
}
