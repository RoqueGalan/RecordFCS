using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;
using PagedList;
using RecordFCS.Helpers;
using System.Collections.Generic;
using RecordFCS.Helpers.Seguridad;

namespace RecordFCS.Controllers
{
    public class ListaValorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        [CustomAuthorize]
        public ActionResult Index()
        {
            //redireccionar a Tipo de Obras
            return RedirectToAction("Lista", "TipoAtributo");
        }

        [CustomAuthorize]
        public ActionResult AgregarValor(Int64 id, string valor, bool aceptar)
        {

            TipoAtributo tipoAtt = db.TipoAtributos.Find(id);

            if (tipoAtt != null)
            {
                if (aceptar)
                {
                    if (valor != null || valor != "")
                    {
                        var lista = db.ListaValores.Where(a => a.TipoAtributoID == id && a.Valor == valor).ToList();
                        if (lista.Count == 0)
                        {
                            var listaValor = new ListaValor()
                            {
                                Valor = valor,
                                Status = true,
                                TipoAtributoID = id
                            };
                            db.ListaValores.Add(listaValor);
                            db.SaveChanges();
                            AlertaSuccess(string.Format("{0}: <b>{1}</b> se agregó con exitó.", tipoAtt.Nombre, listaValor.Valor), true);
                            return Json(new { success = true, valor = listaValor.ListaValorID, texto = listaValor.Valor });
                        }
                        else
                        {
                            AlertaSuccess(string.Format("{0}: <b>{1}</b> se agregó con exitó.", tipoAtt.Nombre, lista.FirstOrDefault().Valor), true);

                            return Json(new { success = true, valor = lista.FirstOrDefault().ListaValorID, texto = lista.FirstOrDefault().Valor });
                        }
                    }
                }
            }

            return Json(new { success = false });
        }

        [CustomAuthorize]
        public ActionResult ListaString(Int64 idTipoAtributo, string busqueda, bool exacta)
        {
            TempData["listaValores"] = null;
            IQueryable<Object> listaTabla;
            List<string> lista = new List<string>();

            TipoAtributo tipoAtt = db.TipoAtributos.Find(idTipoAtributo);


            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (tipoAtt.EsLista)
                {
                    //los valores estan en ListaValor
                    if (exacta)
                    {
                        listaTabla = db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.Valor.StartsWith(busqueda)).OrderBy(a => a.Valor);
                    }
                    else
                    {
                        busqueda = busqueda.ToLower();
                        listaTabla = db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.Valor.ToLower().Contains(busqueda)).OrderBy(a => a.Valor).Take(10);
                    }

