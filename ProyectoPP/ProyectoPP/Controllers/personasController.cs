using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using ProyectoPP.Models;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Contexts;

namespace ProyectoPP.Controllers
{
    public class personasController : Controller
    {
        private patopurificEntities db = new patopurificEntities();
        private ProyectoPP.Models.patopurificEntitiesRoles enRoles = new ProyectoPP.Models.patopurificEntitiesRoles();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        // GET: personas
        public ActionResult Index()
        {
             // revisa si tiene permiso de ver una lista de usuarios
            if (revisarPermisos("Ver usuarios").Result)
                return View(db.persona.ToList().OrderBy(x=> x.nombre));
            else
                return View(db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).ToList());
        }

        // GET: personas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            persona personaTmp = db.persona.Find(id);

            PersonaConRol persona = new PersonaConRol();
            persona.apellido1 = personaTmp.apellido1;
            persona.apellido2 = personaTmp.apellido2;
            persona.nombre = personaTmp.nombre;
            persona.cedula = personaTmp.cedula;
            persona.carne = personaTmp.carne;
            persona.email = personaTmp.email;

            //Ahora obtenemos el ID de ASPNET para obtener el rol
            var user = UserManager.FindByName(persona.cedula);
            string ID = user.Id;

            var aspUser = UserManager.FindById(ID);
            var rol = aspUser.Roles.SingleOrDefault().RoleId;

            switch (rol)
            {
                case "1":
                    persona.rol ="Estudiante";
                break;

                case "2":
                    persona.rol ="Administrador";
                break;

                case "3":
                    persona.rol = "Asistente";
                break;

            }

            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: personas/Create
        public ActionResult Create()
        {
            if (revisarPermisos("Crear usuarios").Result)
                return View();
            else
                return RedirectToAction("Index", "personas");
        }

        // POST: personas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PersonaConRol persona)
        {
            if (ModelState.IsValid)
            {
                // se crea un aplication user para el aspnetuser
            var user = new ApplicationUser { UserName = persona.cedula, Email = persona.email };
            try
                {
                    //puesto que PersonaCrear posee más datos que los que necesita la base de datos se crea userentry
                    var userEntry = new persona();
                    userEntry.apellido1 = persona.apellido1;
                    userEntry.apellido2 = persona.apellido2;
                    userEntry.nombre = persona.nombre;
                    userEntry.cedula = persona.cedula;
                    userEntry.carne = persona.carne + "";
                    userEntry.email = persona.email;


                    //genera el password generico
                    string pass = "Ucr." + persona.cedula;
                    //metodo para crear el usuario con su contraseña de aspnetuser
                    var result = await UserManager.CreateAsync(user, pass);


                    if (result.Succeeded)
                    {
                        string ID = user.Id.ToString();
                        userEntry.id = ID;
                        db.persona.Add(userEntry);
                        db.SaveChanges();

                        var resultado = await this.UserManager.AddToRoleAsync(ID, persona.rol);

                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    await UserManager.DeleteAsync(user);

                    // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario

                    TempData["msg"] = "<script>alert('Ha ocurrido un error al crear al usuario');</script>";
                    return View(persona);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    TempData["msg"] = "<script>alert('Ha ocurrido un error al crear al usuario');</script>";

                    return View(persona);
                }
            }
            
            return View(persona);
        }

        // GET: personas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            persona personaTmp = db.persona.Find(id);

            PersonaConRol persona = new PersonaConRol();
            persona.apellido1 = personaTmp.apellido1;
            persona.apellido2 = personaTmp.apellido2;
            persona.nombre = personaTmp.nombre;
            persona.cedula = personaTmp.cedula;
            persona.carne = personaTmp.carne;
            persona.email = personaTmp.email;

            //Ahora obtenemos el ID de ASPNET para obtener el rol
            var user = UserManager.FindByName(persona.cedula);

            var rol = user.Roles.SingleOrDefault().RoleId;
            
            persona.rol = rol;
            
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: personas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nombre,apellido1,apellido2,cedula,carne,email,rol")] PersonaConRol pcr)
        {
            
            if (ModelState.IsValid)
            {

                persona persona = new persona();
                persona.nombre = pcr.nombre;
                persona.apellido1 = pcr.apellido1;
                persona.apellido2 = pcr.apellido2;
                persona.cedula = pcr.cedula;
                persona.carne = pcr.carne+"";
                persona.email = pcr.email;

                var user = UserManager.FindByName(persona.cedula);
                persona.id = user.Id;

                var rol = user.Roles.SingleOrDefault().RoleId;
                if (revisarPermisos("Editar usuarios").Result)
                {
                    switch (rol)
                    {
                        case "1":
                            rol = "Estudiante";
                            break;

                        case "2":
                            rol = "Profesor";
                            break;

                        case "3":
                            rol = "Asistente";
                            break;

                    }
                    var resultado = this.UserManager.RemoveFromRole(persona.id, rol);

                    UserManager.AddToRoles(persona.id, pcr.rol);
                }
                
                db.Entry(persona).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pcr);
        }

        // GET: personas/Delete/5
        public ActionResult Delete(string id)
        {
            
            if (revisarPermisos("Crear usuarios").Result)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                persona persona = db.persona.Find(id);
                if (persona == null)
                {
                    return HttpNotFound();
                }
                return View(persona);
            }
            else
                return RedirectToAction("Details", new { id = id });

        }

        // POST: personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            persona persona = db.persona.Find(id);
            var user = UserManager.FindById(db.persona.Find(id).id);
            UserManager.Delete(user);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        //** metodo para poder extraer el Nombre de la persona a partir de la cédula */
        /* requiere: cedula
         * modifica:nada
         * retorna: el nombre de la persona
         * */
        public string ObtenerNombre(string id)
        {
            string nombre;
            nombre = db.persona.Find(id).nombre;
            return nombre;
        }
    }
}
