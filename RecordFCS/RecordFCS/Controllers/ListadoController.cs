﻿using RecordFCS.Models;
using RecordFCS.Models.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Helpers;

namespace RecordFCS.Controllers
{
    public class ListadoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Listado
        public ActionResult Index()
        {
            //Mostrar los atributos que sean lista para tratarlos
            //var lista = db.TipoAtributos.Where(a=> a.EsLista && a.Status).OrderBy(a=> a.BuscadorOrden);
            //List<PiezaEnCarrito> listaPiezas = new List<PiezaEnCarrito>();

            //ViewBag.listaPiezas = (List<PiezaEnCarrito>)Session["listaDePiezas"];

            var x = db.Piezas.Take(15).ToList();
            //ViewBag.ListaP = x;

            Session["listaP"] = x;

            return View();
        }


        public ActionResult PiezaAdd(Int64 PiezaID, string NombreLista = "listaTemporal")
        {
            //string mensaje = "";
            var lista = Session[NombreLista] == null ? new List<PiezaEnCarrito>() : (List<PiezaEnCarrito>)Session[NombreLista];
            var recarga = true;
            PiezaEnCarrito pcarrito = lista.SingleOrDefault(a => a.PiezaID == PiezaID);

            if (pcarrito == null)
            {
                pcarrito = ValidarAdd(PiezaID);
                if (pcarrito != null)
                {
                    //agregar la pcarrito a la lista
                    lista.Add(pcarrito);
                    Session[NombreLista] = lista.OrderBy(a => a.ObraID).ThenBy(a => a.PiezaID).ToList();
                    AlertaSuccess(string.Format("({0}) {1} - Se agregó en <b>{2}</b>", pcarrito.PiezaID, pcarrito.Titulo, NombreLista), true);
                }
                else
                {
                    AlertaInverse(string.Format("({0}) - Error. No se puede agregar esta pieza en <b>{1}</b>", PiezaID, NombreLista), true);
                    recarga = false;
                }
            }
            else
            {
                AlertaInverse(string.Format("({0}) {1} - Ya esta en <b>{2}</b>", pcarrito.PiezaID, pcarrito.Titulo, NombreLista), true);
                recarga = false;
            }

            return Json(new { success = true, lista = NombreLista, recarga = recarga }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PiezaDel(Int64 PiezaID, string NombreLista = "listaTemporal")
        {
            //string mensaje = "";
            var lista = Session[NombreLista] == null ? new List<PiezaEnCarrito>() : (List<PiezaEnCarrito>)Session[NombreLista];
            var recarga = true;

            PiezaEnCarrito pcarrito = lista.SingleOrDefault(a => a.PiezaID == PiezaID);

            if (lista.Remove(pcarrito))
            {
                AlertaWarning(string.Format("({0}) {1} - Se eliminó de <b>{2}</b>", pcarrito.PiezaID, pcarrito.Titulo, NombreLista), true);
            }
            else
            {
                AlertaInverse(string.Format("({0}) - Error. No se puede eliminar esta pieza de <b>{1}</b>", PiezaID, NombreLista), true);
                recarga = false;
            }

            return Json(new { success = true, lista = NombreLista, recarga = recarga }, JsonRequestBehavior.AllowGet);
        }


        public PiezaEnCarrito ValidarAdd(Int64 PiezaID)
        {
            PiezaEnCarrito piezaEnCarrito = null;
            var pieza = db.Piezas.Where(a => a.PiezaID == PiezaID).
                Select(a => new
                {
                    a.PiezaID,
                    a.ObraID,
                    ClavePieza = a.Clave,
                    ClaveObra = a.Obra.Clave,
                    Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.AntNombre == "titulo").Valor,
                    Autor = a.AutorPiezas.Select(c => c.Autor.Nombre + " " + c.Autor.Apellido).FirstOrDefault(),
                    Imagen = a.ImagenPiezas.FirstOrDefault()
                }).FirstOrDefault();

            if (pieza != null)
            {
                piezaEnCarrito = new PiezaEnCarrito()
                {
                    ObraID = pieza.ObraID,
                    ClaveObra = pieza.ClaveObra,
                    PiezaID = pieza.PiezaID,
                    ClavePieza = pieza.ClavePieza,
                    Titulo = pieza.Titulo,
                    Autor = pieza.Autor,
                    RutaImagen = pieza.Imagen == null ? RutaImagen.RutaMini_Default : pieza.Imagen.RutaThumb
                };

            }
            return piezaEnCarrito;
        }


        public ActionResult RenderListaTemporal(string NombreLista = "listaTemporal")
        {
            var lista = Session[NombreLista] == null ? new List<PiezaEnCarrito>() : (List<PiezaEnCarrito>)Session[NombreLista];

            var listaAgrupada = lista.GroupBy(a => a.ObraID).ToList();





            //return Json(new { success = true, total = lista.Count, lista = NombreLista});
            ViewBag.totalLT = lista.Count;
            return PartialView("_ListaTemporal", listaAgrupada);
        }


        public ActionResult MenuImprimir(string Accion = "Plano", string NombreLista = "listaDefault")
        {
            var opcion = new OpcionesImprimir()
            {
                Accion = Accion,
                NombreLista = NombreLista,
                Fecha = true,
                Ubicacion = false,
                Unir = 0,
                IncluirPiezas = false,
                FondoAgua = true,
                NoColumnas = 1,
                Linea = 0,
                MostrarDatos = 0,
                NombreLogotipo = 1
            };

            //saber cuantos registros tiene la lista Default

            var lista = Session[opcion.NombreLista] == null ? new List<long>() : (List<long>)Session[opcion.NombreLista];

            opcion.NoElementos = lista.Count;



            return PartialView("_MenuImprimir", opcion);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerarFormato(OpcionesImprimir opcion)
        {
            var vista = PartialView();

            var listadePiezasID = Session[opcion.NombreLista] == null ? new List<long>() : (List<long>)Session[opcion.NombreLista];
            IQueryable<Pieza> listaPiezas = null;

            //listado default plano o excel
            List<itemListaBasicaDefault> listaBasicaDefault = new List<itemListaBasicaDefault>();
            List<itemPiezaGenerica> listaPiezasGenerica = new List<itemPiezaGenerica>();
            //listado 


            //opcion incluir todas las piezas 
            if (opcion.IncluirPiezas)
            {
                var listadeObrasID = db.Obras.Where(a => a.Piezas.Any(b => listadePiezasID.Contains(b.PiezaID))).Select(c => c.ObraID).ToList();
                listaPiezas = db.Piezas.Where(a => listadeObrasID.Contains(a.ObraID));
            }
            else
            {
                listaPiezas = db.Piezas.Where(a => listadePiezasID.Contains(a.PiezaID));
            }


            //opcion datos basicos o completos
            switch (opcion.Accion)
            {
                case "Word":
                case "Imprimir":
                case "Etiqueta":

                    //Basicos - FichaBasica
                    //saber cuales son los atributos basicos de cada pieza
                    foreach (var grupo in listaPiezas.GroupBy(a => a.TipoPieza).ToList())
                    {
                        IEnumerable<Atributo> listaAtributos = null;

                        if (opcion.MostrarDatos == 0)
                            listaAtributos = grupo.Key.Atributos.Where(a => a.EnFichaBasica && a.Status).OrderBy(b => b.Orden).ToList();
                        else
                            listaAtributos = grupo.Key.Atributos.Where(a => a.Status).ToList();

                        //recorrer pieza a pieza
                        foreach (var pieza in grupo)
                        {
                            //crear pieza generica
                            var x = new itemPiezaGenerica()
                            {
                                ObraID = pieza.ObraID,
                                ObraClave = pieza.Obra.Clave,
                                PiezaID = pieza.PiezaID,
                                PiezaClave = pieza.Clave,
                                itemPiezaGenericaCampos = new List<itemPiezaGenericaCampo>()
                            };

                            //agregar sus atributos
                            foreach (var att in listaAtributos)
                            {
                                var campo = new itemPiezaGenericaCampo()
                                {
                                    itemPiezaGenerica = x,
                                    PiezaID = x.PiezaID,
                                    NombreCampo = att.TipoAtributo.Nombre,
                                    Orden = att.Orden
                                };

                                //extraer los valores

                                switch (att.TipoAtributo.AntNombre)
                                {
                                    case "TipoObjeto_Clave":
                                        campo.ValorCampo = pieza.Obra.TipoObra.Nombre;
                                        break;
                                    case "TipoPieza_Clave":
                                        campo.ValorCampo = pieza.TipoPieza.Nombre;
                                        break;
                                    case "m_pieza_coleccion":
                                        if (pieza.Obra.ColeccionID != null)
                                            campo.ValorCampo = pieza.Obra.Coleccion.Nombre;
                                        break;
                                    case "ClassColeccion_Clave":
                                        //PENDIENTE POR IMPLEMENTAR
                                        break;
                                    case "Autor_Clave":
                                        foreach (var item in pieza.AutorPiezas)
                                        {
                                            campo.ValorCampo += item.Autor.Nombre + " " + item.Autor.Apellido + " ";
                                        }
                                        break;
                                    case "m_pieza_dimensiones":
                                        foreach (var item in pieza.Medidas)
                                        {
                                            var medida = item.Largo.HasValue ? item.Largo + "" : "";
                                            medida += item.Ancho.HasValue ? "x" + item.Ancho : "";
                                            medida += item.Profundidad.HasValue ? "x" + item.Profundidad : "";
                                            medida += item.Diametro.HasValue ? "x" + item.Diametro + "Ø" : "";
                                            medida += item.Diametro2.HasValue ? "x" + item.Diametro2 + "Ø" : "";
                                            medida += item.UMLongitud;
                                            medida += item.Peso.HasValue ? ", " + item.Peso + item.UMMasa : "";

                                            campo.ValorCampo += medida;
                                        }
                                        break;
                                    case "m_cats":
                                        foreach (var item in pieza.CatalogoPiezas)
                                        {
                                            campo.ValorCampo += item.Catalogo.Nombre + " ";
                                        }
                                        break;
                                    case "m_guion_det":
                                        foreach (var item in pieza.ExposicionPiezas)
                                        {
                                            campo.ValorCampo += item.Exposicion.Nombre + " ";
                                        }
                                        break;
                                    case "MatriculaTecnica_Clave":
                                        foreach (var item in pieza.TecnicaPiezas)
                                        {
                                            campo.ValorCampo += item.Tecnica.Descripcion + " ";
                                        }
                                        break;
                                    case "Matricula_Clave":
                                        foreach (var item in pieza.MatriculaPieza)
                                        {
                                            campo.ValorCampo += item.Matricula.Descripcion + " ";
                                        }
                                        break;
                                    case "MTecnicaMarco_Clave":
                                        foreach (var item in pieza.TecnicaMarcoPieza)
                                        {
                                            campo.ValorCampo += item.TecnicaMarco.Descripcion + " ";
                                        }
                                        break;
                                    case "fecha_registro_ORI":
                                        campo.ValorCampo = pieza.Obra.FechaRegistro.ToString();
                                        break;
                                    case "fecha_registro":
                                        campo.ValorCampo = pieza.FechaRegistro.ToString();
                                        break;
                                    case "catTipoAdquisicion":
                                        if (pieza.Obra.TipoAdquisicionID != null)
                                        {
                                            campo.ValorCampo = pieza.Obra.TipoAdquisicion.Nombre;
                                        }
                                        break;
                                    case "Propietario_Clave":
                                        if (pieza.Obra.PropietarioID != null)
                                        {
                                            campo.ValorCampo = pieza.Obra.Propietario.Nombre.ToString();
                                        }
                                        break;
                                    case "Ubicacion_Clave (OBRA)":
                                        if (pieza.UbicacionID != null)
                                        {
                                            campo.ValorCampo = pieza.Ubicacion.Nombre;
                                        }
                                        break;
                                    case "estatus":
                                    case "estatus_pieza":
                                        campo.ValorCampo = pieza.Status ? "Activa" : "Desactivada";
                                        break;

                                    case "m_pieza_foto":
                                        var Imagen = pieza.ImagenPiezas.FirstOrDefault();
                                        x.RutaImagen = Imagen == null ? RutaImagen.RutaMini_Default : Imagen.RutaThumb;
                                        break;

                                    default:
                                        if (att.TipoAtributo.NombreID == "Generico")
                                        {
                                            var attPieza = pieza.AtributoPiezas.SingleOrDefault(a => a.AtributoID == att.AtributoID);
                                            if (attPieza != null)
                                            {
                                                if (att.TipoAtributo.EsLista)
                                                {
                                                    if (attPieza.ListaValorID != null)
                                                        campo.ValorCampo = attPieza.ListaValor.Valor;
                                                }
                                                else
                                                    campo.ValorCampo = attPieza.Valor;
                                            }
                                        }
                                        break;
                                }

                                if (!string.IsNullOrWhiteSpace(campo.ValorCampo))
                                {
                                    x.itemPiezaGenericaCampos.Add(campo);
                                }
                            }

                            x.itemPiezaGenericaCampos = x.itemPiezaGenericaCampos.OrderBy(a => a.Orden).ToList();
                            //agregar la pieza generica a la lista
                            listaPiezasGenerica.Add(x);

                        }


                    }


                    break;

                case "Plano":
                case "Excel":
                default:
                    //BASICOS DEFAULT [Plano y Excel] ----------------------------------------------------------------------
                    var listaTemporal = listaPiezas.Select(a => new
                    {
                        a.PiezaID,
                        a.ObraID,
                        ClavePieza = a.Clave,
                        ClaveObra = a.Obra.Clave,
                        Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.AntNombre == "titulo").Valor,
                        Autor = a.AutorPiezas.Select(c => c.Autor.Nombre + " " + c.Autor.Apellido).FirstOrDefault(),
                        TecnicaPieza = a.TecnicaPiezas.FirstOrDefault(),
                        MedidaPieza = a.Medidas.OrderBy(e => e.TipoMedidaID).FirstOrDefault(),
                        Fecha = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.AntNombre == "FechaEjecucion_Clave").ListaValor,
                        Ubicacion = a.Ubicacion,
                        Imagen = a.ImagenPiezas.FirstOrDefault()
                    });
                    foreach (var pieza in listaTemporal.ToList())
                    {
                        //crear la pieza generica
                        var x = new itemPiezaGenerica()
                        {
                            ObraID = pieza.ObraID,
                            ObraClave = pieza.ClaveObra,
                            PiezaID = pieza.PiezaID,
                            PiezaClave = pieza.ClavePieza,
                        };
                        //crear sus atributos que tendre la pieza generica
                        var imagen_tmp = new itemPiezaGenericaCampo() { Orden = 0, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Imagen", ValorCampo = pieza.Imagen == null ? RutaImagen.RutaMini_Default : pieza.Imagen.RutaThumb };
                        var titulo_tmp = new itemPiezaGenericaCampo() { Orden = 1, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Título", ValorCampo = pieza.Titulo };
                        var autor_tmp = new itemPiezaGenericaCampo() { Orden = 2, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Autor(es)", ValorCampo = pieza.Autor };
                        var tecnica_tmp = new itemPiezaGenericaCampo() { Orden = 3, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Técnica", ValorCampo = pieza.TecnicaPieza == null ? "" : pieza.TecnicaPieza.Tecnica.Descripcion };
                        var fecha_tmp = new itemPiezaGenericaCampo() { Orden = 4, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Fecha", ValorCampo = pieza.Fecha == null ? "" : pieza.Fecha.Valor };
                        var ubicacion_tmp = new itemPiezaGenericaCampo() { Orden = 5, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Ubicación", ValorCampo = pieza.Ubicacion == null ? "" : pieza.Ubicacion.Nombre };
                        var medida_tmp = new itemPiezaGenericaCampo() { Orden = 6, itemPiezaGenerica = x, PiezaID = x.PiezaID, NombreCampo = "Medida(s)", ValorCampo = "" };
                        if (pieza.MedidaPieza != null)
                        {
                            medida_tmp.ValorCampo = string.IsNullOrWhiteSpace(pieza.MedidaPieza.Largo.ToString()) ? "" : "x: " + pieza.MedidaPieza.Largo + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Ancho.ToString()) ? "" : "y: " + pieza.MedidaPieza.Ancho.ToString() + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Profundidad.ToString()) ? "" : "z: " + pieza.MedidaPieza.Profundidad.ToString() + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Diametro.ToString()) ? "" : "θ: " + pieza.MedidaPieza.Diametro.ToString() + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Diametro2.ToString()) ? "" : "θ: " + pieza.MedidaPieza.Diametro2.ToString() + " ";
                            medida_tmp.ValorCampo += pieza.MedidaPieza.UMLongitud.ToString() == null ? "" : pieza.MedidaPieza.UMLongitud + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Peso.ToString()) ? "" : "" + pieza.MedidaPieza.Peso.ToString() + " ";
                            medida_tmp.ValorCampo += pieza.MedidaPieza.UMMasa.ToString() == null ? "" : pieza.MedidaPieza.UMMasa + " ";
                            medida_tmp.ValorCampo += string.IsNullOrWhiteSpace(pieza.MedidaPieza.Otra) ? "" : "" + pieza.MedidaPieza.Otra;
                        }
                        //agregarlos a la pieza generica
                        x.itemPiezaGenericaCampos = new List<itemPiezaGenericaCampo>() {
                        imagen_tmp, 
                        titulo_tmp,
                        autor_tmp,
                        tecnica_tmp,
                        fecha_tmp,
                        ubicacion_tmp,medida_tmp
                    };
                        //agregar la pieza generica a la lista
                        listaPiezasGenerica.Add(x);
                    }
                    break;
            }






            //if (opcion.Unir == 1)
            //{
            //    var listaPiezasAgrupada = listaPiezas.GroupBy(a => a.Obra).ToList();
            //}

            listaPiezasGenerica = listaPiezasGenerica.OrderBy(a=> a.ObraID).ToList();



            //Redireccionar a la vista correspondiente
            switch (opcion.Accion)
            {

                case "Excel":
                case "Plano":
                default:
                    vista = PartialView("_ListaTextoPlano", listaPiezasGenerica);
                    break;

                case "Word":
                case "Imprimir":
                case "Etiqueta":
                    vista = PartialView("_ListaEtiquetas",listaPiezasGenerica);
                    break;
            }



            //logo
            if (opcion.NombreLogotipo == 1)
                ViewBag.logo = "Content/img/listado/headMS_plano.gif";
            else
                ViewBag.logo = "Content/img/listado/headFCS_plano.png";


            //if (opcion.Accion == "Plano")
            //{

            //    //return new PdfActionResult("PDF_TextoPlano", listaBasicaDefault, (write, documento) =>
            //    //{
            //    //    documento.SetPageSize(new iTextSharp.text.Rectangle(612f, 792f, 0));
            //    //    documento.NewPage();
            //    //})
            //    //{
            //    //    FileDownloadName = string.Format("{0}-{1:MMMM yyyy}.pdf", opcion.Accion, DateTime.Now)
            //    //};


            //}



            return vista;
        }

    }
}