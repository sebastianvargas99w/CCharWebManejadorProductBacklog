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
using System.Data.Entity.Validation;

using System.IO;
using System.Configuration;
using System.Data.SqlClient;


namespace ProyectoPP.Controllers
{
    public class historiasDeUsuariosController : Controller
    {
        private patopurificEntitiesGeneral db = new patopurificEntitiesGeneral();

        private ApplicationUserManager _userManager;
        private ProyectoPP.Models.patopurificEntitiesRoles enRoles = new ProyectoPP.Models.patopurificEntitiesRoles();

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


        // GET: historiasDeUsuarios
        public ActionResult Index()
        {
            ModeloProductBacklog modelo = new ModeloProductBacklog();

            if (revisarPermisos("Ver proyecto").Result) // Si el usuario no es estudiante
            {

                // Seleccion para el dropdown de proyectos. Carga todos los proyectos que hay
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre", "Seleccione un Proyecto");
            }

            else
            {
                var idproyecto = db.persona.Where(m => m.cedula == System.Web.HttpContext.Current.User.Identity.Name).First().IdProyecto;
                modelo.ListaPB = db.historiasDeUsuario.Where(m => m.proyectoId == idproyecto).ToList();

                // Seleccion para el dropdown de proyectos. Carga solo el proyecto donde participa el estudiante
                ViewBag.Proyecto = new SelectList(db.proyecto.Where(x => x.id == idproyecto), "id", "nombre");
                ViewBag.NombreProyecto = db.proyecto.Where(m => m.id == idproyecto).First().nombre;

            }
            return View(modelo);
        }

        // GET: historiasDeUsuarios/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloProductBacklog modelo = new ModeloProductBacklog();

            modelo.Hu = db.historiasDeUsuario.Find(id);

