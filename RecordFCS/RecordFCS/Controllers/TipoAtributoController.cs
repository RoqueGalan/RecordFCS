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
    public class TipoAtributoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoAtributo
        [CustomAuthorize(permiso = "CatVer")]
        public ActionResult Index()
        {
            //var tipoAtributos = db.TipoAtributos.OrderBy(ta => ta.Nombre);
            //ViewBag.totalRegistros = tipoAtributos.Count();

            return View();
        }

        [CustomAuthorize]
        public ActionResult CampoLista(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }

            var valores = tipoAtributo.ListaValores.Where(a => a.Status).OrderBy(a => a.Valor).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "ListaValorID", "Valor");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }

        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<TipoAtributo> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.TipoAtributos.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.TipoAtributos.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }

            }

            List<string> lista = new List<string>();

            if (listaTabla != null)
            {
                foreach (var item in listaTabla)
                {
                    lista.Add(item.Nombre);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        // GET: TipoAtributo/Lista
        [CustomAuthorize(permiso = "CatVer")]
        public ActionResult Lista()
        {
            var tipoAtributos = db.TipoAtributos.OrderBy(to => to.Nombre);

            ViewBag.totalRegistros = tipoAtributos.Count();

            return PartialView("_Lista", tipoAtributos.ToList());
        }

        // GET: TipoAtributo/Detalles/5
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Detalles(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }

            return View(tipoAtributo);
        }

        // GET: TipoAtributo/Crear
        [CustomAuthorize(permiso = "TipoAttCrear")]
        public ActionResult Crear()
        {
            var tipoAtributo = new TipoAtributo()
            {
                DatoCS = "string",
                DatoHTML = "text",
                NombreID = "Generico",
                BuscadorOrden = 100,
                Status = true,
                EsLista = false,
                Buscador = false
            };

            return PartialView("_Crear", tipoAtributo);
        }

        // POST: TipoAtributo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoAttCrear")]
        public ActionResult Crear([Bind(Include = "TipoAtributoID,Nombre,NombreHTML,Descripcion,DatoHTML,NombreID,DatoCS,Buscador,BuscadorOrden,EsLista,Status")] TipoAtributo tipoAtributo)
        {
            if (ModelState.IsValid)
            {
                //revalidar de nuevo los atributos que deben ser unicos
                if (db.TipoAtributos.Where(a => a.Nombre == tipoAtributo.Nombre).Count() > 0)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un registro con este nombre. Intenta con otro.");
                }

                if (db.TipoAtributos.Where(a => a.NombreHTML == tipoAtributo.NombreHTML).Count() > 0)
                {
                    ModelState.AddModelError("NombreHTML", "Ya existe un registro con este nombre en html. Intenta con otro.");
                }

                if (!ModelState.IsValid)
                {
                    return PartialView("_Crear", tipoAtributo);
                }

                tipoAtributo.Status = true;
                db.TipoAtributos.Add(tipoAtributo);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Atributo: <b>{0}</b> se creo con exitó.", tipoAtributo.Nombre), true);

                string url = Url.Action("Lista", "TipoAtributo");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoAtributo);
        }

        // GET: TipoAtributo/Editar/5
        [CustomAuthorize(permiso = "TipoAttEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoAtributo);
        }

        // POST: TipoAtributo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoAttEdit")]
        public ActionResult Editar([Bind(Include = "TipoAtributoID,Nombre,NombreHTML,NombreID,Descripcion,DatoHTML,DatoCS,EsLista,Status,Buscador,BuscadorOrden")] TipoAtributo tipoAtributo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoAtributo).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Atributo: <b>{0}</b> se edito con exitó.", tipoAtributo.Nombre), true);
                string url = Url.Action("Lista", "TipoAtributo");
                return Json(new { success = true, url = url });
            }
            return PartialView("_Editar", tipoAtributo);
        }

        // GET: TipoAtributo/Eliminar/5
        [CustomAuthorize(permiso = "TipoAttEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoAtributo);
        }

        // POST: TipoAtributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoAttEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            var tipoAtributo = db.TipoAtributos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoAtributo.Status = false;
                    db.Entry(tipoAtributo).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoAtributo.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoAtributos.Remove(tipoAtributo);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoAtributo.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoAtributo");
            return Json(new { success = true, url = url });
        }

        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnicoNombre(string Nombre)
        {
            var lista = db.TipoAtributos.Where(a => a.Nombre == Nombre).ToList();

            return Json(lista.Count == 0);
        }


        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnicoNombreHTML(string NombreHTML)
        {
            var lista = db.TipoAtributos.Where(a => a.NombreHTML == NombreHTML).ToList();

            return Json(lista.Count == 0);
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
