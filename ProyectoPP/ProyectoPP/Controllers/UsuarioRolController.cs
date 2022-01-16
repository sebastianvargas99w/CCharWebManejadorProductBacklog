using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoPP.Models
{
    public class UsuarioRolController : Controller
    {
        patopurificEntitiesUserRoles db = new patopurificEntitiesUserRoles();
        // GET: UsuarioRol
        public ActionResult UsuarioRol()
        {
            UsuarioRol modelo = new UsuarioRol();
            //modelo.ListaPeronas = db.persona.ToList();
            //modelo.ListaRoles = db.AspNetRoles.ToList();
            return View(modelo);
        }

    }
}