            if (modelo.Hu == null)
            {
                return HttpNotFound();
            }
            else
            {
                modelo.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == id).ToList();

            }
            if (!(GetFiles(id).Count == 0))
                modelo.Documento12 = GetFiles(id).First();
            return View(modelo);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase postedFile, String cHUid)
        {
            var docId = db.Documentacion.Count();

            docId++;

            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO Documentacion VALUES (@id, @nombre, @ContentType, @Data, @HUid)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", docId);
                    cmd.Parameters.AddWithValue("@nombre", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", postedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    cmd.Parameters.AddWithValue("@HUid", cHUid);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            //////////////////////////////////////////////////////////

            ModeloProductBacklog modelo = new ModeloProductBacklog();

            modelo.Hu = db.historiasDeUsuario.Find(cHUid);

            if (modelo.Hu == null)
            {
                return HttpNotFound();
            }
            else
            {
                modelo.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == cHUid).ToList();

            }
            if (!(GetFiles(cHUid).Count == 0))
                modelo.Documento12 = GetFiles(cHUid).First();

            return View(viewName: "Details", model: modelo);
        }

        [HttpPost]
        public ActionResult DownloadFile(int? fileId)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT nombre, Data, ContentType FROM Documentacion WHERE id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["ContentType"].ToString();
                        fileName = sdr["nombre"].ToString();
                    }
                    con.Close();
                }
            }

            return File(bytes, contentType, fileName);
        }

        [HttpPost]
        public ActionResult DeleteFile(int? fileId, String cHUid)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "DELETE FROM Documentacion WHERE id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            //////////////////////////////////////////////////////////

            ModeloProductBacklog modelo = new ModeloProductBacklog();

            modelo.Hu = db.historiasDeUsuario.Find(cHUid);

            if (modelo.Hu == null)
            {
                return HttpNotFound();
            }
            else
            {
                modelo.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == cHUid).ToList();

            }
            if (!(GetFiles(cHUid).Count == 0))
                modelo.Documento12 = GetFiles(cHUid).First();

            return View(viewName: "Details", model: modelo);
        }

        private static List<ProyectoPP.Models.DocumentacionModel> GetFiles(String cHUid)
        {
            List<ProyectoPP.Models.DocumentacionModel> files = new List<ProyectoPP.Models.DocumentacionModel>();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id, nombre FROM Documentacion Where HUid = '" + cHUid+"'"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Models.DocumentacionModel
                            {
                                Id = Convert.ToInt32(sdr["id"]),
                                Nombre = sdr["nombre"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return files;
        }

        public ActionResult DetallesCriterios(string idHU, int id)
        {
            criteriosDeAceptacion modelo = new criteriosDeAceptacion();
            modelo = db.criteriosDeAceptacion.Find(idHU, id);
            if (modelo == null)
            {
                return HttpNotFound();
            }
            return View(modelo);
        }

        //Get de editar criterios de aceptación
        public ActionResult EditarCriterio(int id)
        {
            return View(db.criteriosDeAceptacion.Where(m => m.numCriterio == id).ToList().First());
        }

        //Get de eliminar criterios de aceptacion
        public ActionResult EliminarCriterio(int id)
        {
            return View(db.criteriosDeAceptacion.Where(m => m.numCriterio == id).ToList().First());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarCriterio(criteriosDeAceptacion criterio)
        {
            criteriosDeAceptacion modelo = db.criteriosDeAceptacion.Where(m => m.numCriterio == criterio.numCriterio).ToList().First();
            db.criteriosDeAceptacion.Remove(modelo);
            db.SaveChanges();

            ModeloProductBacklog redirect = new ModeloProductBacklog();
            redirect.Hu = db.historiasDeUsuario.Find(modelo.idHU);
            redirect.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == modelo.idHU).ToList();
            return View(viewName: "Details", model: redirect);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCriterio(criteriosDeAceptacion criterio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(criterio).State = EntityState.Modified;
                db.SaveChanges();

                ModeloProductBacklog redirect = new ModeloProductBacklog();
                redirect.Hu = db.historiasDeUsuario.Find(criterio.idHU);
                redirect.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == criterio.idHU).ToList();
                return View(viewName: "Details", model: redirect);
            }
            return View(db.criteriosDeAceptacion.Where(m => m.numCriterio == criterio.numCriterio).ToList().First());
        }


        [HttpPost, ActionName("CreaarCriterio")]
        public ActionResult CreaarCriterio(criteriosDeAceptacion criterio)
        {
            try
            {
                criterio.numCriterio = db.criteriosDeAceptacion.Max(m => m.numCriterio) + 1;
            }
            catch (System.InvalidOperationException)
            {
                criterio.numCriterio = 1;
            }
            db.criteriosDeAceptacion.Add(criterio);
            try
            {
                db.SaveChanges();

                if (criterio.idHU == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModeloProductBacklog modelo = new ModeloProductBacklog();

                modelo.Hu = db.historiasDeUsuario.Find(criterio.idHU);

                if (modelo.Hu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    modelo.Criterios = db.criteriosDeAceptacion.Where(m => m.idHU == criterio.idHU).ToList();
                }

                return View(viewName: "Details", model: modelo);

            }
            catch (DbEntityValidationException ex)
            {

            }

            return View();
        }




        //GET: historiasDeUsuario/CrearCiterio
        public ActionResult CrearCriterio(string hu)
        {
            criteriosDeAceptacion modelo = new criteriosDeAceptacion();


            modelo.idHU = hu;
            modelo.nombre = "";
            modelo.numCriterio = 0;
            modelo.numeroEscenario = 0;
            modelo.resultado = "";
            modelo.contexto = "";
            modelo.evento = "";
            modelo.historiasDeUsuario = db.historiasDeUsuario.Find(hu);

            return View(modelo);
        }

        



            // GET: historiasDeUsuarios/Create
        public ActionResult Create(string ProyectoId)
        {
            if (ProyectoId != null)
            {

                //Le pasamos como parametro a la vista el nombre del proyecto
                ViewBag.proyectoId = ProyectoId;
                ViewBag.nombreProyecto = db.proyecto.Where(p => p.id == ProyectoId).First().nombre.ToString();
                ViewBag.sprintId = new SelectList(db.sprint, "id", "id");
                return View();
            }
            return RedirectToAction("Index", "historiasDeUsuarios");



        }

        

        // POST: historiasDeUsuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HUConIdSeparado historiasDeUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    historiasDeUsuario nuevaHU = new Models.historiasDeUsuario();
                    // se deja como 0 en un caso default
                    if (historiasDeUsuario.numSprint == null)
                    {
                        historiasDeUsuario.numSprint = "0";
                    }


                    /*string query = "SELECT id"
                                 + "FROM historiasDeUsuario "
                                 + "WHERE Discriminator = 'Student' "
                                 + "GROUP BY EnrollmentDate";
                    IEnumerable<EnrollmentDateGroup> data = db.Database.SqlQuery<EnrollmentDateGroup>(query);*/


                    nuevaHU.id = "" + historiasDeUsuario.tipoDeRequerimiento + "-" + historiasDeUsuario.numSprint + "-" + historiasDeUsuario.modulo + "-" + historiasDeUsuario.numHU;
                    nuevaHU.rol = historiasDeUsuario.rol;
                    nuevaHU.funcionalidad = historiasDeUsuario.funcionalidad;
                    nuevaHU.resultado = historiasDeUsuario.resultado;
                    nuevaHU.prioridad = historiasDeUsuario.prioridad;
                    nuevaHU.estimacion = historiasDeUsuario.estimacion;
                    nuevaHU.NumeroEscenario = historiasDeUsuario.NumeroEscenario;
                    nuevaHU.proyectoId = historiasDeUsuario.proyectoId;

                    nuevaHU.NumeroEscenario = historiasDeUsuario.NumeroEscenario;

                    db.historiasDeUsuario.Add(nuevaHU);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    TempData["msg"] = "<script>alert('Ha ocurrido un error al crear la historia de usuario');</script>";

                    return View(historiasDeUsuario);
                }
            }

            ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", historiasDeUsuario.proyectoId);
            //ViewBag.sprintId = new SelectList(db.sprint, "id", "proyectoId", nuevaHU.sprintId);
            return View(historiasDeUsuario);
        }

        // GET: historiasDeUsuarios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            historiasDeUsuario historiasDeUsuario = db.historiasDeUsuario.Find(id);
            if (historiasDeUsuario == null)
            {
                return HttpNotFound();
            }
            //Para tener una dicion más facil separo el Id de la historia de usuario

            HUConIdSeparado HU = new HUConIdSeparado();
            string[] segmentosID = historiasDeUsuario.id.ToString().Split('-');
            HU.tipoDeRequerimiento = segmentosID[0];
            HU.numSprint = segmentosID[1];
            HU.modulo = segmentosID[2];
            HU.numHU = segmentosID[3];

            HU.rol = historiasDeUsuario.rol;
            HU.funcionalidad = historiasDeUsuario.funcionalidad;
            HU.resultado = historiasDeUsuario.resultado;
            HU.prioridad = historiasDeUsuario.prioridad;
            HU.estimacion = historiasDeUsuario.estimacion;

            HU.id = id;
            ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", historiasDeUsuario.proyectoId);
            ViewBag.sprintId = new SelectList(db.sprint, "id", "id", historiasDeUsuario.sprintId);
            return View(HU);
        }

        // POST: historiasDeUsuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HUConIdSeparado historiasDeUsuario)
        {
            if (ModelState.IsValid)
            {
                historiasDeUsuario nuevaHU = new Models.historiasDeUsuario();
                // se deja como 0 en un caso default
                if (historiasDeUsuario.numSprint == null)
                {
                    historiasDeUsuario.numSprint = "0";
                }
                nuevaHU.id = "" + historiasDeUsuario.tipoDeRequerimiento + "-" + historiasDeUsuario.numSprint + "-" + historiasDeUsuario.modulo + "-" + historiasDeUsuario.numHU;
                nuevaHU.rol = historiasDeUsuario.rol;
                nuevaHU.funcionalidad = historiasDeUsuario.funcionalidad;
                nuevaHU.resultado = historiasDeUsuario.resultado;
                nuevaHU.prioridad = historiasDeUsuario.prioridad;
                nuevaHU.estimacion = historiasDeUsuario.estimacion;
                nuevaHU.NumeroEscenario = historiasDeUsuario.NumeroEscenario;
                nuevaHU.proyectoId = historiasDeUsuario.proyectoId;

                db.Entry(historiasDeUsuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.proyectoId = new SelectList(db.proyecto, "id", "nombre", historiasDeUsuario.proyectoId);
            ViewBag.sprintId = new SelectList(db.sprint, "id", "id", historiasDeUsuario.numSprint);
            return View(historiasDeUsuario);
        }

        // GET: historiasDeUsuarios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            historiasDeUsuario historiasDeUsuario = db.historiasDeUsuario.Find(id);
            if (historiasDeUsuario == null)
            {
                return HttpNotFound();
            }
            return View(historiasDeUsuario);
        }

        // POST: historiasDeUsuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            historiasDeUsuario historiasDeUsuario = db.historiasDeUsuario.Find(id);
            db.historiasDeUsuario.Remove(historiasDeUsuario);
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
        [HttpPost]
        public ActionResult Actualizar(ModeloProductBacklog modelo)
        {
            if (revisarPermisos("Ver proyecto").Result) // Si el usuario no es estudiante
            {
                ViewBag.Proyecto = new SelectList(db.proyecto, "id", "nombre", modelo.ProyectoID);
            }
            else
            {
                ViewBag.Proyecto = new SelectList(db.proyecto.Where(x => x.id == modelo.ProyectoID), "id", "nombre");
            }

            modelo.ListaPB = (from H in db.historiasDeUsuario where H.proyectoId == modelo.ProyectoID select H).ToList();

            return View("Index", modelo);
        }
    }
}
