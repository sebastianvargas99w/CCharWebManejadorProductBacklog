using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPP.Models
{
    public class DatosSprintPlanning
    {

        public string ProyectoId { get; set; }

        public string SprintID { get; set; }

        public virtual ICollection<fechas> fechaC { get; set; }
    }
}