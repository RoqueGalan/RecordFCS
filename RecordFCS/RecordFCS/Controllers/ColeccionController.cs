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
    public class ColeccionController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Coleccion
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult AgregarValor(string valor, bool aceptar)
        {
            if (aceptar)
            {
                if (valor != null || valor != "")
                {
                    var lista = db.Colecciones.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var coleccion = new Coleccion()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.Colecciones.Add(coleccion);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Colección: <b>{0}</b> se agregó con exitó.", coleccion.Nombre), true);
                        return Json(new { success = true, valor = coleccion.ColeccionID, texto = coleccion.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Colección: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().ColeccionID, texto = lista.FirstOrDefault().Nombre });
                    }
                }
            }
            return Json(new { success = false });
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

            var valores = db.Colecciones.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "ColeccionID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Coleccion> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Colecciones.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Colecciones.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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


        // GET: Coleccion/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista()
        {
            var colecciones = db.Colecciones.OrderBy(c => c.Nombre);

            ViewBag.totalRegistros = colecciones.Count();

            return PartialView("_Lista", colecciones.ToList());
        }


        // GET: Coleccion/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Coleccion> listaColecciones;

            if (busqueda == "")
            {
                listaColecciones = db.Colecciones.Where(c => c.Status == true).OrderBy(c => c.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaColecciones = db.Colecciones.Where(c => c.Status == true && c.Nombre.ToLower().Contains(busqueda)).OrderBy(c => c.Nombre);
            }

            var totalValores = listaColecciones.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaColecciones.FirstOrDefault().ColeccionID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.ColeccionID = new SelectList(listaColecciones, "ColeccionID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Coleccion/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            Coleccion coleccion = new Coleccion();

            return PartialView("_Crear", coleccion);
        }


        // POST: Coleccion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "ColeccionID,Nombre,AntID")] Coleccion coleccion)
        {
            if (ModelState.IsValid)
            {
                coleccion.Status = true;
                db.Colecciones.Add(coleccion);
                db.SaveChanges();

                AlertaSuccess(string.Format("Colección: <b>{0}</b> se creo con exitó.", coleccion.Nombre), true);
                string url = Url.Action("Lista", "Coleccion");
                return Json(new { success = true, url = url, modelo = "Coleccion" });

            }

            return PartialView("_Crear", coleccion);
        }



        // GET: Coleccion/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coleccion coleccion = db.Colecciones.Find(id);
            if (coleccion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", coleccion);
        }


        // POST: Coleccion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "ColeccionID,Nombre,Status,AntID")] Coleccion coleccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coleccion).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Colección: <b>{0}</b> se edito con exitó.", coleccion.Nombre), true);
                string url = Url.Action("Lista", "Coleccion");
                return Json(new { success = true, url = url, modelo = "Coleccion" });

            }

            return PartialView("_Editar", coleccion);

        }


        // GET: Coleccion/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coleccion coleccion = db.Colecciones.Find(id);
            if (coleccion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", coleccion);

        }

        // POST: Coleccion/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Coleccion coleccion = db.Colecciones.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    coleccion.Status = false;
                    db.Entry(coleccion).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se deshabilito <b>{0}</b>", coleccion.Nombre), true);

                    break;
                case "eliminar":
                    db.Colecciones.Remove(coleccion);
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se elimino <b>{0}</b>", coleccion.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Coleccion");
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
