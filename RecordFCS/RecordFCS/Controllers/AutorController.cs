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
    public class AutorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Autor
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
                    var lista = db.Autores.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        Autor autor = new Autor()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.Autores.Add(autor);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Autor: <b>{0}, {1}</b> se agregó con exitó.", autor.Nombre, autor.Apellido), true);
                        return Json(new { success = true, valor = autor.AutorID, texto = autor.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Autor: <b>{0}, {1}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre, lista.FirstOrDefault().Apellido), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().AutorID, texto = lista.FirstOrDefault().Nombre });
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

            var valores = db.Autores.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "AutorID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Autor> listaAutores;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaAutores = null;
            }
            else
            {

                if (exacta)
                {
                    listaAutores = db.Autores.Where(a => a.Nombre == busqueda || a.Apellido.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaAutores = db.Autores.Where(a => a.Nombre.ToLower().Contains(busqueda) || a.Apellido.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
            }

            List<string> lista = new List<string>();

            if (listaAutores != null)
            {
                foreach (var item in listaAutores)
                {
                    lista.Add(item.Nombre + " " + item.Apellido);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        // GET: Autor/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista(int? pagina)
        {
            var autores = db.Autores.OrderBy(u => u.Nombre);

            ViewBag.totalRegistros = autores.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Autor> paginaAutores = autores.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaAutores);
        }


        // GET: Autor/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Autor> listaAutores;

            if (busqueda == "")
            {
                listaAutores = db.Autores.Where(a => a.Status == true).OrderBy(a => a.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaAutores = db.Autores.Where(a => a.Status == true && (a.Nombre.ToLower().Contains(busqueda) || a.Apellido.ToLower().Contains(busqueda))).OrderBy(a => a.Nombre);
            }

            var totalValores = listaAutores.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaAutores.FirstOrDefault().AutorID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.AutorID = new SelectList(listaAutores, "AutorID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Autor/Crear
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear()
        {
            Autor autor = new Autor();

            return PartialView("_Crear", autor);

        }

        // POST: Autor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatCrear")]
        public ActionResult Crear([Bind(Include = "AutorID,Nombre,Apellido,LugarNacimiento,AnioNacimiento,LugarMuerte,AnioMuerte,Observaciones,Status")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                autor.Status = true;
                db.Autores.Add(autor);
                db.SaveChanges();

                AlertaSuccess(string.Format("Autor: <b>{0}, {1}</b> se creo con exitó.", autor.Nombre, autor.Apellido), true);
                string url = Url.Action("Lista", "Autor");
                return Json(new { success = true, url = url, modelo = "Autor" });
            }

            return PartialView("_Crear", autor);
        }


        // GET: Autor/Editar/5
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autor autor = db.Autores.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", autor);
        }


        // POST: Autor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEdit")]
        public ActionResult Editar([Bind(Include = "AutorID,Nombre,Apellido,LugarNacimiento,AnioNacimiento,LugarMuerte,AnioMuerte,Observaciones,Status")] Autor autor, Int64? idPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autor).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Autor: <b>{0}, {1}</b> se edito con exitó.", autor.Nombre, autor.Apellido), true);

                string url = "";

                if (idPieza == null)
                {
                    url = Url.Action("Lista", "Autor");
                    return Json(new { success = true, url = url });
                }
                else
                {

                    url = Url.Action("Lista", "AutorPieza", new { id = idPieza });
                    return Json(new { success = true, url = url, modelo = "AutorPieza", lista = "lista", idPieza = idPieza });
                }

            }

            return PartialView("_Editar", autor);
        }


        // GET: Autor/Eliminar/5
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autor autor = db.Autores.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", autor);
        }


        // POST: Autor/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "CatEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Autor autor = db.Autores.Find(id);


            switch (btnValue)
            {
                case "deshabilitar":
                    autor.Status = false;
                    db.Entry(autor).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}, {1}</b>", autor.Nombre, autor.Apellido), true);

                    break;
                case "eliminar":
                    db.Autores.Remove(autor);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}, {1}</b>", autor.Nombre, autor.Apellido), true);


                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }
            string url = Url.Action("Lista", "Autor");
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
