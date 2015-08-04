using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using RecordFCS.Helpers;
using RecordFCS.Helpers.Seguridad;

namespace RecordFCS.Controllers
{
    public class ImagenPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        //// GET: ImagenPieza
        //public ActionResult Lista(Int64? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Pieza pieza = db.Piezas.Find(id);
        //    if (pieza == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var imagenPiezas = db.ImagenPiezas.Where(ip => ip.PiezaID == pieza.PiezaID);
        //    ViewBag.PiezaID = pieza.PiezaID;

        //    return PartialView("_Lista", imagenPiezas.ToList());
        //}

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

            string nombreLista = "req_list_" + id;


            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult Carrusel(Int64? id, bool status = false, string tipo = "original")
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null) { return HttpNotFound(); }


            if (status)
            {
                pieza.ImagenPiezas = pieza.ImagenPiezas.Where(i => i.Status == status).OrderBy(i => i.Orden).ToList();
            }
            else
            {
                pieza.ImagenPiezas = pieza.ImagenPiezas.OrderBy(i => i.Orden).ToList();
            }



            var vista = "_Carrusel";

            if (tipo == "thumb")
            {
                vista = "_CarruselThumb";
            }

            ViewBag.PiezaID = id;

            ViewBag.NoImagenes = pieza.ImagenPiezas.Count;

            ViewBag.CarruselID = "Carrusel_" + id;

            return PartialView(vista, pieza.ImagenPiezas);
        }


        //// GET: ImagenPieza/Detalles/5
        //public ActionResult Detalles(Int64? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var imagenPieza = db.ImagenPiezas.Find(id);
        //    if (imagenPieza == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return PartialView("_Detalles", imagenPieza);
        //}


        // GET: ImagenPieza/Crear
        [CustomAuthorize(permiso = "ImagenCrear")]
        public ActionResult Crear(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }

            var imagenPieza = new ImagenPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            var listaImagenes = db.ImagenPiezas.Where(ip => ip.PiezaID == pieza.PiezaID).ToList();
            //GENERAR EN AUTOMATICO LA NUMERACION
            if (listaImagenes.Count == 0)
            {
                imagenPieza.Orden = 1;
            }
            else
            {
                imagenPieza.Orden = listaImagenes.Count + 1;
            }

            return PartialView("_Crear", imagenPieza);
        }


        // POST: ImagenPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ImagenCrear")]
        public ActionResult Crear(ImagenPieza imagenPieza, HttpPostedFileBase FileImagen)
        {

            if (ModelState.IsValid)
            {
                //guardar la imagen en carpeta
                var extension = Path.GetExtension(FileImagen.FileName);
                imagenPieza.ImgNombre = Guid.NewGuid().ToString() + extension;

                var rutaOriginal = Server.MapPath("~" + imagenPieza.Ruta);
                var rutaThumbnail = Server.MapPath("~" + imagenPieza.RutaThumb);

                FileImagen.SaveAs(rutaOriginal);


                //GUARDAR THUMBNAIL
                var thumb = new Thumbnail()
                {
                    OrigenSrc = rutaOriginal,
                    DestinoSrc = rutaThumbnail,
                    LimiteAnchoAlto = 300
                };

                thumb.GuardarThumbnail();



                //guardar en db
                db.ImagenPiezas.Add(imagenPieza);
                db.SaveChanges();



                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });
            }



            return PartialView("_Crear", imagenPieza);
        }


        // GET: ImagenPieza/Editar/5
        [CustomAuthorize(permiso = "ImagenEditar")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", imagenPieza);
        }


        // POST: ImagenPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ImagenEditar")]
        public ActionResult Editar([Bind(Include = "ImagenPiezaID,PiezaID,Orden,Titulo,Descripcion,Status,imgNombre,imgExtension,imgRuta")] ImagenPieza imagenPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imagenPieza).State = EntityState.Modified;
                db.SaveChanges();

                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });

            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "Clave", imagenPieza.PiezaID);
            return View(imagenPieza);
        }


        // GET: ImagenPieza/Eliminar/5
        [CustomAuthorize(permiso = "ImagenEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", imagenPieza);
        }

        // POST: ImagenPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ImagenEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            var imagenPieza = db.ImagenPiezas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    imagenPieza.Status = false;
                    db.Entry(imagenPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", imagenPieza.Titulo), true);

                    break;
                case "eliminar":
                    db.ImagenPiezas.Remove(imagenPieza);
                    db.SaveChanges();

                    //------------ Eliminar el archivo normal y el thumbnail

                    FileInfo infoThumb = new FileInfo(Server.MapPath("~" + imagenPieza.RutaThumb));
                    if (infoThumb.Exists)
                        infoThumb.Delete();
                    else
                        AlertaWarning(string.Format("No se encontro imagen miniatura"), true);

                    FileInfo infoOriginal = new FileInfo(Server.MapPath("~" + imagenPieza.Ruta));
                    if (infoOriginal.Exists)
                        infoOriginal.Delete();
                    else
                        AlertaWarning(string.Format("No se encontro imagen original"), true);


                    // ----------------------------------

                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", imagenPieza.Titulo), true);

                    break;
                default:
                    AlertaDanger(string.Format("No seleccionaste ninguna accion"), true);
                    break;

            }

            string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
            return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });
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
