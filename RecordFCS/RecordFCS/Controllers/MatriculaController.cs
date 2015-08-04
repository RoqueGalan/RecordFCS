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
    public class MatriculaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Matricula
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
                    var lista = db.Matriculas.Where(a => a.Descripcion == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var matricula = new Matricula()
                        {
                            Descripcion = valor,
                            Status = true,
                        };
                        db.Matriculas.Add(matricula);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Matricula: <b>{0}</b> se agregó con exitó.", matricula.Descripcion), true);
                        return Json(new { success = true, valor = matricula.MatriculaID, texto = matricula.Descripcion });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Matricula: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Descripcion), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().MatriculaID, texto = lista.FirstOrDefault().Descripcion });
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

            var valores = db.Matriculas.Where(a => a.Status).OrderBy(a => a.Descripcion).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "MatriculaID", "Descripcion");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Matricula> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Matriculas.Where(a => a.Descripcion.StartsWith(busqueda)).OrderBy(a => a.Descripcion).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Matriculas.Where(a => a.Descripcion.ToLower().Contains(busqueda)).OrderBy(a => a.Descripcion).Take(10);
                }

            }

            List<string> lista = new List<string>();

            if (listaTabla != null)
            {
                foreach (var item in listaTabla)
                {
                    lista.Add(item.Descripcion);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista(int? pagina)
        {
            var matriculas = db.Matriculas.Where(tec => tec.Descripcion != null).OrderBy(tec => tec.Descripcion);

            ViewBag.totalRegistros = matriculas.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Matricula> paginaMatriculas = matriculas.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaMatriculas);
        }


        [CustomAuthorize]
        // GET: Tecnica/FormLista/busqueda?seleccion
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            //cuando se implemente mejor lo de las tecnicas, mejorar esta seccion
            IQueryable<Matricula> listaMatriculas;

            if (busqueda == "")
            {
                listaMatriculas = db.Matriculas.Where(a => a.Status == true).OrderBy(a => a.Descripcion);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaMatriculas = db.Matriculas.Where(a => a.Status == true && (a.Descripcion.ToLower().Contains(busqueda))).OrderBy(a => a.Descripcion);
            }

            var totalValores = listaMatriculas.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaMatriculas.FirstOrDefault().MatriculaID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.MatriculaID = new SelectList(listaMatriculas.Where(lt => lt.Descripcion != null), "MatriculaID", "Descripcion", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Matricula/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            var matricula = new Matricula();

            ViewBag.MatriculaPadreID = new SelectList(db.Matriculas, "MatriculaID", "Descripcion", matricula.MatriculaPadreID);

            return PartialView("_Crear", matricula);
        }

        // POST: Matricula/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "MatriculaID,ClaveSigla,ClaveTexto,MatriculaPadreID,MatriculaSigla,Descripcion,Status,AntID,AntPadreID")] Matricula matricula)
        {
            if (ModelState.IsValid)
            {
                matricula.Status = true;
                db.Matriculas.Add(matricula);
                db.SaveChanges();

                AlertaSuccess(string.Format("Matricula: <b>{0}</b> se creo con exitó.", matricula.Descripcion), true);
                string url = Url.Action("Lista", "Matricula");
                return Json(new { success = true, url = url, modelo = "Matricula" });

            }

            ViewBag.MatriculaPadreID = new SelectList(db.Matriculas, "MatriculaID", "Descripcion", matricula.MatriculaPadreID);

            return View(matricula);
        }


        // GET: Matricula/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Matricula matricula = db.Matriculas.Find(id);
            if (matricula == null)
            {
                return HttpNotFound();
            }

            ViewBag.MatriculaPadreID = new SelectList(db.Matriculas, "MatriculaID", "Descripcion", matricula.MatriculaPadreID);

            return PartialView("_Editar", matricula);

        }

        // POST: Matricula/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "MatriculaID,ClaveSigla,ClaveTexto,MatriculaPadreID,MatriculaSigla,Descripcion,Status,AntID,AntPadreID")] Matricula matricula, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matricula).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Matricula: <b>{0}</b> se edito con exitó.", matricula.Descripcion), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "Matricula");
                    return Json(new { success = true, url = url });
                }
                else
                {
                    url = Url.Action("Lista", "MatriculaPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "MatriculaPieza", lista = "lista", idPieza = idPieza });
                }
            }

            ViewBag.MatriculaPadreID = new SelectList(db.Matriculas, "MatriculaID", "Descripcion", matricula.MatriculaPadreID);

            return PartialView("_Editar", matricula);
        }


        // GET: Matricula/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Matricula matricula = db.Matriculas.Find(id);
            if (matricula == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", matricula);

        }

        // POST: Matricula/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(long id)
        {
            string btnValue = Request.Form["accionx"];

            Matricula matricula = db.Matriculas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    matricula.Status = false;
                    db.Entry(matricula).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", matricula.Descripcion), true);

                    break;
                case "eliminar":
                    db.Matriculas.Remove(matricula);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", matricula.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Matricula");
            return Json(new { success = true, url = url });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuthorize]
        public JsonResult Autocompletar(string term)
        {
            var itemFiltrados = db.Matriculas.Where(t => t.Descripcion.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
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
