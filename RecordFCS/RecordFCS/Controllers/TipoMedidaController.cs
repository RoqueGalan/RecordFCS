using System;
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
    public class TipoMedidaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoMedida
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: TipoMedida/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista()
        {

            var tipoMedidas = db.TipoMedidas.OrderBy(u => u.Nombre);

            ViewBag.totalRegistros = tipoMedidas.Count();

            return PartialView("_Lista", tipoMedidas.ToList());
        }



        // GET: TipoMedida/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            TipoMedida tipoMedida = new TipoMedida();

            return PartialView("_Crear", tipoMedida);
        }

        // POST: TipoMedida/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "TipoMedidaID,Nombre,Status")] TipoMedida tipoMedida)
        {
            if (ModelState.IsValid)
            {
                tipoMedida.Status = true;
                db.TipoMedidas.Add(tipoMedida);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo medida: <b>{0}</b> se creo con exitó.", tipoMedida.Nombre), true);
                string url = Url.Action("Lista", "TipoMedida");
                return Json(new { success = true, url = url, modelo = "TipoMedida" });
            }

            return PartialView("_Crear", tipoMedida);
        }

        // GET: TipoMedida/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMedida tipoMedida = db.TipoMedidas.Find(id);
            if (tipoMedida == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoMedida);
        }

        // POST: TipoMedida/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "TipoMedidaID,Nombre,Status")] TipoMedida tipoMedida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoMedida).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo medida: <b>{0}</b> se edito con exitó.", tipoMedida.Nombre), true);
                string url = Url.Action("Lista", "TipoMedida");

                return Json(new { success = true, url = url });
            }
            return PartialView("_Editar", tipoMedida);
        }

        // GET: TipoMedida/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMedida tipoMedida = db.TipoMedidas.Find(id);
            if (tipoMedida == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoMedida);
        }

        // POST: TipoMedida/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            TipoMedida tipoMedida = db.TipoMedidas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoMedida.Status = false;
                    db.Entry(tipoMedida).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoMedida.Nombre), true);

                    break;
                case "eliminar":
                    db.TipoMedidas.Remove(tipoMedida);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoMedida.Nombre), true);


                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "TipoMedida");
            return Json(new { success = true, url = url });
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
