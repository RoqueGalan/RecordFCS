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
    public class ExposicionPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ExposicionPieza
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

            var exposicionPiezas = db.ExposicionPiezas.Where(ep => ep.PiezaID == pieza.PiezaID);

            ViewBag.PiezaID = pieza.PiezaID;

            return PartialView("_Lista", exposicionPiezas.ToList());
        }


        // GET: ExposicionPieza/Crear
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

            var exposicionPieza = new ExposicionPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            //listar solo las expos que no se haya registrado

            return PartialView("_Crear", exposicionPieza);
        }


        // POST: ExposicionPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,ExposicionID")] ExposicionPieza exposicionPieza)
        {
            exposicionPieza.Status = true;
            //verificar que ExposicionID NULL ó 0
            if (exposicionPieza.ExposicionID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla EXPOSICION
                //extraer el ID y asignarlo a EXPOSICIONPIEZA

                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var a_existe = db.Exposiciones.Where(a => a.Nombre == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a ExposicionPieza
                    if (a_existe.Count > 0)
                    {
                        //ya existe
                        exposicionPieza.ExposicionID = a_existe.FirstOrDefault().ExposicionID;
                        db.Entry(exposicionPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var exposicionNew = new Exposicion()
                        {
                            Nombre = text_BuscarDato,
                            Status = true
                        };

                        db.Exposiciones.Add(exposicionNew);
                        db.SaveChanges();

                        exposicionPieza.ExposicionID = exposicionNew.ExposicionID;
                        db.ExposicionPiezas.Add(exposicionPieza);
                    }

                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Exposición: <b>{0}</b> se agrego con exitó.", exposicion.Nombre), true);
                    string url = Url.Action("Lista", "ExposicionPieza", new { id = exposicionPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "ExposicionPieza", lista = "lista", idPieza = exposicionPieza.PiezaID });
                }
            }
            else
            {
                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var expoPieza_existe = db.ExposicionPiezas.Where(a => a.PiezaID == exposicionPieza.PiezaID && a.ExposicionID == exposicionPieza.ExposicionID).ToList();
                // si es repetido agregar su ID a ExposicionPieza
                if (expoPieza_existe.Count <= 0)
                {
                    //crear
                    db.ExposicionPiezas.Add(exposicionPieza);
                    db.SaveChanges();
                }

                //AlertaSuccess(string.Format("Exposición: <b>{0}</b> se agrego con exitó.", exposicion.Nombre), true);
                string url = Url.Action("Lista", "ExposicionPieza", new { id = exposicionPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "ExposicionPieza", lista = "lista", idPieza = exposicionPieza.PiezaID });
            }
            //AlertaSuccess(string.Format("Exposición: <b>{0}</b> se agrego con exitó.", exposicion.Nombre), true);
            ViewBag.ExposicionID = new SelectList(db.Exposiciones.Where(e => e.Status == true), "ExposicionID", "Nombre", exposicionPieza.ExposicionID);
            return PartialView("_Crear", exposicionPieza);
        }


        // GET: ExposicionPieza/Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idExposicion)
        {
            if (idPieza == null || idExposicion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExposicionPieza exposicionPieza = db.ExposicionPiezas.Find(idPieza, idExposicion);
            if (exposicionPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", exposicionPieza);
        }


        // POST: ExposicionPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64? idPieza, Int64? idExposicion)
        {
            string btnValue = Request.Form["accionx"];

            var exposicionPieza = db.ExposicionPiezas.Find(idPieza, idExposicion);
            var exposicion = exposicionPieza.Exposicion;

            switch (btnValue)
            {
                case "deshabilitar":
                    exposicionPieza.Status = false;
                    db.Entry(exposicionPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", exposicion.Nombre), true);

                    break;
                case "eliminar":
                    db.ExposicionPiezas.Remove(exposicionPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", exposicion.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "ExposicionPieza", new { id = exposicionPieza.PiezaID });
            return Json(new { success = true, url = url, modelo = "ExposicionPieza", lista = "lista", idPieza = exposicionPieza.PiezaID });
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
