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
    public class AutorPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: AutorPieza
        [CustomAuthorize]
        public ActionResult Lista(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }

            var autorPiezas = db.AutorPiezas.Where(ap => ap.PiezaID == pieza.PiezaID);

            ViewBag.PiezaID = pieza.PiezaID;

            return PartialView("_Lista", autorPiezas.ToList());
        }


        // GET: AutorPieza/Crear
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }
            var autorPieza = new AutorPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };


            return PartialView("_Crear", autorPieza);
        }


        // POST: AutorPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,AutorID")] AutorPieza autorPieza)
        {
            autorPieza.Status = true;
            //veridicar que CatalogoID NULL ó 0
            if (autorPieza.AutorID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla CATALOGO
                //extraer el ID y asignarlo a CATALOGOPIEZA

                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var a_existe = db.Autores.Where(a => a.Nombre == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a CatalogoPieza
                    if (a_existe.Count > 0)
                    {
                        //ya existe
                        autorPieza.AutorID = a_existe.FirstOrDefault().AutorID;
                        db.Entry(autorPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var autorNew = new Autor()
                        {
                            Nombre = text_BuscarDato,
                            Status = true
                        };

                        db.Autores.Add(autorNew);
                        db.SaveChanges();

                        autorPieza.AutorID = autorNew.AutorID;
                        db.AutorPiezas.Add(autorPieza);
                    }

                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Catalogo: <b>{0}</b> se agrego con exitó.", catalogo.Nombre), true);
                    string url = Url.Action("Lista", "AutorPieza", new { id = autorPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "AutorPieza", lista = "lista", idPieza = autorPieza.PiezaID });
                }
            }
            else
            {
                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var autPieza_existe = db.AutorPiezas.Where(a => a.PiezaID == autorPieza.PiezaID && a.AutorID == autorPieza.AutorID).ToList();
                // si es repetido agregar su ID a CatalogoPieza
                if (autPieza_existe.Count <= 0)
                {
                    //crear
                    db.AutorPiezas.Add(autorPieza);
                    db.SaveChanges();
                }
                //AlertaSuccess(string.Format("Catalogo: <b>{0}</b> se agrego con exitó.", catalogo.Nombre), true);
                string url = Url.Action("Lista", "AutorPieza", new { id = autorPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "AutorPieza", lista = "lista", idPieza = autorPieza.PiezaID });
            }

            ViewBag.AutorID = new SelectList(db.Autores.Where(a => a.Status == true).OrderBy(a => a.Nombre), "AutorID", "Nombre", autorPieza.AutorID);
            return PartialView("_Crear", autorPieza);
        }


        // GET: AutorPieza/Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idAutor)
        {
            if (idPieza == null || idAutor == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutorPieza autorPieza = db.AutorPiezas.Find(idPieza, idAutor);
            if (autorPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", autorPieza);
        }


        // POST: AutorPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64? idPieza, Int64? idAutor)
        {
            string btnValue = Request.Form["accionx"];

            var autorPieza = db.AutorPiezas.Find(idPieza, idAutor);
            var autor = autorPieza.Autor;

            switch (btnValue)
            {
                case "deshabilitar":
                    autorPieza.Status = false;
                    db.Entry(autorPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", autor.Nombre), true);

                    break;
                case "eliminar":
                    db.AutorPiezas.Remove(autorPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", autor.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "AutorPieza", new { id = autorPieza.PiezaID });
            return Json(new { success = true, url = url, modelo = "AutorPieza", lista = "lista", idPieza = autorPieza.PiezaID });
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
