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
    public class CatalogoPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: CatalogoPieza
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

            return PartialView("_Lista", pieza.CatalogoPiezas.ToList());
        }


        // GET: CatalogoPieza/Crear
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

            var catalogoPieza = new CatalogoPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            return PartialView("_Crear", catalogoPieza);
        }

        // POST: CatalogoPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaAdd")]
        public ActionResult Crear([Bind(Include = "PiezaID,CatalogoID,Status")] CatalogoPieza catalogoPieza)
        {
            catalogoPieza.Status = true;
            //veridicar que CatalogoID NULL ó 0
            if (catalogoPieza.CatalogoID == 0)
            {
                //si es NULL ó 0
                //agregar valor de la busqueda a la tabla CATALOGO
                //extraer el ID y asignarlo a CATALOGOPIEZA


                //validar que BuscarDato no sea "" o NULL
                var text_BuscarDato = Request.Form["BuscarDato"].ToString();
                if (!String.IsNullOrEmpty(text_BuscarDato))
                {
                    var cat_existe = db.Catalogos.Where(cat => cat.Nombre == text_BuscarDato).ToList();
                    // si es repetido agregar su ID a CatalogoPieza
                    if (cat_existe.Count > 0)
                    {
                        //ya existe
                        catalogoPieza.CatalogoID = cat_existe.FirstOrDefault().CatalogoID;
                        db.Entry(catalogoPieza).State = EntityState.Modified;
                    }
                    else
                    {
                        //no existe
                        var catalogoNew = new Catalogo()
                        {
                            Nombre = text_BuscarDato,
                            Status = true
                        };

                        db.Catalogos.Add(catalogoNew);
                        db.SaveChanges();

                        catalogoPieza.CatalogoID = catalogoNew.CatalogoID;
                        db.CatalogoPiezas.Add(catalogoPieza);
                    }

                    db.SaveChanges();
                    //AlertaSuccess(string.Format("Catalogo: <b>{0}</b> se agrego con exitó.", catalogo.Nombre), true);
                    string url = Url.Action("Lista", "CatalogoPieza", new { id = catalogoPieza.PiezaID });
                    return Json(new { success = true, url = url, modelo = "CatalogoPieza", lista = "lista", idPieza = catalogoPieza.PiezaID });
                }
            }
            else
            {
                //no es NULL ó 0
                //verificar que no exista ya el registro para la pieza
                var catPieza_existe = db.CatalogoPiezas.Where(cat => cat.PiezaID == catalogoPieza.PiezaID && cat.CatalogoID == catalogoPieza.CatalogoID).ToList();
                // si es repetido agregar su ID a CatalogoPieza
                if (catPieza_existe.Count <= 0)
                {
                    //crear
                    db.CatalogoPiezas.Add(catalogoPieza);
                    db.SaveChanges();
                }

                //AlertaSuccess(string.Format("Catalogo: <b>{0}</b> se agrego con exitó.", catalogo.Nombre), true);
                string url = Url.Action("Lista", "CatalogoPieza", new { id = catalogoPieza.PiezaID });
                return Json(new { success = true, url = url, modelo = "CatalogoPieza", lista = "lista", idPieza = catalogoPieza.PiezaID });
            }


            ViewBag.CatalogoID = new SelectList(db.Catalogos, "CatalogoID", "Nombre", catalogoPieza.CatalogoID);

            return PartialView("_Crear", catalogoPieza);
        }


        // GET: CatalogoPieza/Eliminar/5
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult Eliminar(Int64? idPieza, Int64? idCatalogo)
        {
            if (idPieza == null || idCatalogo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatalogoPieza catalogoPieza = db.CatalogoPiezas.Find(idPieza, idCatalogo);
            if (catalogoPieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", catalogoPieza);
        }

        // POST: CatalogoPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64? idPieza, Int64? idCatalogo)
        {
            string btnValue = Request.Form["accionx"];

            var catalogoPieza = db.CatalogoPiezas.Find(idPieza, idCatalogo);
            var catalogo = catalogoPieza.Catalogo;

            switch (btnValue)
            {
                case "deshabilitar":
                    catalogoPieza.Status = false;
                    db.Entry(catalogoPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", catalogo.Nombre), true);

                    break;
                case "eliminar":
                    db.CatalogoPiezas.Remove(catalogoPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", catalogo.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "CatalogoPieza", new { id = catalogoPieza.PiezaID });
            return Json(new { success = true, url = url, modelo = "CatalogoPieza", lista = "lista", idPieza = catalogoPieza.PiezaID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [CustomAuthorize]
        public ActionResult RenderBuscarCampo(Int64? idTipoAtributo)
        {
            var tipoAtt = db.TipoAtributos.Find(idTipoAtributo);

            //generar url
            var controlador = "";
            var accion = "ListaString";

            var urlBusqueda = "";

            if (tipoAtt.NombreID == "Generico")
            {
                //Buscar los valores en la tabla ListaValor dependiendo el TipoAtributo
                controlador = "ListaValor";
                urlBusqueda = Url.Action(accion, controlador, new { idTipoAtributo = idTipoAtributo });
            }
            else
            {
                if (tipoAtt.EsLista)
                {
                    //es un catalogo ejem: Controlador/Accion
                    //Url = Autor/ListaString

                    controlador = tipoAtt.NombreID;
                    urlBusqueda = Url.Action(accion, controlador);
                }
                else
                {
                    //es un atributo exclusivo, obra o pieza
                    controlador = tipoAtt.DatoHTML;
                    urlBusqueda = Url.Action(accion, controlador, new { campo = tipoAtt.NombreID });
                }

            }

            ViewBag.rutaAccion = urlBusqueda;

            return PartialView("_RenderBuscarCampo", tipoAtt);
        }
    }
}
