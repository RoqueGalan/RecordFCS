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
    public class MedidaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        //public ActionResult AgregarValor(string valor, bool aceptar)
        //{
        //    if (aceptar)
        //    {
        //        if (valor != null || valor != "")
        //        {
        //            var lista = db.Catalogos.Where(a => a.Nombre == valor).ToList();
        //            if (lista.Count == 0)
        //            {
        //                var catalogo = new Catalogo()
        //                {
        //                    Nombre = valor,
        //                    Status = true,
        //                };
        //                db.Catalogos.Add(catalogo);
        //                db.SaveChanges();
        //                AlertaSuccess(string.Format("Catálogo: <b>{0}</b> se agregó con exitó.", catalogo.Nombre), true);
        //                return Json(new { success = true, valor = catalogo.CatalogoID, texto = catalogo.Nombre });
        //            }
        //            else
        //            {
        //                AlertaSuccess(string.Format("Catálogo: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
        //                return Json(new { success = true, valor = lista.FirstOrDefault().CatalogoID, texto = lista.FirstOrDefault().Nombre });
        //            }
        //        }
        //    }
        //    return Json(new { success = false });
        //}


        // GET: Medida
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

            var medidas = db.Medidas.Where(m => m.PiezaID == pieza.PiezaID);
            ViewBag.PiezaID = pieza.PiezaID;

            return PartialView("_Lista", medidas.ToList());
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

            //var valores = db.Autores.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            //ViewBag.Valores = new SelectList(valores, "AutorID", "Nombre");

            var valores = db.TipoMedidas.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "TipoMedidaID", "Nombre");

            var valoresUMLongitud = from UMLongitud e in Enum.GetValues(typeof(UMLongitud))
                         select new { Id = e, Name = e.ToString() };

            ViewData["med_UMLongitud"] = new SelectList(valoresUMLongitud, "Id", "Name");


            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        // GET: Tecnica/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            var medida = new Medida();
            var listaMedidas = db.TipoMedidas;

            ViewBag.TipoMedidaID = new SelectList(listaMedidas, "TipoMedidaID", "Nombre");


            return PartialView("_FormLista", medida);
        }


        // GET: Medida/Detalles/idPieza/idTipoMedida
        [CustomAuthorize]
        public ActionResult Detalles(Int64? idPieza, Int64? idTipoMedida)
        {
            if (idPieza == null || idTipoMedida == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medida medida = db.Medidas.Find(idPieza, idTipoMedida);
            if (medida == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Detalles", medida);
        }


        // GET: Medida/Crear
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
            var medida = new Medida()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            //Solo listar los tipos de medidas que no se hayan registrado
            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medida.TipoMedidaID);


            return PartialView("_Crear", medida);
        }


        // POST: Medida/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,TipoMedidaID,Largo,Ancho,Profundidad,Diametro,Diametro2,UMLongitud,Peso,UMMasa,Status")] Medida medida)
        {
            if (ModelState.IsValid)
            {
                medida.Status = true;
                db.Medidas.Add(medida);
                db.SaveChanges();

                var tipoMedida = db.TipoMedidas.Find(medida.TipoMedidaID);

                AlertaSuccess(string.Format("Medida: <b>{0}</b> se agrego con exitó.", tipoMedida.Nombre), true);

                string url = Url.Action("Lista", "Medida", new { id = medida.PiezaID });

                return Json(new { success = true, url = url, modelo = "Medida", lista = "lista", idPieza = medida.PiezaID });

            }

            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medida.TipoMedidaID);
            return PartialView("_Crear", medida);
        }


        // GET: Medida/Editar/5
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar(Int64? idPieza, Int64? idTipoMedida)
        {
            if (idPieza == null || idTipoMedida == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medida medida = db.Medidas.Find(idPieza, idTipoMedida);
            if (medida == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medida.TipoMedidaID);

            return PartialView("_Editar", medida);
        }


        // POST: Medida/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,TipoMedidaID,Largo,Ancho,Profundidad,Diametro,Diametro2,UMLongitud,Peso,UMMasa,Status")] Medida medida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medida).State = EntityState.Modified;
                db.SaveChanges();

                string url = Url.Action("Detalles", "Medida", new { idPieza = medida.PiezaID, idTipoMedida = medida.TipoMedidaID });

                return Json(new { success = true, url = url, idPieza = medida.PiezaID, idTipoMedida = medida.TipoMedidaID });

            }

            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medida.TipoMedidaID);

            return PartialView("_Editar", medida);

        }


        // GET: Medida/_Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idTipoMedida)
        {
            if (idPieza == null || idTipoMedida == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medida medida = db.Medidas.Find(idPieza, idTipoMedida);
            if (medida == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", medida);
        }

        // POST: Medida/_Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64? idPieza, Int64? idTipoMedida)
        {
            string btnValue = Request.Form["accionx"];

            var medida = db.Medidas.Find(idPieza, idTipoMedida);
            var tipoMedida = medida.TipoMedida;

            switch (btnValue)
            {
                case "deshabilitar":
                    medida.Status = false;
                    db.Entry(medida).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoMedida.Nombre), true);

                    break;
                case "eliminar":
                    db.Medidas.Remove(medida);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoMedida.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Medida", new { id = medida.PiezaID });
            return Json(new { success = true, url = url, modelo = "Medida", lista = "lista", idPieza = medida.PiezaID });
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
