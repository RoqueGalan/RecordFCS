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
    public class TipoPermisoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoPermiso
        [CustomAuthorize(permiso = "TipoPermisoVer")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: TipoPermiso/Lista
        [CustomAuthorize(permiso = "TipoPermisoVer")]
        public ActionResult Lista()
        {
            var tipoPermisos = db.TipoPermisos.OrderBy(to => to.Clave);

            ViewBag.totalRegistros = tipoPermisos.Count();

            return PartialView("_Lista", tipoPermisos.ToList());
        }

        // GET: TipoPermisos/Crear
        [CustomAuthorize(permiso = "TipoPermisoCrear")]
        public ActionResult Crear()
        {
            TipoPermiso tipoPermiso = new TipoPermiso();

            return PartialView("_Crear", tipoPermiso);
        }

        // POST: TipoPermisos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoPermisoCrear")]
        public ActionResult Crear([Bind(Include = "TipoPermisoID,Clave,Nombre,Descripcion,Status")] TipoPermiso tipoPermiso)
        {
            if (ModelState.IsValid)
            {
                //revalidar la clave
                if (db.TipoPermisos.Where(a => a.Clave == tipoPermiso.Clave).Count() > 0)
                {
                    ModelState.AddModelError("Clave", "Ya existe un registro con esta clave. Intenta con otro.");
                    return PartialView("_Crear", tipoPermiso);
                }

                db.TipoPermisos.Add(tipoPermiso);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Permiso: <b>{0}, {1}</b> se creo con exitó.", tipoPermiso.Clave, tipoPermiso.Nombre), true);
                string url = Url.Action("Lista", "TipoPermiso");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoPermiso);
        }

        // GET: TipoPermisos/Editar/5
        [CustomAuthorize(permiso = "TipoPermisoEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPermiso tipoPermiso = db.TipoPermisos.Find(id);
            if (tipoPermiso == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoPermiso);
        }

        // POST: TipoPermisos/Editar/5
        [CustomAuthorize(permiso = "TipoPermisoEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "TipoPermisoID,Clave,Nombre,Descripcion,Status")] TipoPermiso tipoPermiso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoPermiso).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Permiso: <b>{0}, {1}</b> se edito con exitó.", tipoPermiso.Clave, tipoPermiso.Nombre), true);
                string url = Url.Action("Lista", "TipoPermiso");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoPermiso);
        }

        // GET: TipoPermisos/Eliminar/5
        [CustomAuthorize(permiso = "TipoPermisoEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPermiso tipoPermiso = db.TipoPermisos.Find(id);
            if (tipoPermiso == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", tipoPermiso);
        }

        // POST: TipoPermisos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoPermisoEliminar")]
        public ActionResult DeleteConfirmed(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            TipoPermiso tipoPermiso = db.TipoPermisos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoPermiso.Status = false;
                    db.Entry(tipoPermiso).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}, {1}</b>", tipoPermiso.Clave, tipoPermiso.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoPermisos.Remove(tipoPermiso);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}, {1}</b>", tipoPermiso.Clave, tipoPermiso.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "TipoPermiso");
            return Json(new { success = true, url = url });
        }

        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnicoClave(string Clave)
        {
            var lista = db.TipoPermisos.Where(a => a.Clave == Clave);

            return Json(lista.Count() == 0);
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
