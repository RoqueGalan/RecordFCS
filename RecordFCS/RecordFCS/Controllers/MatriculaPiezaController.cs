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
    public class MatriculaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: MatriculaPieza
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

            return PartialView("_Lista", pieza.MatriculaPieza.ToList());
        }



        // GET: MatriculaPieza/Crear
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

            var matriculaPieza = new MatriculaPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            //ViewBag.MatriculaID = new SelectList(db.Matriculas, "MatriculaID", "ClaveSigla");
            //ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "Clave");
            return PartialView("_Crear", matriculaPieza);
        }

        // POST: MatriculaPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,MatriculaID")] MatriculaPieza matriculaPieza)
        {
            matriculaPieza.Status = true;
            if (matriculaPieza.MatriculaID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla TECNICA
                //extraer el ID y asignarlo a TECNICAPIEZA

                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var matricula_existe = db.Matriculas.Where(a => a.Descripcion == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a MatriculaPieza
                    if (matricula_existe.Count > 0)
                    {
                        //ya existe
                        matriculaPieza.MatriculaID = matricula_existe.FirstOrDefault().MatriculaID;
                        db.Entry(matriculaPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var matriculaNew = new Matricula()
                        {
                            Descripcion = text_BuscarDato,
                            Status = true
                        };

                        db.Matriculas.Add(matriculaNew);
                        db.SaveChanges();

                        matriculaPieza.MatriculaID = matriculaNew.MatriculaID;
                        db.MatriculaPiezas.Add(matriculaPieza);
                    }

                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                    string url = Url.Action("Lista", "MatriculaPieza", new { id = matriculaPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "MatriculaPieza", lista = "lista", idPieza = matriculaPieza.PiezaID });
                }
            }
            else
            {
                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var matPieza_existe = db.MatriculaPiezas.Where(a => a.PiezaID == matriculaPieza.PiezaID).ToList();
                // si es repetido agregar su ID a PiezaPieza
                if (matPieza_existe.Count <= 0)
                {
                    //crear
                    db.MatriculaPiezas.Add(matriculaPieza);
                    db.SaveChanges();
                }
                else
                {
                    //editar
                }
                //AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agrego con exitó.", tecnica.Descripcion), true);
                string url = Url.Action("Lista", "MatriculaPieza", new { id = matriculaPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "MatriculaPieza", lista = "lista", idPieza = matriculaPieza.PiezaID });

            }

            ViewBag.MatriculaID = new SelectList(db.Matriculas, "MatriculaID", "ClaveSigla", matriculaPieza.MatriculaID);

            return PartialView("_Crear", matriculaPieza);
        }


        // GET: MatriculaPieza/Editar/5
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar(Int64? idPieza, Int64? idMatricula)
        {
            if (idPieza == null || idMatricula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatriculaPieza matriculaPieza = db.MatriculaPiezas.Find(idPieza, idMatricula);
            if (matriculaPieza == null)
            {
                return HttpNotFound();
            }

            ViewBag.MatriculaID = new SelectList(db.Matriculas, "MatriculaID", "ClaveSigla", matriculaPieza.MatriculaID);

            return PartialView("_Editar", matriculaPieza);
        }

        // POST: MatriculaPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,MatriculaID,Status")] MatriculaPieza matriculaPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matriculaPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Matricula: <b>{0}</b> se edito con exitó.", matriculaPieza.Matricula.Descripcion), true);
                string url = Url.Action("Lista", "MatriculaPieza", new { id = matriculaPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "MatriculaPieza", lista = "lista", idPieza = matriculaPieza.PiezaID });
            }

            ViewBag.MatriculaID = new SelectList(db.Matriculas, "MatriculaID", "ClaveSigla", matriculaPieza.MatriculaID);

            return PartialView("_Editar", matriculaPieza);
        }

        // GET: MatriculaPieza/Eliminar/5
                [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatriculaPieza matriculaPieza = db.MatriculaPiezas.Find(id);
            if (matriculaPieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", matriculaPieza);
        }

        // POST: MatriculaPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
                public ActionResult EliminarConfirmado(long id)
        {
            string btnValue = Request.Form["accionx"];

            var matriculaPieza = db.MatriculaPiezas.Find(id);
            var matricula = matriculaPieza.Matricula;

            switch (btnValue)
            {
                case "deshabilitar":
                    matriculaPieza.Status = false;
                    db.Entry(matriculaPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", matricula.Descripcion), true);

                    break;
                case "eliminar":
                    db.MatriculaPiezas.Remove(matriculaPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", matricula.Descripcion), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "MatriculaPieza", new { id = matriculaPieza.PiezaID });
            return Json(new { success = true, url = url, modelo = "MatriculaPieza", lista = "lista", idPieza = matriculaPieza.PiezaID });

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
