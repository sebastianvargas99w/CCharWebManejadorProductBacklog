using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ProyectoPP.Controllers
{
    public class proyectoController : Controller
    {
        private patopurificEntitiesGeneral db = new patopurificEntitiesGeneral();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return UserManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // Carga la intefaz de Index del módulo de proyecto
        // GET: proyecto
        public ActionResult Index()
        {
            
            if (System.Web.HttpContext.Current.User.IsInRole("Estudiante")) // Si el usuario es estudiante
            {

                var idproyecto = db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).First().IdProyecto;
                ViewBag.NombreProyecto = db.proyecto.Where(m => m.id == idproyecto).First().nombre;
                var proyecto = db.proyecto.Where(p => p.id == idproyecto);
                return View(proyecto.ToList());
            }
            var proyectos = db.proyecto.Include(p => p.persona);

            return View(proyectos.ToList());
        }


        // Carga la intefaz de Details del módulo de proyecto
        // GET: proyecto/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proyecto proyecto = db.proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }

            ViewBag.Grupo = new SelectList(db.persona.Where(x => x.IdProyecto == id && x.cedula != proyecto.lider), "cedula", "nombre");
            return View(proyecto);
        }

        // Carga la intefaz de Create del módulo de proyecto
        // GET: proyecto/Create
        public ActionResult Create()
        {

            ViewBag.lider = new SelectList(db.persona.Where(x=>x.IdProyecto == null), "cedula", "nombre");
            return View();
        }

        //Guarda el nuevo proyecto en la base de datos.
        // POST: proyecto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(proyecto proyecto)
        {
            if (ModelState.IsValid)
            {

                proyecto.id = proyecto.nombre + proyecto.lider;

                var persona = db.persona.Find(proyecto.lider);
                persona.IdProyecto = proyecto.id;
                
                db.proyecto.Add(proyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.lider = new SelectList(db.persona.Where(x => x.IdProyecto == null), "cedula", "nombre");
            return View(proyecto);
        }

        //Guarda los cambios del proyecto en la base de datos.
        // GET: proyecto/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proyecto proyecto = db.proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            ViewBag.Listalider = new SelectList(db.persona.Where(x => x.IdProyecto == id), "cedula", "nombre", proyecto.persona1);
            ViewBag.ListaAgregar = new SelectList(db.persona.Where(x => x.IdProyecto == null), "cedula", "nombre");
            ViewBag.ListaQuitar = new SelectList(db.persona.Where(x => x.IdProyecto == id && x.cedula != proyecto.lider), "cedula", "nombre");
            return View(proyecto);
        }

        // POST: proyecto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                if (proyecto.listaAgregar != null) // Reviso si la lista es null para evitar errores
                {
                    for (int a = 0; a < proyecto.listaAgregar.Length; a++) // para cada elemento de la lista
                    {
                        // le asigno el proyecto a la persona
                        persona persona = db.persona.Find(proyecto.listaAgregar[a]);
                        persona.IdProyecto = proyecto.id;
                    }
                }

                if (proyecto.listaQuitar != null) // reviso si la lista es null para evitar errores
                {

                    for (int a = 0; a < proyecto.listaQuitar.Length; a++) // para cada elemento de la lista
                    {
                        // le quito el proyecto a la persona
                        persona persona = db.persona.Find(proyecto.listaQuitar[a]);
                        persona.IdProyecto = null;
                    }
                }

                db.Entry(proyecto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = proyecto.id});
            }
            ViewBag.lider = new SelectList(db.persona, "cedula", "nombre", proyecto.lider);
            return View(proyecto);
        }

        // Carga de interfaz de Delete del módulo de proyecto.
        // GET: proyecto/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proyecto proyecto = db.proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // Borra de la base de datos el proyecto que se seleccionó.
        // POST: proyecto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            proyecto proyecto = db.proyecto.Find(id);
            db.proyecto.Remove(proyecto);
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
    }
}
