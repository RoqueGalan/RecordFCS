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
using System.IO;
using RecordFCS.Helpers;

namespace RecordFCS.Controllers
{
    public class PiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Pieza/Detalles/5
        [CustomAuthorize(permiso = "ObraFichComplet")]
        public ActionResult Detalles(Int64? piezaID)
        {
            if (piezaID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.Find(piezaID);

            if (pieza == null)
            {
                return HttpNotFound();
            }

            //la pieza es maestra
            if (pieza.TipoPieza.EsMaestra)
            {
                switch (pieza.Obra.Status)
                {
                    case Status.Inactivo:
                        ViewBag.ColorBarra = "barraEstado-default";
                        break;
                    case Status.PreRegistro:
                        ViewBag.ColorBarra = "barraEstado-black";
                        break;
                    case Status.Activo:
                        ViewBag.ColorBarra = "barraEstado-success";
                        break;
                }
            }
            else
            {
                if (pieza.Status)
                    ViewBag.ColorBarra = "barraEstado-success";
                else
                    ViewBag.ColorBarra = "barraEstado-default";
            }

            pieza.TipoPieza.Atributos = pieza.TipoPieza.Atributos.Where(a => a.Status && a.TipoAtributo.AntNombre != "m_pieza_foto").OrderBy(a => a.Orden).ToList();
            pieza.AtributoPiezas = pieza.AtributoPiezas.Where(a => a.Valor != null || a.ListaValorID != null).ToList();

            return PartialView("_DetallesMaestraMejorada", pieza);
        }



        // GET: Pieza/Detalles/5
        [CustomAuthorize]
        public ActionResult FichaBasica(Int64? idPieza)
        {
            if (idPieza == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.SingleOrDefault(p => p.PiezaID == idPieza);

            if (pieza == null)
                return HttpNotFound();

            pieza.AtributoPiezas = pieza.AtributoPiezas.Where(ap => ap.Atributo.EnFichaBasica == true).OrderBy(ap => ap.Atributo.Orden).ToList();



            var imgRequerido = pieza.TipoPieza.Atributos.Where(a => a.EnFichaBasica && a.Status && a.TipoAtributo.AntNombre == "m_pieza_foto").FirstOrDefault();

            if (imgRequerido != null)
            {
                ViewBag.verImagen = "si";

                if (pieza.ImagenPiezas.Count > 0)
                {
                    ViewBag.tieneImagen = "si";
                    pieza.ImagenPiezas = pieza.ImagenPiezas.OrderBy(a => a.Orden).ToList();
                }
                else
                {
                    ViewBag.tieneImagen = "no";
                }

            }
            else
            {
                ViewBag.verImagen = "no";
            }

            pieza.TipoPieza.Atributos = pieza.TipoPieza.Atributos.Where(a => a.EnFichaBasica && a.Status && a.TipoAtributo.AntNombre != "m_pieza_foto").ToList();

            return PartialView("_FichaBasicaMejorada", pieza);
        }



        // GET: Pieza/Crear/ObraID
        [CustomAuthorize(permiso = "ObraAddPieza")]
        public ActionResult Crear(Int64? obraID)
        {
            if (obraID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(obraID);
            if (obra == null)
            {
                return HttpNotFound();
            }

            //Llenar Datos de Pieza
            Pieza pieza = new Pieza()
            {
                ObraID = obra.ObraID,
                Status = true
            };

            //lista de otras piezas qe se pueden agregar a la obra
            //no mostrar las maestras
            //una obra solo puede contener una pieza maestra

            ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas.Where(a => a.TipoObraID == obra.TipoObraID && a.EsMaestra == false).OrderBy(a => a.Nombre), "TipoPiezaID", "Nombre");

            return PartialView("_Crear", pieza);
        }

        // POST: Pieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ObraAddPieza")]
        public ActionResult Crear([Bind(Include = "PiezaID,ObraID,TipoPiezaID,Status")] Pieza pieza, string Folio)
        {
            Obra obra = db.Obras.Find(pieza.ObraID);
            TipoPieza tipoPieza = db.TipoPiezas.Find(pieza.TipoPiezaID);

            pieza.FechaRegistro = DateTime.Now;
            pieza.UbicacionID = obra.UbicacionID;

            if (obra.Status == Status.Activo)
                pieza.Status = true;
            else
                pieza.Status = false;
                
            

            //lista de atributos
            var listaAtributos = db.Atributos.Where(a => a.TipoPiezaID == pieza.TipoPiezaID).ToList();

            // filtrar los atributos que son solo de la PIEZA
            foreach (var item in listaAtributos)
            {
                string campo = "req_list_" + item.TipoAtributoID;
                Int64 id = Convert.ToInt64(Request.Form[campo]);

                switch (item.TipoAtributo.DatoCS)
                {
                    case "UbicacionID":
                        var ubicacion = db.Ubicaciones.Find(id);
                        if (ubicacion != null)
                        {
                            pieza.UbicacionID = id;
                        }
                        break;
                }
            }

            //validar la clave al final
            var clave = "";
            var listaClaves = db.Piezas.Where(p => (p.ObraID == pieza.ObraID && p.TipoPiezaID == pieza.TipoPiezaID)).ToList();

            ////GENERAR EN AUTOMATICO LA CLAVE
            if (listaClaves.Count == 0)
                clave = obra.Clave + "-" + tipoPieza.Clave;
            else
            {
                var listaC = new List<Int32>();
                foreach (var item in listaClaves)
                {
                    char x = Convert.ToChar(tipoPieza.Clave);
                    char s = '-';
                    clave = item.Clave.Split(s).Last();
                    clave = clave.Split(x).Last();
                    if (clave != "")
                        listaC.Add(Convert.ToInt32(clave));
                }
                if (listaC.Count == 0)
                    clave = obra.Clave + "-" + tipoPieza.Clave + "2";
                else
                {
                    listaC.Sort();
                    var mayor = listaC.LastOrDefault(); 
                    mayor = mayor + 1;
                    clave = obra.Clave + "-" + tipoPieza.Clave + mayor;
                }
            }

            pieza.Clave = clave;

            db.Piezas.Add(pieza);
            db.SaveChanges();

            //llenar los atributoPieza y si son requeridos extraer el valor
            foreach (var item in listaAtributos)
            {
                var atributoPieza = new AtributoPieza()
                {
                    PiezaID = pieza.PiezaID,
                    AtributoID = item.AtributoID
                };

                //si son NombreID == "Generico" guardar en AtributoPieza
                if (item.TipoAtributo.NombreID == "Generico")
                {
                    //y si es Requerido extraer el valor del form
                    if (item.Requerido)
                    {
                        //si es lista lo guarda en ValorListaID y si no en Valor
                        if (item.TipoAtributo.EsLista)
                        {
                            string campo = "req_list_" + item.TipoAtributoID;
                            Int64? id = Convert.ToInt64(Request.Form[campo]);
                            var listaValor = db.ListaValores.Find(id);

                            if (listaValor != null)
                            {
                                atributoPieza.ListaValorID = id;
                            }
                        }
                        else
                        {
                            string campo = "req_" + item.TipoAtributoID;
                            string valor = Request.Form[campo];

                            if (!String.IsNullOrEmpty(valor) || !String.IsNullOrWhiteSpace(valor))
                            {
                                atributoPieza.Valor = valor;
                            }
                        }
                    }
                }
                else if (item.TipoAtributo.DatoHTML == "Catalogo")
                {
                    //Si no es generico pero si es DatoHTML = "Catalogo"
                    // y si es Requerido entonces guardar el valor en la TABLAPieza
                    if (item.Requerido)
                    {
                        string campo = "req_list_" + item.TipoAtributoID;
                        int? id = Convert.ToInt32(Request.Form[campo]);
                        switch (item.TipoAtributo.DatoCS)
                        {
                            case "TecnicaPieza":
                                var tecnica = db.Tecnicas.Find(id);
                                if (tecnica != null)
                                {
                                    TecnicaPieza tecPieza = new TecnicaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        TecnicaID = tecnica.TecnicaID,
                                        Status = true
                                    };
                                    db.TecnicaPiezas.Add(tecPieza);
                                    db.SaveChanges();
                                }
                                break;

                            case "AutorPieza":
                                var autor = db.Autores.Find(id);
                                if (autor != null)
                                {
                                    AutorPieza autorPieza = new AutorPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AutorID = autor.AutorID,
                                        Status = true
                                    };
                                    db.AutorPiezas.Add(autorPieza);
                                    db.SaveChanges();
                                }
                                break;

                            case "ImagenPieza":
                                //PEndiente por implementar
                                HttpPostedFileBase FileImagen = Request.Files["FileName"];

                                var extension = Path.GetExtension(FileImagen.FileName);
                                var imagenPieza = new ImagenPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    Status = true,
                                    Orden = 1,
                                    Titulo = Request.Form["imagen_" + "Titulo"],
                                    Descripcion = Request.Form["imagen_" + "Descripcion"],
                                };

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

                                break;

                            case "Medida":
                                //Pendiente por implementar
                                var tipoMedida = db.TipoMedidas.Find(id);
                                if (tipoMedida != null)
                                {
                                    Medida medida = new Medida()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        TipoMedidaID = tipoMedida.TipoMedidaID,
                                        Status = true
                                    };
                                    var xlargo = Request.Form["med_" + "Largo"].ToString();
                                    var xancho = Request.Form["med_" + "Ancho"].ToString();
                                    var xprofundidad = Request.Form["med_" + "Profundidad"].ToString();
                                    var xdiametro = Request.Form["med_" + "Diametro"].ToString();
                                    var xdiametro2 = Request.Form["med_" + "Diametro2"].ToString();
                                    var xUMLongitud = Request.Form["med_" + "UMLongitud"].ToString();

                                    if (xlargo != "0" && xlargo != "")
                                        medida.Largo = Convert.ToDouble(xlargo);
                                    if (xancho != "0" && xancho != "")
                                        medida.Ancho = Convert.ToDouble(xancho);
                                    if (xprofundidad != "0" && xprofundidad != "")
                                        medida.Profundidad = Convert.ToDouble(xprofundidad);
                                    if (xdiametro != "0" && xdiametro != "")
                                        medida.Diametro = Convert.ToDouble(xdiametro);
                                    if (xdiametro2 != "0" && xdiametro2 != "")
                                        medida.Diametro2 = Convert.ToDouble(xdiametro2);

                                    switch (xUMLongitud)
                                    {
                                        case "cm":
                                            medida.UMLongitud = UMLongitud.cm;
                                            break;
                                        case "km":
                                            medida.UMLongitud = UMLongitud.km;
                                            break;
                                        case "m":
                                            medida.UMLongitud = UMLongitud.m;
                                            break;
                                        case "mm":
                                            medida.UMLongitud = UMLongitud.mm;
                                            break;
                                        case "pulgadas":
                                            medida.UMLongitud = UMLongitud.pulgadas;
                                            break;
                                    }
                                    db.Medidas.Add(medida);
                                    db.SaveChanges();

                                }
                                break;

                            case "CatalogoPieza":
                                var catalogo = db.Catalogos.Find(id);
                                if (catalogo != null)
                                {
                                    CatalogoPieza catPieza = new CatalogoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        CatalogoID = catalogo.CatalogoID,
                                        Status = true
                                    };
                                    db.CatalogoPiezas.Add(catPieza);
                                    db.SaveChanges();
                                }
                                break;

                            case "ExposicionPieza":
                                var exposicion = db.Exposiciones.Find(id);
                                if (exposicion != null)
                                {
                                    ExposicionPieza expoPieza = new ExposicionPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        ExposicionID = exposicion.ExposicionID,
                                        Status = true
                                    };
                                    db.ExposicionPiezas.Add(expoPieza);
                                    db.SaveChanges();
                                }
                                break;

                            case "MatriculaPieza":
                                var matricula = db.Matriculas.Find(id);
                                if (matricula != null)
                                {
                                    MatriculaPieza matPieza = new MatriculaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        MatriculaID = matricula.MatriculaID,
                                        Status = true
                                    };
                                    db.MatriculaPiezas.Add(matPieza);
                                    db.SaveChanges();
                                }
                                break;

