using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPP.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ProyectoPP.Controllers
{
    public class sprintsController : Controller
    {
        private patopurificEntitiesGeneral db = new patopurificEntitiesGeneral();

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

        private async Task<bool> revisarPermisos(string permiso)
        {

            string userName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = UserManager.FindByName(userName);
            var rol = user.Roles.SingleOrDefault().RoleId;

            var permisoID = db.permisos.Where(m => m.permiso == permiso).First().id_permiso;
            var listaRoles = db.AspNetRoles.Where(m => m.permisos.Any(c => c.id_permiso == permisoID)).ToList().Select(n => n.Id);

            bool userRol = false;
            foreach (var element in listaRoles)
            {
                if (element == rol)
                    userRol = true;
            }
            return userRol;
        }

        // GET: sprints
        public ActionResult Index()
        {

            Sprint2 modelo = new Sprint2();

            if ( !System.Web.HttpContext.Current.User.IsInRole("Estudiante")) // Si el usuario no es estudiante
            {

                // Seleccion para el dropdown de proyectos. Carga todos los proyectos que hay
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre", "Seleccione un Proyecto");
            }

            else
            {
                var idproyecto = db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).First().IdProyecto;                

                // Seleccion para el dropdown de proyectos. Carga solo el proyecto donde participa el estudiante
                ViewBag.Proyecto = new SelectList(db.proyecto.Where(x => x.id == idproyecto), "id", "nombre");
                ViewBag.NombreProyecto = db.proyecto.Where(m => m.id == idproyecto).First().nombre;

            }
            return View(modelo);
        }

        public ActionResult SprintPlanning()
        {

            DatosSprintPlanning modelo = new DatosSprintPlanning();

            if (!System.Web.HttpContext.Current.User.IsInRole("Estudiante")) // Si el usuario no es estudiante
            {

                // Seleccion para el dropdown de proyectos. Carga todos los proyectos que hay
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre");

                // El dropdown de sprints queda en blanco porque no sabemos aún cuál proyecto se va a seleccionar. Para esto solamente busco los sprints donde el id es igual ""
                ViewBag.Sprint = new SelectList(db.sprint.Where(x => x.proyectoId == ""), "id", "id");
                
                // Repito el proceso con las HU, busco las que tengan ID ""
                ViewBag.HU = db.historiasDeUsuario.Where(x => x.proyectoId == "").ToList();
               
            }

            else
            {
                var idproyecto = db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).First().IdProyecto;
                
                // Seleccion para el dropdown de proyectos. Carga solo el proyecto donde participa el estudiante
                ViewBag.Proyecto = new SelectList(db.proyecto.Where(x => x.id == idproyecto), "id", "nombre");

                // Seleccion para el dropdown de sprints. Carga todos los sprints que hay asociados al proyecto seleccionado
                ViewBag.Sprint = new SelectList(db.sprint.Where(x => x.proyectoId == idproyecto), "id", "id");

                // Lss hitorias de usuario no se cargan ya que necesito seleccionar un sprint
                ViewBag.HU = db.historiasDeUsuario.Where(x => x.proyectoId == "").ToList();

            }
            modelo.fechaC = db.fechas.ToList();
            return View(modelo);
        }

        // GET: sprints/Details/5
        public ActionResult Details(string id, string proyectoId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sprint sprint = db.sprint.Find(id, proyectoId);
            Sprint2 s = new Sprint2();
            s.fechaInicio = sprint.fechaInicio;


            /*string[] fecha = sprint.fechaFinal.ToString().Split('/');
            int año = Int32.Parse(fecha[2]);

            s.fechaFinal = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[1])  ); */
            s.fechaFinal = sprint.fechaFinal;          
            s.id = sprint.id;
            s.proyectoId = sprint.proyectoId;

            ViewBag.nombreProyecto = db.proyecto.Where(p => p.id == proyectoId).FirstOrDefault().nombre;

            if (sprint == null)
            {
                return HttpNotFound();
            }
            return View(s);
        }

        // GET: sprints/Create
        
        public ActionResult Create(string proyectoId)
        {
            if (proyectoId != null)
            {

                ViewBag.nombreProyecto = db.proyecto.Where(p => p.id == proyectoId).First().nombre;

                //autogenero el numero de sprint
                var max = db.sprint.Where(s => s.proyectoId == proyectoId).Max(s => s.id);
                if (max == null) max = "0";
                int n = Int32.Parse(max);
                n = n + 1;

                //asigno valores que ya deben de ir por defecto y no se pueden modificar
                ViewBag.proyectoId = proyectoId;
                ViewBag.id = "" + n;
                
                Sprint2 mod = new Sprint2();
                mod.proyectoId = proyectoId;
                mod.nombreProyecto = db.proyecto.Where(p => p.id == proyectoId).First().nombre;
                mod.id = "" + n;


                return View(mod);
            }
            TempData["msg"] = "<script>alert('Primero seleccione un pryecto');</script>";
            return RedirectToAction("Index");
        }

        // POST: sprints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Sprint2 sprint)
        {
            sprint nuevoSprint = new sprint();
            //nuevoSprint.historiasDeUsuario = sprint.historiasDeUsuario;
            nuevoSprint.id = sprint.id;
            nuevoSprint.fechaInicio = sprint.fechaInicio;
            nuevoSprint.fechaFinal = sprint.fechaFinal;
            nuevoSprint.proyectoId = sprint.proyectoId;
            nuevoSprint.proyecto = sprint.proyecto;
            db.sprint.Add(nuevoSprint);
                try
                {
                    

                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    TempData["msg"] = "<script>alert('Ha ocurrido un error al crear el sprint');</script>";

                    return View(sprint);
                }

            //ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", sprint.proyectoId);
            //return View(sprint);
        }

        public ActionResult CreateHito()
        {
            return View();

            TempData["msg"] = "<script>alert('Primero seleccione un pryecto');</script>";
            return RedirectToAction("Index");
        }

        // POST: sprints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHito(fechas fecha)
        {
            //sprint nuevoSprint = new sprint();
            //nuevoSprint.historiasDeUsuario = sprint.historiasDeUsuario;
            //nuevoSprint.id = sprint.id;
            //nuevoSprint.fechaInicio = sprint.fechaInicio;
            //nuevoSprint.fechaFinal = sprint.fechaFinal;
            //nuevoSprint.proyectoId = sprint.proyectoId;
            //nuevoSprint.proyecto = sprint.proyecto;
            db.fechas.Add(fecha);
            try
            {
                db.SaveChanges();
                return RedirectToAction("SprintPlanning");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["msg"] = "<script>alert('Ha ocurrido un error al crear el sprint');</script>";

                return View();
            }

            //ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", sprint.proyectoId);
            //return View(sprint);
        }

        // GET: sprints/Edit/5
        public ActionResult Edit(string id, string proyectoId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // se busca en la base de datos el sprinta editar
            sprint sprintTMP = db.sprint.Find(id, proyectoId);
            Sprint2 sprint = new Sprint2();

            // en caso de que no lo encuentre
            if (sprintTMP == null)
            {
                return HttpNotFound();
            }

            // Aqui se le asignan los campos a una clase que no es autogenerada para poder asi editar los nombres desplegados
            sprint.proyectoId = sprintTMP.proyectoId;
            sprint.id = sprintTMP.id;
            sprint.fechaInicio = sprintTMP.fechaInicio;

            /*string[] fecha = sprintTMP.fechaFinal.ToString().Split('/');

            sprint.fechaFinal = new DateTime(Int32.Parse(fecha[0]), Int32.Parse(fecha[1]), Int32.Parse(fecha[2]) );*/
            sprint.fechaFinal = sprintTMP.fechaFinal;

            
           // ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", sprint.proyectoId);
            return View(sprint);
        }

        // POST: sprints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fechaInicio,fechaFinal,proyectoId")] sprint sprint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sprint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", sprint.proyectoId);
            return View(sprint);
        }

        // GET: sprints/Delete/5
        public ActionResult Delete(string id, string proyectoId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sprint sprint = db.sprint.Find(id,proyectoId);
            if (sprint == null)
            {
                return HttpNotFound();
            }
            return View(sprint);
        }

        // POST: sprints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string proyectoId)
        {
            sprint sprint = db.sprint.Find(id, proyectoId);
            db.sprint.Remove(sprint);
            db.SaveChanges();
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



        /** Metodo para actualizar la vista una vez seleccionado un un proyecto*/
        public ActionResult Actualizar(Sprint2 modelo)
        {
            // Si el usuario no es estudiante
            if (!System.Web.HttpContext.Current.User.IsInRole("Estudiante")) 
            {
                ViewBag.Sprints = db.sprint.Where(m => m.proyectoId == modelo.proyectoId).ToList();
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre", modelo.proyectoId);
            }
            // Si el usuario es estudiante
            else
            {
                ViewBag.Sprints = new SelectList(db.sprint.Where(x => x.id == modelo.id), "id", "fechaInicio", "fechaFinal", "proyectoId", modelo.id);
            }

            //modelo.ListaPB = (from H in db.historiasDeUsuario where H.proyectoId == modelo.ProyectoID select H).ToList();

            return View("Index", modelo);
        }

        public ActionResult ActualizarSprintPlanning(DatosSprintPlanning modelo)
        {

            if (!System.Web.HttpContext.Current.User.IsInRole("Estudiante")) // Si el usuario no es estudiante
            {

                // Seleccion para el dropdown de proyectos. Carga todos los proyectos que hay
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre", modelo.ProyectoId);

                // El dropdown de sprints carga solamente los sprints asociados al proyecto seleccionado
                ViewBag.Sprint = new SelectList(db.sprint.Where(x => x.proyectoId == modelo.ProyectoId), "id", "id", modelo.SprintID);
                
                // El dropdonw de las hisotrias de usuario se hace con la combinación de ambas cosas
                ViewBag.HU = db.historiasDeUsuario.Where(x => x.proyectoId == modelo.ProyectoId && x.sprintId == modelo.SprintID).ToList();

            }

            else
            {
                var idproyecto = db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).First().IdProyecto;

                // Seleccion para el dropdown de proyectos. Carga solo el proyecto donde participa el estudiante
                ViewBag.Proyecto = new SelectList(db.proyecto.Where(x => x.id == idproyecto), "id", "nombre", modelo.ProyectoId);

                // Seleccion para el dropdown de sprints. Carga todos los sprints que hay asociados al proyecto seleccionado
                ViewBag.Sprint = new SelectList(db.sprint.Where(x => x.proyectoId == idproyecto), "id", "id", modelo.SprintID);

                ViewBag.HU = db.historiasDeUsuario.Where(x => x.proyectoId == idproyecto && x.sprintId == modelo.SprintID).ToList();

            }

            return View("SprintPlanning", modelo);
        }
    }
}
