using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPP.Models
{
    public class HUConIdSeparado
    {
        [Display(Name = "Tipo de requerimiento")]
        [Required(ErrorMessage = "Campo requerido")]
        public string tipoDeRequerimiento { get; set; }

        [Display(Name = "Módulo")]
        [Required(ErrorMessage = "Campo requerido")]
        public string modulo { get; set; }

        [Display(Name = "Número de sprint")]
        [Required(ErrorMessage = "Campo requerido")]
        public string numSprint { get; set; }

        [Display(Name = "Número de H.U.")]
        [Required(ErrorMessage = "Campo requerido")]
        public string numHU { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Campo requerido")]
        public string rol { get; set; }

        [Display(Name = "Funcionalidad")]
        [Required(ErrorMessage = "Campo requerido")]
        public string funcionalidad { get; set; }

        [Display(Name = "Resultado")]
        [Required(ErrorMessage = "Campo requerido")]
        public string resultado { get; set; }

        [Display(Name = "Prioridad")]
        [Required(ErrorMessage = "Campo requerido")]
        public int prioridad { get; set; }

        [Display(Name = "Estimación")]
        [Required(ErrorMessage = "Campo requerido")]
        public int estimacion { get; set; }

        [Display(Name = "Nombre del Proyecto")]
        [Required(ErrorMessage = "Campo requerido")]
        public string proyectoId { get; set; }

        [Display(Name = "Número de escenario")]
        [Required(ErrorMessage = "Campo requerido")]
        public int NumeroEscenario { get; set; }
        [Display(Name = "Sprint Id")]
        [Required(ErrorMessage = "Campo requerido")]
        public string id { get; set; }



    }
}