                            case "TecnicaMarcoPieza":
                                var tenicaMarco = db.TecnicaMarcos.Find(id);
                                if (tenicaMarco != null)
                                {
                                    TecnicaMarcoPieza tecMarcoPieza = new TecnicaMarcoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        TecnicaMarcoID = tenicaMarco.TecnicaMarcoID,
                                        Status = true
                                    };
                                    db.TecnicaMarcoPiezas.Add(tecMarcoPieza);
                                    db.SaveChanges();
                                }
                                break;
                        }
                    }
                }

                db.AtributoPiezas.Add(atributoPieza);
                db.SaveChanges();
            }

            return RedirectToAction("Detalles", "Obra", new { id = obra.ObraID });
        }

        // GET: Pieza/Editar/5
        [CustomAuthorize(permiso = "PiezaEdit")]
        public ActionResult Editar(Int64? piezaID, string campo)
        {
            if (piezaID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(piezaID);
            if (pieza == null)
            {
                return HttpNotFound();
            }

            ViewBag.campo = campo;


            switch (campo)
            {
                case "TipoPiezaID":
                    ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
                    break;
                case "UbicacionID":
                    ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
                    break;
                default:
                    ViewBag.campo = "Error";
                    break;
            }

            return PartialView("_Editar", pieza);
        }

        // POST: Pieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,ObraID,Clave,TipoPiezaID,UbicacionID,Status")] Pieza pieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detalles", "Obra", new { id = pieza.ObraID });

            }

            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);

            return PartialView("_Editar", pieza);
        }


        // GET: Pieza/Eliminar/5
        [CustomAuthorize(permiso = "PiezaEliminar")]
        public ActionResult Eliminar(Int64? id)
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

            return PartialView("_Eliminar", pieza);
        }

        // POST: Pieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Pieza pieza = db.Piezas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    pieza.Status = false;
                    db.Entry(pieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se deshabilito la pieza <b>{0}</b> completamente", pieza.Clave), true);

                    break;
                case "eliminar":
                    db.Piezas.Remove(pieza);
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se elimino la pieza <b>{0}</b>", pieza.Clave), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            return RedirectToAction("Detalles", "Obra", new { id = pieza.ObraID });
        }





        // GET: Pieza/CambiarStatus/5?Estado
        [CustomAuthorize(permiso = "PiezaStatus")]
        public ActionResult CambiarStatus(Int64? id, bool Estado)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pieza pieza = new Pieza()
            {
                PiezaID = Convert.ToInt64(id),
                Status = Estado
            };

            if (Estado)
            {
                ViewBag.PagName = "Activar";
                ViewBag.Estado = ViewBag.PagName;
            }
            else
            {
                ViewBag.PagName = "Desactivar";
                ViewBag.Estado = ViewBag.PagName;
            }

            return PartialView("_CambiarStatus", pieza);
        }

        // POST: Obra/CambiarStatus/5?Estado
        [HttpPost, ActionName("CambiarStatus")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PiezaStatus")]
        public ActionResult CambiarStatusConfirmado(Int64 id)
        {
            Pieza pieza = db.Piezas.Find(id);

            if (pieza != null)
            {
                if (Request.Form["Estado"] == "Activar")
                {
                    pieza.Status = true;
                    AlertaSuccess(string.Format("Se Activo la pieza <b>{0}</b>", pieza.Clave), true);
                }
                else
                {
                    pieza.Status = false;
                    AlertaSuccess(string.Format("Se deshabilito la pieza <b>{0}</b>", pieza.Clave), true);
                }

                db.Entry(pieza).State = EntityState.Modified;
                db.SaveChanges();
            }

            string url = Url.Action("Detalles", "Obra", new { id = pieza.ObraID });
            return Json(new { success = true, url = url, modelo = "Pieza", accion = "recarga" }, JsonRequestBehavior.AllowGet);
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
