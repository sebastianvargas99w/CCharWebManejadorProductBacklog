using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPP.Models
{
    //Esta clase contiene los modelos de AspNetRoles, Permisos, dos clases que también se utilizan en la interfaz de Administación de roles.
    public class Roles
    {

        // Esta clase contiene los roles con sus permisos asociados si existe un objeto con un rol y un permiso entonces significa que el
        //rol tiene ese permiso, se usa para cargar la interfaz de administación de roles.
        public class Asociaciones
        {
            //Constructor de la clase
            public Asociaciones(string rolParam, string permisoParam)
            {
                this.rol = rolParam;
                this.permiso = permisoParam;
            }

            //Set y get de rol
            public string rol { get; set; }
            //Set y get de permiso
            public string permiso  { get; set; }
        }

        //Esta clase contiene todos los roles y todos los permisos, si "existe" es verdadero entonces significa que el rol tiene asignado ese
        //permiso.
        public class GuardarAux
        {
            //Constructor de la clase
            public GuardarAux(string rolParam, int permisoParam, bool existeParam)
            {
                this.rol = rolParam;
                this.permiso = permisoParam;
                this.existe = existeParam;

            }

            //Constructor de la clase
            public GuardarAux()
            {
            }
            //Set y get de rol
            public string rol { get; set; }
            //Set y get de permiso
            public int permiso { get; set; }
            //Set y get de existe
            public bool existe { get; set; }

        }

        public List<GuardarAux> ListaGuardar { get; set; }

        public permisos ModeloPermisos { get; set; }

        public AspNetRoles ModeloNetRoles { get; set; }

        public List<permisos> ListaPermisos { get; set; }

        public List<AspNetRoles> ListaRoles { get; set; }

        public List<Asociaciones>  ListaAscociaciones { get; set; }
}
}