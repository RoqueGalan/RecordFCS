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
    public class TecnicaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TecnicaPieza
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

            ViewBag.PiezaID = pieza.PiezaID;

            return PartialView("_Lista", pieza.TecnicaPiezas.ToList());
        }


        // GET: TecnicaPieza/Crear
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

            var tecnicaPieza = new TecnicaPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };


            return PartialView("_Crear", tecnicaPieza);
        }


        // POST: TecnicaPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,TecnicaID")] TecnicaPieza tecnicaPieza)
        {

            tecnicaPieza.Status = true;
            //veridicar que CatalogoID NULL ó 0
            if (tecnicaPieza.TecnicaID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla TECNICA
                //extraer el ID y asignarlo a TECNICAPIEZA

                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var tecnica_existe = db.Tecnicas.Where(a => a.Descripcion == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a TecnicaPieza
                    if (tecnica_existe.Count > 0)
                    {
                        //ya existe
                        tecnicaPieza.TecnicaID = tecnica_existe.FirstOrDefault().TecnicaID;
                        db.Entry(tecnicaPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var tecnicaNew = new Tecnica()
                        {
                            Descripcion = text_BuscarDato,
                            Status = true
                        };

                        db.Tecnicas.Add(tecnicaNew);
                        db.SaveChanges();

                        tecnicaPieza.TecnicaID = tecnicaNew.TecnicaID;
                        db.TecnicaPiezas.Add(tecnicaPieza);
                    }

                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                    string url = Url.Action("Lista", "TecnicaPieza", new { id = tecnicaPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = tecnicaPieza.PiezaID });
                }
            }
            else
            {
                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var tecPieza_existe = db.TecnicaPiezas.Where(a => a.PiezaID == tecnicaPieza.PiezaID).ToList();
                // si es repetido agregar su ID a PiezaPieza
                if (tecPieza_existe.Count <= 0)
                {
                    //crear
                    db.TecnicaPiezas.Add(tecnicaPieza);
                    db.SaveChanges();
                }
                else
                {
                    //editar
                }
                //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                string url = Url.Action("Lista", "TecnicaPieza", new { id = tecnicaPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = tecnicaPieza.PiezaID });
            }

            ViewBag.TecnicaID = new SelectList(db.Tecnicas.Where(a => a.Status == true).OrderBy(a => a.Descripcion), "TecnicaID", "Descripcion", tecnicaPieza.TecnicaID);
            return PartialView("_Crear", tecnicaPieza);

        }


        // GET: TecnicaPieza/Editar/5
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar(Int64? idPieza, Int64? idTecnica)
        {
            if (idPieza == null || idTecnica == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(idPieza, idTecnica);
            if (tecnicaPieza == null)
            {
                return HttpNotFound();
            }

            ViewBag.TecnicaID = new SelectList(db.Tecnicas, "TecnicaID", "Descripcion", tecnicaPieza.TecnicaID);

            return PartialView("_Editar", tecnicaPieza);
        }


        // POST: TecnicaPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,TecnicaID,Status")] TecnicaPieza tecnicaPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tecnicaPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Técnica: <b>{0}</b> se edito con exitó.", tecnicaPieza.Tecnica.Descripcion), true);
                string url = Url.Action("Lista", "TecnicaPieza", new { id = tecnicaPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = tecnicaPieza.PiezaID });

            }

            ViewBag.TecnicaID = new SelectList(db.Tecnicas, "TecnicaID", "ClaveSiglas", tecnicaPieza.TecnicaID);

            return PartialView("_Editar", tecnicaPieza);
        }


        // GET: TecnicaPieza/Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idTecnica)
        {
            if (idPieza == null || idTecnica == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(idPieza, idTecnica);
            if (tecnicaPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", tecnicaPieza);
        }

        // POST: TecnicaPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64 PiezaID, Int64 TecnicaID)
        {
            string btnValue = Request.Form["accionx"];

            var tecnicaPieza = db.TecnicaPiezas.Find(PiezaID, TecnicaID);
            var tecnica = tecnicaPieza.Tecnica;

            switch (btnValue)
            {
                case "deshabilitar":
                    tecnicaPieza.Status = false;
                    db.Entry(tecnicaPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tecnica.Descripcion), true);

                    break;
                case "eliminar":
                    db.TecnicaPiezas.Remove(tecnicaPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tecnica.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "TecnicaPieza", new { id = PiezaID });
            return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = PiezaID });

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
