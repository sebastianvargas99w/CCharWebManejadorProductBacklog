using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProyectoPP.Models
{
        public class PersonaConRol
        {
            [Display(Name = "Nombre")]
            [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El nombre solo puede estar compuesto por letras.")]
        [Required(ErrorMessage = "Campo requerido")]
        public string nombre { get; set; }

            [Display(Name = "Primer apellido")]
            [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El primer apellido solo puede estar compuesto por letras")]
        [Required(ErrorMessage = "Campo requerido")]
        public string apellido1 { get; set; }

            [Display(Name = "Segundo apellido")]
            [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El segundo apellido solo puede estar compuesto por letras")]
        [Required(ErrorMessage = "Campo requerido")]
        public string apellido2 { get; set; }

            [RegularExpression(@"^[0-9]{9,9}$", ErrorMessage = "La cédula debe contener 9 numeros")]
            [Display(Name = "Cédula")]
        [Required(ErrorMessage = "Campo requerido")]
        public string cedula { get; set; }

            [RegularExpression(@"^[A-Z][0-9]{5,5}$", ErrorMessage = "La primera letra debe estar en mayúscula y contener 5 digitos despues de esta")]
            [Display(Name = "Carné")]
            public string carne { get; set; }

            [RegularExpression(@"^[a-zA-Z0-9\.\-]+@[a-zA-Z0-9\.\-]+\.[a-z]{1,3}$", ErrorMessage = "No es un formato de correo electronico válido")]
            [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Campo requerido")]
        public string email { get; set; }

            public string id { get; set; }

            [Display(Name = "Rol")]
            public string rol { get; set; }
        }

    }