                    if (listaTabla != null)
                    {
                        int i = 0;
                        foreach (var item in listaTabla as IQueryable<ListaValor>)
                        {
                            i++;
                            if (i > 10)
                                break;

                            lista.Add(item.Valor);
                        }
                    }
                }
                else
                {
                    //los valores estan en AtributoPieza
                    if (exacta)
                    {
                        listaTabla = db.AtributoPiezas.Where(a => a.Atributo.TipoAtributoID == tipoAtt.TipoAtributoID && a.Valor.StartsWith(busqueda)).OrderBy(a => a.Valor);
                    }
                    else
                    {
                        busqueda = busqueda.ToLower();
                        listaTabla = db.AtributoPiezas.Where(a => a.Atributo.TipoAtributoID == tipoAtt.TipoAtributoID && a.Valor.ToLower().Contains(busqueda)).OrderBy(a => a.Valor);
                    }

                    if (listaTabla != null)
                    {
                        int i = 0;
                        foreach (var item in listaTabla as IQueryable<AtributoPieza>)
                        {
                            i++;
                            if (i > 10)
                                break;

                            lista.Add(item.Valor);
                        }
                    }
                }

            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }



        // GET: Lista/TipoAtributoID
        [CustomAuthorize(permiso = "CatVer")]
        public ActionResult Lista(Int64? id, int? pagina)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);

            if (tipoAtributo == null || !tipoAtributo.EsLista)
            {
                return HttpNotFound();
            }

            ViewBag.totalRegistros = tipoAtributo.ListaValores.Count;
            ViewBag.TipoAtributoID = id;


            //paginador
            int pagTamano = 100;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<ListaValor> paginaListaValores = tipoAtributo.ListaValores.OrderBy(lv => lv.Valor).ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaListaValores);
        }



        // GET: Propietario/FormLista
        // Campo que se muestra en los formularios
        [CustomAuthorize]
        public ActionResult FormLista(Int64 idTipoAtributo, string busqueda, Int64? seleccion)
        {
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(idTipoAtributo);

            if (tipoAtributo == null || !tipoAtributo.EsLista)
            {
                return HttpNotFound();
            }

            // validar que no muestre los campos con valor NULL


            //buscar registros sin importar aA
            //var listaValores = db.ListaValores.Where(lv => lv.TipoAtributoID == idTipoAtributo && lv.Valor.ToUpper().Contains(busqueda.ToUpper())).OrderBy(lv => lv.Valor);

            IQueryable<ListaValor> listaValores;

            if (busqueda == "")
            {
                listaValores = db.ListaValores.Where(lv => lv.TipoAtributoID == idTipoAtributo).OrderBy(lv => lv.Valor);

            }
            else
            {
                busqueda = busqueda.ToLower();
                listaValores = db.ListaValores.Where(lv => lv.TipoAtributoID == idTipoAtributo && lv.Status == true && lv.Valor.ToLower().Contains(busqueda)).OrderBy(lv => lv.Valor);
            }

            var totalValores = listaValores.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaValores.FirstOrDefault().ListaValorID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.ListaValorID = new SelectList(listaValores, "ListaValorID", "Valor", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: ListaValor/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear(Int64? id)
        {
            if (id == null)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);

            if (tipoAtributo == null || !tipoAtributo.EsLista)
            {
                return HttpNotFound();
            }

            ListaValor listaValor = new ListaValor()
            {
                TipoAtributo = tipoAtributo,
                TipoAtributoID = tipoAtributo.TipoAtributoID,
                Status = true
            };


            return PartialView("_Crear", listaValor);
        }

        // POST: ListaValor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "ListaValorID,TipoAtributoID,Valor,Status,AntID")] ListaValor listaValor)
        {
            if (ModelState.IsValid)
            {
                listaValor.Status = true;
                db.ListaValores.Add(listaValor);
                db.SaveChanges();

                AlertaSuccess(string.Format("Valor: <b>{0}</b> se agrego con exitó.", listaValor.Valor), true);
                string url = Url.Action("Lista", "ListaValor", new { id = listaValor.TipoAtributoID });

                return Json(new { success = true, url = url, modelo = "ListaValor" });
            }

            // si hay error
            return PartialView("_Crear", listaValor);
        }


        // GET: ListaValor/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListaValor listaValor = db.ListaValores.Find(id);
            if (listaValor == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", listaValor);
        }

        // POST: ListaValor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "ListaValorID,TipoAtributoID,Valor,Status,AntID")] ListaValor listaValor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(listaValor).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Valor: <b>{0}</b> se edito con exitó.", listaValor.Valor), true);
                string url = Url.Action("Lista", "ListaValor", new { id = listaValor.TipoAtributoID });

                return Json(new { success = true, url = url });
            }
            // si hay error

            return PartialView("_Editar", listaValor);
        }

        // GET: ListaValor/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListaValor listaValor = db.ListaValores.Find(id);
            if (listaValor == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", listaValor);
        }

        // POST: ListaValor/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            ListaValor listaValor = db.ListaValores.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    listaValor.Status = false;
                    db.Entry(listaValor).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", listaValor.Valor), true);

                    break;
                case "eliminar":
                    db.ListaValores.Remove(listaValor);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", listaValor.Valor), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;


            }

            string url = Url.Action("Lista", "ListaValor", new { id = listaValor.TipoAtributoID });
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
