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
using PagedList;

namespace RecordFCS.Controllers
{
    public class TecnicaMarcoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TecnicaMarco
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
                    var lista = db.TecnicaMarcos.Where(a => a.Descripcion == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var tecnicaMarco = new TecnicaMarco()
                        {
                            Descripcion = valor,
                            Status = true,
                        };
                        db.TecnicaMarcos.Add(tecnicaMarco);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Técnica Marco: <b>{0}</b> se agregó con exitó.", tecnicaMarco.Descripcion), true);
                        return Json(new { success = true, valor = tecnicaMarco.TecnicaMarcoID, texto = tecnicaMarco.Descripcion });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Técnica Marco: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Descripcion), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().TecnicaMarcoID, texto = lista.FirstOrDefault().Descripcion });
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

            var valores = db.TecnicaMarcos.Where(a => a.Status).OrderBy(a => a.Descripcion).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "TecnicaMarcoID", "Descripcion");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }

        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<TecnicaMarco> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.TecnicaMarcos.Where(a => a.Descripcion == busqueda).OrderBy(a => a.Descripcion);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.TecnicaMarcos.Where(a => a.Descripcion.ToLower().Contains(busqueda)).OrderBy(a => a.Descripcion);
                }

            }

            List<string> lista = new List<string>();

            if (listaTabla != null)
            {
                int i = 0;
                foreach (var item in listaTabla)
                {
                    i++;
                    if (i > 10)
                        break;

                    lista.Add(item.Descripcion);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista(int? pagina)
        {
            var tecnicaMarcos = db.TecnicaMarcos.Where(tec => tec.Descripcion != null).OrderBy(tec => tec.Descripcion);

            ViewBag.totalRegistros = tecnicaMarcos.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<TecnicaMarco> paginaTecnicaMarcos = tecnicaMarcos.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaTecnicaMarcos);
        }


        [CustomAuthorize]
        // GET: Tecnica/FormLista/busqueda?seleccion
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            //cuando se implemente mejor lo de las tecnicas, mejorar esta seccion
            IQueryable<TecnicaMarco> listaTecnicaMarcos;

            if (busqueda == "")
            {
                listaTecnicaMarcos = db.TecnicaMarcos.Where(a => a.Status == true).OrderBy(a => a.Descripcion);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaTecnicaMarcos = db.TecnicaMarcos.Where(a => a.Status == true && (a.Descripcion.ToLower().Contains(busqueda))).OrderBy(a => a.Descripcion);
            }

            var totalValores = listaTecnicaMarcos.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaTecnicaMarcos.FirstOrDefault().TecnicaMarcoID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.TecnicaMarcoID = new SelectList(listaTecnicaMarcos.Where(lt => lt.Descripcion != null), "TecnicaMarcoID", "Descripcion", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: TecnicaMarco/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            var tecnicaMarco = new TecnicaMarco();
            //ViewBag.TecnicaMarcoPadreID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion");
            return PartialView("_Crear", tecnicaMarco);
        }

        // POST: TecnicaMarco/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "TecnicaMarcoID,ClaveSigla,ClaveTexto,TecnicaMarcoPadreID,MatriculaSigla,Descripcion,Status,AntID,AntPadreID")] TecnicaMarco tecnicaMarco)
        {
            if (ModelState.IsValid)
            {
                tecnicaMarco.Status = true;
                db.TecnicaMarcos.Add(tecnicaMarco);
                db.SaveChanges();

                AlertaSuccess(string.Format("Técnica Marco: <b>{0}</b> se creo con exitó.", tecnicaMarco.Descripcion), true);
                string url = Url.Action("Lista", "TecnicaMarco");
                return Json(new { success = true, url = url, modelo = "TecnicaMarco" });

            }

            ViewBag.TecnicaMarcoPadreID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion", tecnicaMarco.TecnicaMarcoPadreID);
            return PartialView("_Crear", tecnicaMarco);
        }

        // GET: TecnicaMarco/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaMarco tecnicaMarco = db.TecnicaMarcos.Find(id);
            if (tecnicaMarco == null)
            {
                return HttpNotFound();
            }
            ViewBag.TecnicaMarcoPadreID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion", tecnicaMarco.TecnicaMarcoPadreID);
            return PartialView("_Editar", tecnicaMarco);
        }

        // POST: TecnicaMarco/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "TecnicaMarcoID,ClaveSigla,ClaveTexto,TecnicaMarcoPadreID,MatriculaSigla,Descripcion,Status,AntID,AntPadreID")] TecnicaMarco tecnicaMarco, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tecnicaMarco).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Técnica Marco: <b>{0}</b> se edito con exitó.", tecnicaMarco.Descripcion), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "TecnicaMarco");
                    return Json(new { success = true, url = url });
                }
                else
                {
                    url = Url.Action("Lista", "TecnicaMarcoPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "TecnicaMarcoPieza", lista = "lista", idPieza = idPieza });
                }
            }
            ViewBag.TecnicaMarcoPadreID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion", tecnicaMarco.TecnicaMarcoPadreID);
            return PartialView("_Editar", tecnicaMarco);
        }

        // GET: TecnicaMarco/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaMarco tecnicaMarco = db.TecnicaMarcos.Find(id);
            if (tecnicaMarco == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tecnicaMarco);
        }

        // POST: TecnicaMarco/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(long id)
        {
            string btnValue = Request.Form["accionx"];

            TecnicaMarco tecnicaMarco = db.TecnicaMarcos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tecnicaMarco.Status = false;
                    db.Entry(tecnicaMarco).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tecnicaMarco.Descripcion), true);

                    break;
                case "eliminar":
                    db.TecnicaMarcos.Remove(tecnicaMarco);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tecnicaMarco.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "TecnicaMarco");
            return Json(new { success = true, url = url });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuthorize]
        public JsonResult Autocompletar(string term)
        {
            var itemFiltrados = db.TecnicaMarcos.Where(t => t.Descripcion.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(itemFiltrados, JsonRequestBehavior.AllowGet);
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
