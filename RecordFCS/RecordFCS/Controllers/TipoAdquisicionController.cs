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
    public class TipoAdquisicionController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoAdquisicion
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
                    var lista = db.TipoAdquisiciones.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var tipoAdquisicion = new TipoAdquisicion()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.TipoAdquisiciones.Add(tipoAdquisicion);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Tipo de Adquisición: <b>{0}</b> se agregó con exitó.", tipoAdquisicion.Nombre), true);
                        return Json(new { success = true, valor = tipoAdquisicion.TipoAdquisicionID, texto = tipoAdquisicion.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Tipo de Adquisición: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().TipoAdquisicionID, texto = lista.FirstOrDefault().Nombre });
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

            var valores = db.TipoAdquisiciones.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "TipoAdquisicionID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<TipoAdquisicion> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.TipoAdquisiciones.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.TipoAdquisiciones.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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


        // GET: TipoAdquisicion/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista()
        {
            var tipoAdquisicion = db.TipoAdquisiciones.OrderBy(tad => tad.Nombre);

            ViewBag.totalRegistros = tipoAdquisicion.Count();

            return PartialView("_Lista", tipoAdquisicion.ToList());
        }


        // GET: TipoAdquisicion/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<TipoAdquisicion> listaTipoAdquisiciones;

            if (busqueda == "")
            {
                listaTipoAdquisiciones = db.TipoAdquisiciones.Where(ta => ta.Status == true).OrderBy(ta => ta.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaTipoAdquisiciones = db.TipoAdquisiciones.Where(ta => ta.Status == true && ta.Nombre.ToLower().Contains(busqueda)).OrderBy(c => c.Nombre);
            }

            var totalValores = listaTipoAdquisiciones.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaTipoAdquisiciones.FirstOrDefault().TipoAdquisicionID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.TipoAdquisicionID = new SelectList(listaTipoAdquisiciones, "TipoAdquisicionID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }

        // GET: TipoAdquisicion/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            TipoAdquisicion tipoAdquisicion = new TipoAdquisicion();

            return PartialView("_Crear", tipoAdquisicion);
        }

        // POST: TipoAdquisicion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "TipoAdquisicionID,Nombre")] TipoAdquisicion tipoAdquisicion)
        {
            if (ModelState.IsValid)
            {
                tipoAdquisicion.Status = true;
                db.TipoAdquisiciones.Add(tipoAdquisicion);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Adquisición: <b>{0}</b> se creo con exitó.", tipoAdquisicion.Nombre), true);
                string url = Url.Action("Lista", "TipoAdquisicion");
                return Json(new { success = true, url = url, modelo = "TipoAdquisicion" });
            }

            return PartialView("_Crear", tipoAdquisicion);
        }


        // GET: TipoAdquisicion/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAdquisicion tipoAdquisicion = db.TipoAdquisiciones.Find(id);
            if (tipoAdquisicion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoAdquisicion);
        }

        // POST: TipoAdquisicion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "TipoAdquisicionID,Nombre,Status")] TipoAdquisicion tipoAdquisicion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoAdquisicion).State = EntityState.Modified;
                db.SaveChanges();


                AlertaSuccess(string.Format("Tipo de Adquisición: <b>{0}</b> se edito con exitó.", tipoAdquisicion.Nombre), true);
                string url = Url.Action("Lista", "TipoAdquisicion");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoAdquisicion);
        }


        // GET: TipoAdquisicion/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAdquisicion tipoAdquisicion = db.TipoAdquisiciones.Find(id);
            if (tipoAdquisicion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", tipoAdquisicion);
        }

        // POST: TipoAdquisicion/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            TipoAdquisicion tipoAdquisicion = db.TipoAdquisiciones.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoAdquisicion.Status = false;
                    db.Entry(tipoAdquisicion).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se deshabilito <b>{0}</b>", tipoAdquisicion.Nombre), true);

                    break;
                case "eliminar":
                    db.TipoAdquisiciones.Remove(tipoAdquisicion);
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se elimino <b>{0}</b>", tipoAdquisicion.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "TipoAdquisicion");
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
