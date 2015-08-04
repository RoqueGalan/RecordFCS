using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;
using PagedList;
using RecordFCS.Helpers.Seguridad;

namespace RecordFCS.Controllers
{
    public class ExposicionController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Exposicion
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
                    var lista = db.Exposiciones.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var exposicion = new Exposicion()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.Exposiciones.Add(exposicion);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Exposición: <b>{0}</b> se agregó con exitó.", exposicion.Nombre), true);
                        return Json(new { success = true, valor = exposicion.ExposicionID, texto = exposicion.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Exposición: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().ExposicionID, texto = lista.FirstOrDefault().Nombre });
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

            var valores = db.Exposiciones.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "ExposicionID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Exposicion> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Exposiciones.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Exposiciones.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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


        // GET: Exposicion/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista(int? pagina)
        {
            var exposiciones = db.Exposiciones.OrderBy(ex => ex.Nombre);

            ViewBag.totalRegistros = exposiciones.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Exposicion> paginaExposiciones = exposiciones.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaExposiciones);
        }


        // GET: Exposicion/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Exposicion> listaExpos;

            if (busqueda == "")
            {
                listaExpos = db.Exposiciones.Where(a => a.Status == true).OrderBy(a => a.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaExpos = db.Exposiciones.Where(a => a.Status == true && (a.Nombre.ToLower().Contains(busqueda))).OrderBy(a => a.Nombre);
            }

            var totalValores = listaExpos.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaExpos.FirstOrDefault().ExposicionID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.ExposicionID = new SelectList(listaExpos, "ExposicionID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Exposicion/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            Exposicion exposicion = new Exposicion();
            return PartialView("_Crear", exposicion);
        }

        // POST: Exposicion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "ExposicionID,Nombre,Status,AntID")] Exposicion exposicion)
        {
            if (ModelState.IsValid)
            {
                exposicion.Status = true;
                db.Exposiciones.Add(exposicion);
                db.SaveChanges();

                AlertaSuccess(string.Format("Exposición: <b>{0}</b> se creo con exitó.", exposicion.Nombre), true);
                string url = Url.Action("Lista", "Exposicion");
                return Json(new { success = true, url = url, modelo = "Exposicion" });
            }

            return PartialView("_Crear", exposicion);
        }


        // GET: Exposicion/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exposicion exposicion = db.Exposiciones.Find(id);
            if (exposicion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", exposicion);
        }


        // POST: Exposicion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "ExposicionID,Nombre,Status,AntID")] Exposicion exposicion, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exposicion).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Exposición: <b>{0}</b> se edito con exitó.", exposicion.Nombre), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "Exposicion");
                    return Json(new { success = true, url = url });
                }
                else
                {
                    url = Url.Action("Lista", "ExposicionPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "ExposicionPieza", lista = "lista", idPieza = idPieza });
                }

            }

            return PartialView("_Editar", exposicion);
        }


        // GET: Exposicion/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exposicion exposicion = db.Exposiciones.Find(id);
            if (exposicion == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", exposicion);
        }


        // POST: Exposicion/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Exposicion exposicion = db.Exposiciones.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    exposicion.Status = false;
                    db.Entry(exposicion).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", exposicion.Nombre), true);

                    break;
                case "eliminar":
                    db.Exposiciones.Remove(exposicion);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", exposicion.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Exposicion");
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
