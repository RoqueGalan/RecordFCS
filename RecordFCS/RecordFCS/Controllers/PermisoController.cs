﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;
using RecordFCS.Helpers.Seguridad;

namespace RecordFCS.Controllers
{
    public class PermisoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Permiso
        [CustomAuthorize]
        public ActionResult Lista(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            //lista de todos los tipos de permisos que sean Status Verdadero

            var listaPermisos = new List<Permiso>();
            foreach (var item in db.TipoPermisos.Where(a => a.Status).ToList())
            {
                Permiso permiso = new Permiso()
                {
                    Usuario = usuario,
                    UsuarioID = usuario.UsuarioID,
                    TipoPermiso = item,
                    TipoPermisoID = item.TipoPermisoID
                };

                if (usuario.Permisos.Where(a => a.TipoPermisoID == item.TipoPermisoID).Count() > 0)
                    permiso.Status = true;
                else
                    permiso.Status = false;

                listaPermisos.Add(permiso);
            }

            listaPermisos = listaPermisos.OrderBy(a=>a.TipoPermiso.Nombre).ToList();

            ViewBag.totalRegistros = usuario.Permisos.Count();

            if (!User.IsInRole("UsuarioPermisosEdit"))
            {
                listaPermisos = listaPermisos.Where(a => a.Status).ToList();
            }

            return PartialView("_Lista", listaPermisos);
        }

        // GET: Permiso/Details/5
        [CustomAuthorize(permiso = "UsuarioPermisosEdit")]
        public ActionResult CambiarStatus(Int64? id, Int64? TipoPermisoID, bool Estado)
        {
            var permiso = new Permiso()
            {
                UsuarioID = Convert.ToInt64(id),
                TipoPermiso = db.TipoPermisos.Find(TipoPermisoID),
                TipoPermisoID = Convert.ToInt64(TipoPermisoID),
                Status = Estado
            };

            if (Estado)
            {
                ViewBag.PagName = "Desactivar";
            }
            else
            {
                ViewBag.PagName = "Activar";
            }

            return PartialView("_CambiarStatus", permiso);
        }

        // POST: Permiso/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UsuarioPermisosEdit")]
        public ActionResult CambiarStatus([Bind(Include = "UsuarioID,TipoPermisoID,Status")] Permiso permiso)
        {
            var valPermiso = db.Permisos.Find(permiso.UsuarioID, permiso.TipoPermisoID);
            TipoPermiso tipoPermiso = db.TipoPermisos.Find(permiso.TipoPermisoID);

            if (valPermiso == null)
            {
                //no existe y hay que insertarlo
                permiso.Status = true;
                db.Permisos.Add(permiso);
                AlertaSuccess(string.Format("Permiso: <b>{0}</b> se ACTIVO.", tipoPermiso.Nombre), true);
            }
            else
            {
                //existe entonces hay que eliminarlo
                db.Permisos.Remove(valPermiso);
                AlertaWarning(string.Format("Permiso: <b>{0}</b> se DESACTIVO.", tipoPermiso.Nombre), true);

            }

            db.SaveChanges();

            string url = Url.Action("Lista", "Permiso", new { id = permiso.UsuarioID });
            return Json(new { success = true, url = url, modelo = "Permiso" }, JsonRequestBehavior.AllowGet);
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
