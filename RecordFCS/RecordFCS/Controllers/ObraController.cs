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
    public class ObraController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();


        // GET: Obra
        [CustomAuthorize(permiso = "ObraListaPlana")]
        public ActionResult Index()
        {
            var obras = db.Obras.Include(o => o.Coleccion).Include(o => o.Propietario).Include(o => o.TipoAdquisicion).Include(o => o.TipoObra).Include(o => o.Ubicacion);

            return View(obras.ToList());
        }


        //Al parecer este ya no se ocupa
        public ActionResult ListaString(string campo, string busqueda, bool exacta)
        {
            IQueryable<Obra> listaTabla;
            List<string> lista = new List<string>();


            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                //agregar logica para cada registro que no sea catalogo
                switch (campo)
                {
                    case "FechaRegistro":
                        //2015-05-11 12:12:05.140
                        //DateTime FechaBusqueda = Convert.ToDateTime(busqueda);            

                        if (exacta)
                        {
                            listaTabla = db.Obras.Where(a => a.FechaRegistro.ToString().StartsWith(busqueda)).OrderBy(a => a.FechaRegistro);
                        }
                        else
                        {
                            listaTabla = db.Obras.Where(a => a.FechaRegistro.ToString().Contains(busqueda)).OrderBy(a => a.FechaRegistro);
                        }

                        if (listaTabla != null)
                        {
                            foreach (var item in listaTabla as IQueryable<Obra>)
                            {
                                lista.Add(item.FechaRegistro.ToString());
                            }
                        }
                        break;

                    default:
                        listaTabla = null;
                        break;
                }
            }

            TempData["listaValores"] = lista;


            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        // GET: Obra/Detalles/5
        [CustomAuthorize(permiso = "ObraFichComplet")]
        public ActionResult Detalles(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                //return HttpNotFound();
            }

            switch (obra.Status)
            {
                case Status.Inactivo:
                    AlertaWarning(string.Format("Esta Obra se encuentra DESACTIVADA, " +
                                                "por tal motivo no puede ser utilizada en el sistema, " +
                                                "solo como consulta."), true);
                    ViewBag.ColorBarra = "barraEstado-default";
                    break;
                case Status.PreRegistro:
                    AlertaWarning(string.Format("Esta Obra se encuentra en PRE-REGISTRO, " +
                                                "por tal motivo no puede ser utilizada en el sistema " +
                                                "hasta que no se complete la verificación."), true);
                    ViewBag.ColorBarra = "barraEstado-black";
                    break;

                case Status.Activo:
                    ViewBag.ColorBarra = "barraEstado-success";
                    break;
            }

            return View("Detalles",obra);
        }


        // GET: Obra/Registro
        [CustomAuthorize(permiso = "ObraRegistrar")]
        public ActionResult Registro()
        {
            Obra obra = new Obra();

            //ultimo registro + 1
            Obra ultimo = db.Obras.ToList().LastOrDefault();
            int folio = 1;

            if (ultimo != null)
            {
                folio = Convert.ToInt32(ultimo.Clave) + 1;
            }
            
            //calcular Folio o Clave temporal de Registro
            //var fecha = DateTime.Now;

            //string folioText = "R" + fecha.DayOfYear + fecha.Year + "-";
            //int folioNum = 1;
            //string folio = folioText + folioNum.ToString().PadLeft(4, '0');

            //while (db.Obras.Where(a => a.Clave == folio).Count() > 0)
            //{
            //    //como no es 0 entonces....
            //    folioNum++;
            //    folio = folioText + folioNum.ToString().PadLeft(4, '0');
            //}

            ViewBag.Folio = folio;


            ViewBag.TipoObraID = new SelectList(db.TipoObras.Where(a => a.Status).Select(a => new {a.TipoObraID, a.Nombre }).OrderBy(a => a.Nombre), "TipoObraID", "Nombre");

            return View();
        }


        // POST: Obra/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ObraRegistrar")]
        public ActionResult Registro(Int64 TipoObraID, Int64 TipoPiezaID, string Folio)
        {

            db.Dispose();
            db = new RecordFCSContext();
            ////Validar el Folio
            //while (db.Obras.Where(a => a.Clave == Folio).Count() > 0)
            //{
            //    string[] nuevaClave = Folio.Split('-');
            //    int numero = Convert.ToInt32(nuevaClave[1]);
            //    numero++;
            //    Folio = nuevaClave[0] + '-' + numero.ToString().PadLeft(4, '0');
            //}




            var obra = new Obra()
            {
                Clave = Folio,
                TipoObraID = TipoObraID,
                FechaRegistro = DateTime.Now,
                Status = Status.PreRegistro,
            };

            //lista de atributos
            var listaAtributos = db.Atributos.Where(a => a.TipoPiezaID == TipoPiezaID).ToList();

            // filtrar los atributos que son solo de la OBRA
            foreach (var item in listaAtributos)
            {
                string campo = "req_list_" + item.TipoAtributoID;
                Int64 id = Convert.ToInt64(Request.Form[campo]);
                switch (item.TipoAtributo.DatoCS)
                {
                    case "TipoAdquisicionID":
                        var tipoAdq = db.TipoAdquisiciones.Where(a => a.TipoAdquisicionID == id).Select(a => new { a.TipoAdquisicionID, a.Nombre}).FirstOrDefault();
                        if (tipoAdq != null)
                        {
                            obra.TipoAdquisicionID = id;
                        }
                        break;

                    case "PropietarioID":
                        var propietario = db.Propietarios.Where(a => a.PropietarioID == id).Select(a => new { a.PropietarioID, a.Nombre}).FirstOrDefault();
                        if (propietario != null)
                        {
                            obra.PropietarioID = id;
                        }
                        break;

                    case "ColeccionID":
                        var coleccion = db.Colecciones.Where(a => a.ColeccionID == id).Select(a => new { a.ColeccionID, a.Nombre}).FirstOrDefault();
                        if (coleccion != null)
                        {
                            obra.ColeccionID = id;
                        }
                        break;

                    case "UbicacionID":
                        var ubicacion = db.Ubicaciones.Where(a => a.UbicacionID == id).Select(a => new { a.UbicacionID, a.Nombre}).FirstOrDefault();
                        if (ubicacion != null)
                        {
                            obra.UbicacionID = id;
                        }
                        break;
                }
            }


            //validar la clave al final
            //ultimo registro + 1
            var ultimo = db.Obras.Select(a => new { a.ObraID, a.TipoObraID, a.Status, a.Clave}).ToList().LastOrDefault();

            if (ultimo != null)
            {
                var folio = Convert.ToInt32(ultimo.Clave) + 1;
                obra.Clave = folio.ToString();
            }

            db.Obras.Add(obra);
            db.SaveChanges();

            // crear la pieza
            var tipoPieza = db.TipoPiezas.Where(a => a.TipoPiezaID == TipoPiezaID).Select(a => new {a.TipoPiezaID, a.TipoObraID, a.Clave }).FirstOrDefault();

            var pieza = new Pieza()
            {
                ObraID = obra.ObraID,
                Clave = obra.Clave + "-" + tipoPieza.Clave, // Rdddaaaa-0000-A
                TipoPiezaID = TipoPiezaID,
                UbicacionID = obra.UbicacionID,
                FechaRegistro = obra.FechaRegistro,
                Status = true
            };

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

                            var listaValor = db.ListaValores.Where(a => a.ListaValorID == id).Select(a => new {a.ListaValorID, a.TipoAtributoID}).FirstOrDefault();

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
                                var tecnica = db.Tecnicas.Where(a => a.TecnicaID == id).Select(a => new { a.TecnicaID, a.Descripcion}).FirstOrDefault();
                                if (tecnica != null)
                                {
                                    TecnicaPieza tecPieza = new TecnicaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        TecnicaID = tecnica.TecnicaID,
                                        Status = true
                                    };
                                    db.TecnicaPiezas.Add(tecPieza);
                                    //db.SaveChanges();
                                }
                                break;

                            case "AutorPieza":
                                var autor = db.Autores.Where(a => a.AutorID == id).Select(a => new { a.AutorID, a.Nombre}).FirstOrDefault();
                                if (autor != null)
                                {
                                    AutorPieza autorPieza = new AutorPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AutorID = autor.AutorID,
                                        Status = true
                                    };
                                    db.AutorPiezas.Add(autorPieza);
                                    //db.SaveChanges();
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
                                //db.SaveChanges();

                                break;

                            case "Medida":
                                //Pendiente por implementar
                                var tipoMedida = db.TipoMedidas.Where(a => a.TipoMedidaID == id).Select(a => new { a.TipoMedidaID, a.Nombre}).FirstOrDefault();
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
                                    var xprofundidad =Request.Form["med_" + "Profundidad"].ToString();
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
                                    //db.SaveChanges();
     
                                }
                                break;

                            case "CatalogoPieza":
                                var catalogo = db.Catalogos.Where(a => a.CatalogoID == id).Select(a => new { a.CatalogoID, a.Nombre}).FirstOrDefault();
                                if (catalogo != null)
                                {
                                    CatalogoPieza catPieza = new CatalogoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        CatalogoID = catalogo.CatalogoID,
                                        Status = true
                                    };
                                    db.CatalogoPiezas.Add(catPieza);
                                    //db.SaveChanges();
                                }
                                break;

                            case "ExposicionPieza":
                                var exposicion = db.Exposiciones.Where(a => a.ExposicionID == id).Select(a => new {a.ExposicionID, a.Nombre }).FirstOrDefault();
                                if (exposicion != null)
                                {
                                    ExposicionPieza expoPieza = new ExposicionPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        ExposicionID = exposicion.ExposicionID,
                                        Status = true
                                    };
                                    db.ExposicionPiezas.Add(expoPieza);
                                    //db.SaveChanges();
                                }
                                break;

                            case "MatriculaPieza":
                                var matricula = db.Matriculas.Where(a => a.MatriculaID == id).Select(a => new { a.MatriculaID, a.Descripcion}).FirstOrDefault();
                                if (matricula != null)
                                {
                                    MatriculaPieza matPieza = new MatriculaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        MatriculaID = matricula.MatriculaID,
                                        Status = true
                                    };
                                    db.MatriculaPiezas.Add(matPieza);
                                    //db.SaveChanges();
                                }
                                break;

                            case "TecnicaMarcoPieza":
                                var tenicaMarco = db.TecnicaMarcos.Where(a => a.TecnicaMarcoID == id).Select(a => new { a.TecnicaMarcoID, a.Descripcion}).FirstOrDefault();
                                if (tenicaMarco != null)
                                {
                                    TecnicaMarcoPieza tecMarcoPieza = new TecnicaMarcoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        TecnicaMarcoID = tenicaMarco.TecnicaMarcoID,
                                        Status = true
                                    };
                                    db.TecnicaMarcoPiezas.Add(tecMarcoPieza);
                                    //db.SaveChanges();
                                }
                                break;
                        }
                    }
                }

                db.AtributoPiezas.Add(atributoPieza);
            }

            db.SaveChanges();


            //redireccionar si se tiene el permiso para ver ficha completa
            if (User.IsInRole("ObraFichComplet"))
            {
                return RedirectToAction("Detalles", "Obra", new { id = obra.ObraID });
            }
            else
            {
                AlertaSuccess(string.Format("Se registro la obra: {0}, pero no tiene los permisos para visualizarla.", obra.Clave),true);
                return RedirectToAction("Registro", "Obra");
            }
        }


        // GET: Obra/Editar/5
        [CustomAuthorize(permiso = "ObraEdit")]
        public ActionResult Editar(Int64? obraID, string campo)
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

            ViewBag.campo = campo;

            switch (campo)
            {
                case "ColeccionID":
                    ViewBag.ColeccionID = new SelectList(db.Colecciones, "ColeccionID", "Nombre", obra.ColeccionID);
                    break;
                case "PropietarioID":
                    ViewBag.PropietarioID = new SelectList(db.Propietarios, "PropietarioID", "Nombre", obra.PropietarioID);
                    break;
                case "TipoAdquisicionID":
                    ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones, "TipoAdquisicionID", "Nombre", obra.TipoAdquisicionID);
                    break;
                case "UbicacionID":
                    ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", obra.UbicacionID);
                    break;
                case "TipoObraID":
                    ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.UbicacionID);
                    break;
                case "Status":
                    ViewBag.campo = campo;
                    break;
                default:
                    ViewBag.campo = "Error";
                    break;
            }

            return PartialView("_Editar", obra);
        }

        // POST: Obra/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ObraEdit")]
        public ActionResult Editar([Bind(Include = "ObraID,Clave,TipoObraID,TipoAdquisicionID,PropietarioID,ColeccionID,UbicacionID,FechaRegistro,Status")] Obra obra)
        {
            string campo = Request.Form["campo"];

            string text_BuscarDato = "";

            if (Request.Form["BuscarDato"] != null)
            {
                text_BuscarDato = Request.Form["BuscarDato"].ToString();
            }

            //saber que campo es el que se esta editando
            //saber si el ID del campo no es null o 0
            //si es null o 0 agregar lo que tiene BuscarDato en la tabla correspondiente al campo

            switch (campo)
            {
                case "ColeccionID":
                    if (obra.ColeccionID == 0)
                    {
                        //es null, entonces agregar valor de busqueda a la tabla CAMPO
                        //extraer el ID y asignarlo a OBRA.CAMPO_ID
                        if (!String.IsNullOrEmpty(text_BuscarDato))
                        {
                            var obj_existe = db.Colecciones.Where(o => o.Nombre == text_BuscarDato).ToList();
                            //si es repetido agregar su ID a OBRA.CAMPO_ID
                            if (obj_existe.Count > 0)
                            {
                                //ya existe
                                obra.ColeccionID = obj_existe.FirstOrDefault().ColeccionID;
                            }
                            else
                            {
                                //no existe
                                var objNew = new Coleccion()
                                {
                                    Nombre = text_BuscarDato,
                                    Status = true
                                };
                                db.Colecciones.Add(objNew);
                                db.SaveChanges();
                                obra.ColeccionID = objNew.ColeccionID;
                            }
                            db.Entry(obra).State = EntityState.Modified;
                            db.SaveChanges();
                            //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                            var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                            return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                        }
                    }
                    else
                    {
                        //No es NULL o 0
                        //Actualizar OBRA.CAMPO_ID
                        db.Entry(obra).State = EntityState.Modified;
                        db.SaveChanges();
                        //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                        var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                        return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                    }

                    ViewBag.ColeccionID = new SelectList(db.Colecciones.Where(o => o.Status == true).OrderBy(o => o.Nombre), "ColeccionID", "Nombre", obra.ColeccionID);

                    break;


                case "PropietarioID":
                    if (obra.PropietarioID == 0)
                    {
                        //es null, entonces agregar valor de busqueda a la tabla CAMPO
                        //extraer el ID y asignarlo a OBRA.CAMPO_ID
                        if (!String.IsNullOrEmpty(text_BuscarDato))
                        {
                            var obj_existe = db.Propietarios.Where(o => o.Nombre.ToString() == text_BuscarDato).ToList();
                            //si es repetido agregar su ID a OBRA.CAMPO_ID
                            if (obj_existe.Count > 0)
                            {
                                //ya existe
                                obra.PropietarioID = obj_existe.FirstOrDefault().PropietarioID;
                            }
                            else
                            {
                                //no existe
                                var objNew = new Propietario()
                                {
                                    Nombre = Convert.ToInt32(text_BuscarDato),
                                    Status = true
                                };
                                db.Propietarios.Add(objNew);
                                db.SaveChanges();
                                obra.PropietarioID = objNew.PropietarioID;
                            }
                            db.Entry(obra).State = EntityState.Modified;
                            db.SaveChanges();
                            //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                            var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                            return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                        }
                    }
                    else
                    {
                        //No es NULL o 0
                        //Actualizar OBRA.CAMPO_ID
                        db.Entry(obra).State = EntityState.Modified;
                        db.SaveChanges();
                        //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                        var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                        return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                    }

                    ViewBag.PropietarioID = new SelectList(db.Propietarios.Where(o => o.Status == true).OrderBy(o => o.Nombre), "PropietarioID", "Nombre", obra.PropietarioID);

                    break;

                case "TipoAdquisicionID":
                    if (obra.TipoAdquisicionID == 0)
                    {
                        //es null, entonces agregar valor de busqueda a la tabla CAMPO
                        //extraer el ID y asignarlo a OBRA.CAMPO_ID
                        if (!String.IsNullOrEmpty(text_BuscarDato))
                        {
                            var obj_existe = db.TipoAdquisiciones.Where(o => o.Nombre == text_BuscarDato).ToList();
                            //si es repetido agregar su ID a OBRA.CAMPO_ID
                            if (obj_existe.Count > 0)
                            {
                                //ya existe
                                obra.TipoAdquisicionID = obj_existe.FirstOrDefault().TipoAdquisicionID;
                            }
                            else
                            {
                                //no existe
                                var objNew = new TipoAdquisicion()
                                {
                                    Nombre = text_BuscarDato,
                                    Status = true
                                };
                                db.TipoAdquisiciones.Add(objNew);
                                db.SaveChanges();
                                obra.TipoAdquisicionID = objNew.TipoAdquisicionID;
                            }
                            db.Entry(obra).State = EntityState.Modified;
                            db.SaveChanges();
                            //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                            var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                            return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                        }
                    }
                    else
                    {
                        //No es NULL o 0
                        //Actualizar OBRA.CAMPO_ID
                        db.Entry(obra).State = EntityState.Modified;
                        db.SaveChanges();
                        //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                        var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                        return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                    }
                    ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones.Where(o => o.Status == true).OrderBy(o => o.Nombre), "TipoAdquisicionID", "Nombre", obra.TipoAdquisicionID);

                    break;

                case "TipoObraID":
                    if (obra.TipoObraID == 0)
                    {

                    }
                    else
                    {
                        //No es NULL o 0
                        //Actualizar OBRA.CAMPO_ID
                        db.Entry(obra).State = EntityState.Modified;
                        db.SaveChanges();
                        //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                        var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                        return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                    }

                    ViewBag.TipoObraID = new SelectList(db.TipoObras.Where(o => o.Status == true).OrderBy(o => o.Nombre), "TipoObraID", "Nombre", obra.TipoObraID);

                    break;

                case "Status":
                    if (obra.Status == 0)
                    {

                    }
                    else
                    {
                        //No es NULL o 0
                        //Actualizar OBRA.CAMPO_ID
                        db.Entry(obra).State = EntityState.Modified;
                        db.SaveChanges();
                        //AlertaSuccess(string.Format("{0}: <b>{1}</b> se edito con exitó.",campo, obra.Coleccion.Nombre), true);
                        var url = Url.Action("CampoValor", "Obra", new { id = obra.ObraID, campo = campo });
                        return Json(new { success = true, url = url, modelo = "Obra", idObra = obra.ObraID, campo = campo });
                    }
                    break;
                default:
                    break;
            }

            ViewBag.campo = campo;

            return PartialView("_Editar", obra);

        }


        [CustomAuthorize]
        public ActionResult CampoValor(Int64? id, string campo)
        {
            Obra obra = db.Obras.Find(id);

            switch (campo)
            {
                case "ColeccionID":
                    ViewBag.Valor = obra.Coleccion.Nombre;
                    break;
                case "PropietarioID":
                    ViewBag.Valor = obra.Propietario.Nombre;
                    break;
                case "TipoAdquisicionID":
                    ViewBag.Valor = obra.TipoAdquisicion.Nombre;
                    break;
                case "UbicacionID":
                    ViewBag.Valor = obra.Ubicacion.Nombre;
                    break;
                case "TipoObraID":
                    ViewBag.Valor = obra.TipoObra.Nombre;
                    break;
                case "Status":
                    ViewBag.Valor = obra.Status;
                    break;
                default:
                    ViewBag.campo = "Error";
                    break;
            }


            return PartialView("_CampoValor");
        }


        // GET: Obra/Eliminar/5
        [CustomAuthorize(permiso = "ObraEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", obra);
        }

        // POST: Obra/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ObraEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Obra obra = db.Obras.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    obra.Status = Status.Inactivo;
                    db.Entry(obra).State = EntityState.Modified;
                    db.SaveChanges();

                    //cambiar todas las piezas a Status false
                    //generar algoritmo
                    AlertaSuccess(string.Format("Se deshabilito la obra <b>{0}</b> completamente", obra.Clave), true);

                    break;
                case "eliminar":
                    //eliminar todas las piezas
                    foreach (var p in obra.Piezas.ToList())
                    {
                        foreach (var attPieza in p.AtributoPiezas.ToList())
                        {
                            db.AtributoPiezas.Remove(attPieza);
                            db.SaveChanges();
                        }
                        db.Piezas.Remove(p);
                        db.SaveChanges();
                    }
                    db.Obras.Remove(obra);
                    db.SaveChanges();
                    //eliminar todas las piezas
                    //generar algoritmo
                    AlertaSuccess(string.Format("Se elimino la obra <b>{0}</b>", obra.Clave), true);
                    return RedirectToAction("Index", "Home");

                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            return RedirectToAction("Detalles", "Obra", new { id = obra.ObraID });

        }



        // GET: Obra/CambiarStatus/5?Estado
        [CustomAuthorize(permiso = "ObraStatus")]
        public ActionResult CambiarStatus(Int64? id, bool Estado)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Obra obra = new Obra()
            {
                ObraID = Convert.ToInt64(id),
                Status = Estado ? Status.Activo : Status.Inactivo
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

            return PartialView("_CambiarStatus", obra);
        }


        // POST: Obra/CambiarStatus/5?Estado
        [HttpPost, ActionName("CambiarStatus")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "ObraStatus")]
        public ActionResult CambiarStatusConfirmado(Int64 id)
        {
            Obra obra = db.Obras.Find(id);
            bool estado = false;
            if (obra != null)
            {
                if (Request.Form["Estado"] == "Activar")
                {
                    obra.Status = Status.Activo;
                    estado = true;
                    AlertaSuccess(string.Format("Se Activo la obra <b>{0}</b>", obra.Clave), true);
                }
                else
                {
                    obra.Status = Status.Inactivo;
                    estado = false;
                    AlertaSuccess(string.Format("Se deshabilito la obra <b>{0}</b>", obra.Clave), true);
                }

                db.Entry(obra).State = EntityState.Modified;

                //desactivar la pieza maestra
                var piezaM = obra.Piezas.Where(a => a.TipoPieza.EsMaestra).FirstOrDefault();
                piezaM.Status = estado;
                db.Entry(piezaM).State = EntityState.Modified;
                db.SaveChanges();
            }

            string url = Url.Action("Detalles", "Obra", new { id = obra.ObraID });
            return Json(new { success = true, url = url, modelo = "Obra", accion = "recarga" }, JsonRequestBehavior.AllowGet);
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
