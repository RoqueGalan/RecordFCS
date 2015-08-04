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

namespace RecordFCS.Controllers
{
    public class CatalogoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Catalogo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AgregarValor(string valor, bool aceptar)
        {
            if (aceptar)
            {
                if (valor != null || valor != "")
                {
                    var lista = db.Catalogos.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var catalogo = new Catalogo()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.Catalogos.Add(catalogo);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Catálogo: <b>{0}</b> se agregó con exitó.", catalogo.Nombre), true);
                        return Json(new { success = true, valor = catalogo.CatalogoID, texto = catalogo.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Catálogo: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().CatalogoID, texto = lista.FirstOrDefault().Nombre });
                    }
                }
            }
            return Json(new { success = false });
        }

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

            var valores = db.Catalogos.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "CatalogoID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }

        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Catalogo> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Catalogos.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Catalogos.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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

        public ActionResult Lista(int? pagina)
        {
            var catalogos = db.Catalogos.OrderBy(ca => ca.Nombre);

            ViewBag.totalRegistros = catalogos.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Catalogo> paginaCatalogos = catalogos.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaCatalogos);
        }

        // GET: Catalogo/FormLista/busqueda?seleccion
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Catalogo> listaCatalogos;

            if(busqueda == "")
            {
                listaCatalogos = db.Catalogos.Where(cat => cat.Status == true).OrderBy(cat=>cat.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaCatalogos = db.Catalogos.Where(cat => cat.Status == true && cat.Nombre.ToLower().Contains(busqueda)).OrderBy(cat => cat.Nombre);
            }

            var totalValores = listaCatalogos.Count();

            if (totalValores > 0)
            {
                if(seleccion == null)
                {
                    seleccion = listaCatalogos.FirstOrDefault().CatalogoID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.CatalogoID = new SelectList(listaCatalogos, "CatalogoID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }

        // GET: Catalogo/Crear
        public ActionResult Crear()
        {
            Catalogo catalogo = new Catalogo();
            return PartialView("_Crear", catalogo);
        }

        // POST: Catalogo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "CatalogoID,Nombre,Status,AntID")] Catalogo catalogo)
        {
            if (ModelState.IsValid)
            {
                catalogo.Status = true;
                db.Catalogos.Add(catalogo);
                db.SaveChanges();

                AlertaSuccess(string.Format("Catalogo: <b>{0}</b> se creo con exitó.", catalogo.Nombre), true);
                string url = Url.Action("Lista", "Catalogo");
                return Json(new { success = true, url = url, modelo = "Catalogo" });
            }

            return PartialView("_Crear", catalogo);

        }

        // GET: Catalogo/Editar/5
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalogo catalogo = db.Catalogos.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", catalogo);
        }

        // POST: Catalogo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "CatalogoID,Nombre,Status,AntID")] Catalogo catalogo, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Catalogo: <b>{0}</b> se edito con exitó.", catalogo.Nombre), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "Catalogo");
                    return Json(new { success = true, url = url });
                }
                else
                {
                    url = Url.Action("Lista", "CatalogoPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "CatalogoPieza", lista = "lista", idPieza = idPieza });
                }

            }

            return PartialView("_Editar", catalogo);
        }

        // GET: Catalogo/Eliminar/5
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalogo catalogo = db.Catalogos.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", catalogo);
        }

        // POST: Catalogo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Catalogo catalogo = db.Catalogos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    catalogo.Status = false;
                    db.Entry(catalogo).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", catalogo.Nombre), true);

                    break;
                case "eliminar":
                    db.Catalogos.Remove(catalogo);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", catalogo.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Catalogo");
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
