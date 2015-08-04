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
    public class TecnicaMarcoPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TecnicaMarcoPieza
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

            return PartialView("_Lista", pieza.TecnicaMarcoPieza.ToList());
        }


        // GET: TecnicaMarcoPieza/Crear
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

            var tecnicaMarcoPieza = new TecnicaMarcoPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            return PartialView("_Crear", tecnicaMarcoPieza);

        }

        // POST: TecnicaMarcoPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,TecnicaMarcoID,Status")] TecnicaMarcoPieza tecnicaMarcoPieza)
        {
            tecnicaMarcoPieza.Status = true;

            //verificar que ID no sea NULL o 0
            if (tecnicaMarcoPieza.TecnicaMarcoID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla TECNICAMARCO
                //extraer el ID y asignarlo a TECNICAMARCOPIEZA

                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var tecnicaMarco_existe = db.TecnicaMarcos.Where(a => a.Descripcion == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a TecnicaMarcoPieza
                    if (tecnicaMarco_existe.Count > 0)
                    {
                        //ya existe
                        tecnicaMarcoPieza.TecnicaMarcoID = tecnicaMarco_existe.FirstOrDefault().TecnicaMarcoID;
                        db.Entry(tecnicaMarcoPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var tecnicaMarcoNew = new TecnicaMarco()
                        {
                            Descripcion = text_BuscarDato,
                            Status = true
                        };

                        db.TecnicaMarcos.Add(tecnicaMarcoNew);
                        db.SaveChanges();

                        tecnicaMarcoPieza.TecnicaMarcoID = tecnicaMarcoNew.TecnicaMarcoID;
                        db.TecnicaMarcoPiezas.Add(tecnicaMarcoPieza);
                    }
                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                    string url = Url.Action("Lista", "TecnicaMarcoPieza", new { id = tecnicaMarcoPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "TecnicaMarcoPieza", lista = "lista", idPieza = tecnicaMarcoPieza.PiezaID });
                }
            }
            else
            {

                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var tecMarcoPieza_existe = db.TecnicaMarcoPiezas.Where(a => a.PiezaID == tecnicaMarcoPieza.PiezaID).ToList();
                // si es repetido agregar su ID a PiezaPieza
                if (tecMarcoPieza_existe.Count <= 0)
                {
                    //crear
                    db.TecnicaMarcoPiezas.Add(tecnicaMarcoPieza);
                    db.SaveChanges();
                }
                else
                {
                    //editar
                }
                //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                string url = Url.Action("Lista", "TecnicaMarcoPieza", new { id = tecnicaMarcoPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "TecnicaPieza", lista = "lista", idPieza = tecnicaMarcoPieza.PiezaID });

            }

            ViewBag.TecnicaMarcoID = new SelectList(db.TecnicaMarcos.Where(a => a.Status == true).OrderBy(a => a.Descripcion), "TecnicaMarcoID", "Descripcion", tecnicaMarcoPieza.TecnicaMarcoID);
            return PartialView("_Crear", tecnicaMarcoPieza);
        }

        // GET: TecnicaMarcoPieza/Editar/5
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar(Int64? idPieza, Int64? idTecnicaMarco)
        {
            if (idPieza == null || idTecnicaMarco == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaMarcoPieza tecnicaMarcoPieza = db.TecnicaMarcoPiezas.Find(idPieza, idTecnicaMarco);


            if (tecnicaMarcoPieza == null)
            {
                return HttpNotFound();
            }
            ViewBag.TecnicaMarcoID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion", tecnicaMarcoPieza.TecnicaMarcoID);

            return PartialView("_Editar", tecnicaMarcoPieza);
        }

        // POST: TecnicaMarcoPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,TecnicaMarcoID,Status")] TecnicaMarcoPieza tecnicaMarcoPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tecnicaMarcoPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Técnica Marco: <b>{0}</b> se edito con exitó.", tecnicaMarcoPieza.TecnicaMarco.Descripcion), true);
                string url = Url.Action("Lista", "TecnicaMarcoPieza", new { id = tecnicaMarcoPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "TecnicaMarcoPieza", lista = "lista", idPieza = tecnicaMarcoPieza.PiezaID });

            }
            ViewBag.TecnicaMarcoID = new SelectList(db.TecnicaMarcos, "TecnicaMarcoID", "Descripcion", tecnicaMarcoPieza.TecnicaMarcoID);
            return PartialView("_Editar", tecnicaMarcoPieza);
        }

        // GET: TecnicaMarcoPieza/Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idTecnicaMarco)
        {
            if (idPieza == null || idTecnicaMarco == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaMarcoPieza tecnicaMarcoPieza = db.TecnicaMarcoPiezas.Find(idPieza, idTecnicaMarco);

            if (tecnicaMarcoPieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tecnicaMarcoPieza);
        }

        // POST: TecnicaMarcoPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64 PiezaID, Int64 TecnicaMarcoID)
        {
            string btnValue = Request.Form["accionx"];

            TecnicaMarcoPieza tecnicaMarcoPieza = db.TecnicaMarcoPiezas.Find(PiezaID, TecnicaMarcoID);
            var tecnicaMarco = tecnicaMarcoPieza.TecnicaMarco;

            switch (btnValue)
            {
                case "deshabilitar":
                    tecnicaMarcoPieza.Status = false;
                    db.Entry(tecnicaMarcoPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tecnicaMarco .Descripcion), true);

                    break;
                case "eliminar":
                    db.TecnicaMarcoPiezas.Remove(tecnicaMarcoPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tecnicaMarco.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "TecnicaMarcoPieza", new { id = PiezaID });
            return Json(new { success = true, url = url, modelo = "TecnicaMarcoPieza", lista = "lista", idPieza = PiezaID });

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
