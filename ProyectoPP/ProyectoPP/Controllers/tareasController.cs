using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPP.Models;

namespace ProyectoPP.Controllers
{
    public class tareasController : Controller
    {
        private patopurificEntitiesGeneral db = new patopurificEntitiesGeneral();

        // GET: tareas
        public ActionResult Index(string HU)
        {
            var tarea = db.tarea.Where(t => t.HU == HU);
            ViewBag.HUid = HU;
            return View(tarea.ToList());
        }

        // GET: tareas/Details/5
        public ActionResult Details(string HU, int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarea tarea = db.tarea.Find(HU, id);
            tarea.progreso = db.progreso.Where(t => t.HU == HU && t.id == id).ToList();
            if (tarea == null)
            {
                return HttpNotFound();
            }
            return View(tarea);
        }

        // GET: tareas/Create
        public ActionResult Create(string HU)
        {
            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol");
            tarea tarea = new tarea();
            tarea.HU = HU;
            return View(tarea);
        }

        // POST: tareas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tarea tarea)
        {
            if (ModelState.IsValid)
            {
                db.tarea.Add(tarea);
                db.SaveChanges();
                return RedirectToAction("Index", new { HU = tarea.HU });
            }

            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol", tarea.HU);

            return View();
        }

        public ActionResult addProgreso(string HU, int id)
        {
            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol");
            progreso progreso = new progreso();
            progreso.HU = HU;
            progreso.id = id;
            ViewBag.fechaCorte = new SelectList(db.fechas, "fechaCorte");
            return View(progreso);
        }

        // POST: tareas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addProgreso(progreso progreso)
        {
            if (ModelState.IsValid)
            {
                db.progreso.Add(progreso);
                db.SaveChanges();
                return RedirectToAction("Index", new { HU = progreso.HU });
            }

            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol", progreso.HU);

            return View();
        }

        // GET: tareas/Edit/5
        public ActionResult Edit(string HU, int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarea tarea = db.tarea.Find(HU, id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol", tarea.HU);
            return View(tarea);
        }

        // POST: tareas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tarea tarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { HU = tarea.HU });
            }
            ViewBag.HU = new SelectList(db.historiasDeUsuario, "id", "rol", tarea.HU);
            return View(tarea);
        }

        // GET: tareas/Delete/5
        public ActionResult Delete(string HU, int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarea tarea = db.tarea.Find(HU, id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            return View(tarea);
        }

        // POST: tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string HU, int id)
        {
            tarea tarea = db.tarea.Find(HU, id);
            db.tarea.Remove(tarea);
            db.SaveChanges();
            return RedirectToAction("Index", new { HU = tarea.HU });
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
