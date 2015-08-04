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
    public class TecnicaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Tecnica
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
                    var lista = db.Tecnicas.Where(a => a.Descripcion == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var tecnica = new Tecnica()
                        {
                            Descripcion = valor,
                            Status = true,
                        };
                        db.Tecnicas.Add(tecnica);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agregó con exitó.", tecnica.Descripcion), true);
                        return Json(new { success = true, valor = tecnica.TecnicaID, texto = tecnica.Descripcion });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Descripcion), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().TecnicaID, texto = lista.FirstOrDefault().Descripcion });
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

            var valores = db.Tecnicas.Where(a => a.Status).OrderBy(a => a.Descripcion).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "TecnicaID", "Descripcion");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Tecnica> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Tecnicas.Where(a => a.Descripcion.StartsWith(busqueda)).OrderBy(a => a.Descripcion);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Tecnicas.Where(a => a.Descripcion.ToLower().Contains(busqueda)).OrderBy(a => a.Descripcion);
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
            var tecnicas = db.Tecnicas.Where(tec => tec.Descripcion != null).OrderBy(tec => tec.Descripcion);

            ViewBag.totalRegistros = tecnicas.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Tecnica> paginaTecnicas = tecnicas.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaTecnicas);
        }


        [CustomAuthorize]
        // GET: Tecnica/FormLista/busqueda?seleccion
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            //cuando se implemente mejor lo de las tecnicas, mejorar esta seccion
            IQueryable<Tecnica> listaTecnicas;

            if (busqueda == "")
            {
                listaTecnicas = db.Tecnicas.Where(a => a.Status == true).OrderBy(a => a.Descripcion);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaTecnicas = db.Tecnicas.Where(a => a.Status == true && (a.Descripcion.ToLower().Contains(busqueda))).OrderBy(a => a.Descripcion);
            }

            var totalValores = listaTecnicas.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaTecnicas.FirstOrDefault().TecnicaID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.TecnicaID = new SelectList(listaTecnicas.Where(lt => lt.Descripcion != null), "TecnicaID", "Descripcion", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Tecnica/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            var tecnica = new Tecnica();

            //ViewBag.TecnicaPadreID = new SelectList(db.Tecnicas, "TecnicaID", "ClaveSiglas", tecnica.TecnicaPadreID);

            return PartialView("_Crear", tecnica);
        }


        // POST: Tecnica/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "TecnicaID,ClaveSiglas,ClaveTexto,TecnicaPadreID,MatriculaSiglas,MatriculaTexto,Descripcion,Status,AntID,AntPadreID")] Tecnica tecnica)
        {
            if (ModelState.IsValid)
            {
                tecnica.Status = true;
                db.Tecnicas.Add(tecnica);
                db.SaveChanges();

                AlertaSuccess(string.Format("Técnica: <b>{0}</b> se creo con exitó.", tecnica.Descripcion), true);
                string url = Url.Action("Lista", "Tecnica");
                return Json(new { success = true, url = url, modelo = "Tecnica" });

            }

            ViewBag.TecnicaPadreID = new SelectList(db.Tecnicas, "TecnicaID", "ClaveSiglas", tecnica.TecnicaPadreID);
            return View("_Crear",tecnica);
        }

        [CustomAuthorize(permiso = "CatEdit")]
        // GET: Tecnica/Editar/5
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tecnica tecnica = db.Tecnicas.Find(id);
            if (tecnica == null)
            {
                return HttpNotFound();
            }

            ViewBag.TecnicaPadreID = new SelectList(db.Tecnicas, "TecnicaID", "Descripcion", tecnica.TecnicaPadreID);

            return PartialView("_Editar", tecnica);
        }


        // POST: Tecnica/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "TecnicaID,ClaveSiglas,ClaveTexto,TecnicaPadreID,MatriculaSiglas,MatriculaTexto,Descripcion,Status,AntID,AntPadreID")] Tecnica tecnica, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tecnica).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Técnica: <b>{0}</b> se edito con exitó.", tecnica.Descripcion), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "Tecnica");
                    return Json(new { success = true, url = url });
                }
                else
                {
                    url = Url.Action("Lista", "TecnicaPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = idPieza });
                }

            }
            ViewBag.TecnicaPadreID = new SelectList(db.Tecnicas, "TecnicaID", "ClaveSiglas", tecnica.TecnicaPadreID);
            return PartialView("_Editar", tecnica);
        }

        // GET: Tecnica/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tecnica tecnica = db.Tecnicas.Find(id);
            if (tecnica == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tecnica);
        }

        // POST: Tecnica/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Tecnica tecnica = db.Tecnicas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tecnica.Status = false;
                    db.Entry(tecnica).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tecnica.Descripcion), true);

                    break;
                case "eliminar":
                    db.Tecnicas.Remove(tecnica);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tecnica.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Tecnica");
            return Json(new { success = true, url = url });

        }


        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuthorize]
        public JsonResult Autocompletar(string term)
        {
            var itemFiltrados = db.Tecnicas.Where(t => t.Descripcion.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
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
