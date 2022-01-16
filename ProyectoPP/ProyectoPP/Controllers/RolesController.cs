using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoPP.Models;
using System.Data.Entity;

using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;



namespace ProyectoPP.Controllers
{
    public class RolesController : Controller
    {

        patopurificEntitiesRoles baseDatos = new patopurificEntitiesRoles();
        
        // GET: Roles

        private ProyectoPP.Models.patopurificEntitiesRoles enRoles = new ProyectoPP.Models.patopurificEntitiesRoles();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //verifica si los usuarios tienen permisos para acceder a ventana.
        private async Task<bool> revisarPermisos(string permiso)
        {

            string userName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = UserManager.FindByName(userName);
            var rol = user.Roles.SingleOrDefault().RoleId;

            var permisoID = enRoles.permisos.Where(m => m.permiso == permiso).First().id_permiso;
            var listaRoles = enRoles.AspNetRoles.Where(m => m.permisos.Any(c => c.id_permiso == permisoID)).ToList().Select(n => n.Id);

            bool userRol = false;
            foreach (var element in listaRoles)
            {
                if (element == rol)
                    userRol = true;
            }
            return userRol;
        }

        public string EsEstudiante()
        {

            string userName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = UserManager.FindByName(userName);
            var rol = user.Roles.SingleOrDefault().RoleId;
                     
            return rol;
        }

        //envía un valor diferente dependiendo de si el usuario tiene permiso o no.
        public string RevisarPermisosB(string permiso)
        {
            string respuesta="0";
            if (this.revisarPermisos(permiso).Result)
            {
                respuesta = "1";
            }
            return respuesta;
        }

        //Le envía la información necesaria a la vista para que se le presente al usuario
        public ActionResult RolesView()
        {
            Roles modelo = new Roles();
            if (revisarPermisos("ver accesos").Result)
            {
                //se copia la información desde la base de datos en el modelo
                modelo.ListaRoles = baseDatos.AspNetRoles.ToList();
                modelo.ListaPermisos = baseDatos.permisos.ToList();
                modelo.ListaAscociaciones = new List<Roles.Asociaciones>();
                modelo.ListaGuardar = new List<Roles.GuardarAux>();

                //Añade todos los roles con sus respectivos permisos al modelo, este modelo se usa sólo para cargar la
                //información en la pantalla.
                foreach (var AspNetRoles in modelo.ListaRoles)
                {

                    foreach (var permisos in AspNetRoles.permisos)
                    {
                        modelo.ListaAscociaciones.Add(new Roles.Asociaciones(AspNetRoles.Name, permisos.permiso));
                    }
                }

                //Prepara el modelo que se utilizará para guardar la información en la base de datos.
                foreach (var AspNetRoles in modelo.ListaRoles)
                {
                    foreach (var permisos in modelo.ListaPermisos)
                    {
                        bool asignado = false;
                        for (int contador = 0; contador < modelo.ListaAscociaciones.Count(); contador++)
                        {
                            if (AspNetRoles.Name == modelo.ListaAscociaciones.ElementAt(contador).rol)
                            {
                                if (modelo.ListaAscociaciones.ElementAt(contador).permiso == permisos.permiso)
                                {
                                    modelo.ListaGuardar.Add(new Roles.GuardarAux(AspNetRoles.Id, permisos.id_permiso, true));
                                    asignado = true;
                                }
                            }
                            if (contador == modelo.ListaAscociaciones.Count() - 1 && asignado == false)
                            {
                                modelo.ListaGuardar.Add(new Roles.GuardarAux(AspNetRoles.Id, permisos.id_permiso, false));
                                asignado = false;
                            }
                        }

                    }
                }
                return View(modelo);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Contiene la funcionalidad del botón "Aceptar" que guarda los cambios que el usuario le quiere hacer a la base de datos.
        [HttpPost]
        public ActionResult Aceptar(Roles mod)
        {   
            //Carga la información de la base de datos para luego modificarla.
            Roles modelo = new Roles();
            modelo.ListaRoles = baseDatos.AspNetRoles.ToList();
            modelo.ListaPermisos = baseDatos.permisos.ToList();

            //Actualiza las llaves foráneas de roles.
            int numero = -1;
            foreach (var rol in modelo.ListaRoles)
            {
                numero++;
                rol.permisos.Clear();
                foreach (var asoc in mod.ListaGuardar)
                {
                    if (asoc.rol == rol.Id)
                    {
                        if (asoc.existe == true)
                        {
                            foreach (var permiso in modelo.ListaPermisos)
                            {
                                if (permiso.id_permiso == asoc.permiso)
                                {
                                    modelo.ListaRoles.ElementAt(numero).permisos.Add(permiso);
                                }
                            }
                            
                        }
                    }
                }
            }

            //Actualiza las llaves foráneas de permisos.
            int contador = -1;
            foreach (var permiso in modelo.ListaPermisos)
            {
                contador++;
                
                permiso.AspNetRoles.Clear();
                foreach (var asoc in mod.ListaGuardar)
                {
                    if (asoc.permiso == permiso.id_permiso)
                    {
                        if (asoc.existe == true)
                        {
                            foreach (var rol in modelo.ListaRoles)
                            {
                                if (rol.Id == asoc.rol)
                                {
                                    modelo.ListaPermisos.ElementAt(contador).AspNetRoles.Add(rol);
                                }
                            }
                        }

                    }

                }
            }
            


            if (ModelState.IsValid)
            {
                //Guarda los cambios de la actualización de la base de datos.
                foreach (var permiso in modelo.ListaPermisos)
                {   
                    baseDatos.Entry(permiso).State = EntityState.Modified;

                }
                foreach (var roles in modelo.ListaRoles)
                {
                    baseDatos.Entry(roles).State = EntityState.Modified;
                }
                baseDatos.SaveChanges();
                //Le notifica al usuario que la información se guardó exitosamente.
                TempData["msg"] = "<script>alert('Se asignaron los permisos a sus respectivos roles correctamente.');</script>";
                return RedirectToAction("RolesView");
            }
            return View(modelo);
        }
    }
}