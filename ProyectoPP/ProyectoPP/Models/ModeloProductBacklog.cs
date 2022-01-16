using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPP.Models
{
    public class ModeloProductBacklog
    {
        public ModeloProductBacklog()
        {
            ListaPB = new List<historiasDeUsuario>();
            Criterios = new List<criteriosDeAceptacion>();
        }
        public List<historiasDeUsuario> ListaPB{ get; set; }

        public List<criteriosDeAceptacion> Criterios { get; set; }

        public historiasDeUsuario Hu { get; set;}

        [Display(Name = "Nombre del Proyecto")]
        public string ProyectoID { get; set; }

        public DocumentacionModel Documento12 { get; set; }
    }
}