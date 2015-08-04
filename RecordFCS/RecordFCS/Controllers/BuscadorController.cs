using RecordFCS.Helpers;
using RecordFCS.Models;
using RecordFCS.Models.ViewsModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS.Controllers
{
    public class BuscadorController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Buscador
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult MenuFiltros(string rutaVista = "_ResultadosBusqueda")
        {
            ViewBag.rutaVista = rutaVista;
            return PartialView("_MenuFiltros");
        }


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


        public ActionResult RenderListaCoincidencias()
        {
            List<string> lista = (List<string>)TempData["listaValores"];

            if (lista != null)
            {
                ViewBag.totalValores = lista.Count();
            }
            else
            {
                ViewBag.totalValores = 0;
            }

            return PartialView("_ListaCoincidencias", lista);
        }


        [HttpPost]
        public ActionResult MostrarResultados(string rutaVista = "_ResultadosBusqueda", string nombreListaImprimir = "listaDefault")
        {
            //paginador
            int? pagina = Convert.ToInt32(Request.Form["pag"].ToString());
            int? regxpag = Convert.ToInt32(Request.Form["NoResultPag"].ToString());
            int pagTamano = 5;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;
            pagTamano = regxpag.HasValue ? Convert.ToInt32(regxpag) : 5;
            //

            Session[nombreListaImprimir] = new List<long>();

            List<long> listaImprimir = (List<long>)Session[nombreListaImprimir];


            IQueryable<Pieza> listaPiezas = db.Piezas;
            IPagedList<Int64> paginaPiezasIDs;

            //campo de Claves
            if (!String.IsNullOrEmpty(Request["claves"]))
            {
                List<string> listaClaves = new List<string>();
                string[] clavesForm = Request["claves"].Split(',');

                foreach (var claves in clavesForm)
                {
                    if (claves.Contains("-"))
                    {
                        string[] clave = claves.Split('-');
                        var claveInicio = Convert.ToInt32(clave[0]);
                        var claveFinal = Convert.ToInt32(clave[1]);
                        int temp = 0;

                        if (claveInicio > claveFinal)
                        {
                            temp = claveInicio;
                            claveInicio = claveFinal;
                            claveFinal = temp;
                        }

                        for (int i = claveInicio; i <= claveFinal; i++)
                        {
                            listaClaves.Add(i.ToString());
                        }

                    }
                    else
                    {
                        listaClaves.Add(claves);
                    }

                    //listaPiezas = listaPiezas.Where(a => listaClaves.Any(b => b == a.Obra.Clave));

                    listaPiezas = listaPiezas.Where(a => listaClaves.Contains(a.Obra.Clave));
                }

            }


            //primero saber que campos vienen, y con que caracteristicas
            List<Campo> listaCampos = new List<Campo>();

            foreach (var item in Request.Form.Keys)
            {
                if (item.ToString().StartsWith("id_"))
                {
                    var id = Convert.ToInt32(Request.Form[item.ToString()]);
                    var tipoAtt = db.TipoAtributos.Find(id);
                    var valor = Request["valor_" + id];
                    var exactoText = Request["exacto_" + id];
                    bool exacto = false;
                    if (exactoText == "true")
                    {
                        exacto = true;
                    }
                    var campo = new Campo();
                    campo.TipoAtributo = tipoAtt;
                    campo.Valor = valor;
                    campo.Exacto = exacto;
                    listaCampos.Add(campo);
                }
            }

            //implementar el buscador
            foreach (var campo in listaCampos)
            {
                if (campo.TipoAtributo.NombreID == "Generico")
                {
                    //saber si esLista
                    if (campo.TipoAtributo.EsLista)
                    {
                        //como es lista entonces buscar en ListaValor
                        listaPiezas = listaPiezas.Where(a => a.AtributoPiezas.Any(b => b.ListaValor.TipoAtributoID == campo.TipoAtributo.TipoAtributoID && b.ListaValor.Valor.Contains(campo.Valor)));
                    }
                    else
                    {
                        listaPiezas = listaPiezas.Where(a => a.AtributoPiezas.Any(b => b.Atributo.TipoAtributoID == campo.TipoAtributo.TipoAtributoID && b.Valor.Contains(campo.Valor)));
                    }
                }
                else if (campo.TipoAtributo.DatoHTML == "Obra")
                {
                    //todos los campos de Obra
                    switch (campo.TipoAtributo.NombreID)
                    {
                        case "Propietario":
                            //implementar las otras busquedas especiales en Propietario Nombre
                            listaPiezas = listaPiezas.Where(a => a.Obra.Propietario.Nombre.ToString().Contains(campo.Valor));
                            break;
                        case "Coleccion":
                            listaPiezas = listaPiezas.Where(a => a.Obra.Coleccion.Nombre.Contains(campo.Valor));
                            break;
                        case "TipoAdquisicion":
                            listaPiezas = listaPiezas.Where(a => a.Obra.TipoAdquisicion.Nombre.Contains(campo.Valor));
                            break;
                        case "TipoObra":
                            listaPiezas = listaPiezas.Where(a => a.Obra.TipoObra.Nombre.Contains(campo.Valor));
                            break;
                        case "Status":
                            //implementar la busqueda de Status
                            break;
                        case "FechaRegistro":
                            //implementar la busqueda por Fecha de Registro
                            break;
                    }
                }
                else
                {
                    //todos los campos de Pieza
                    switch (campo.TipoAtributo.NombreID)
                    {
                        case "Tecnica":
                            listaPiezas = listaPiezas.Where(a => a.TecnicaPiezas.Any(b => b.Tecnica.Descripcion.Contains(campo.Valor)));
                            break;
                        case "Autor":
                            listaPiezas = listaPiezas.Where(a => a.AutorPiezas.Any(b => b.Autor.Nombre.Contains(campo.Valor)));
                            break;
                        case "Medida":
                            //implementar la busqueda de medida
                            break;
                        case "Catalogo":
                            listaPiezas = listaPiezas.Where(a => a.CatalogoPiezas.Any(b => b.Catalogo.Nombre.Contains(campo.Valor)));
                            break;
                        case "Exposicion":
                            listaPiezas = listaPiezas.Where(a => a.ExposicionPiezas.Any(b => b.Exposicion.Nombre.Contains(campo.Valor)));
                            break;
                        case "TipoPieza":
                            listaPiezas = listaPiezas.Where(a => a.TipoPieza.Nombre.Contains(campo.Valor));
                            break;
                        case "Ubicacion":
                            listaPiezas = listaPiezas.Where(a => a.Ubicacion.Nombre.Contains(campo.Valor));
                            break;
                    }
                }
            }

            // trae todas las piezas, habra piezas que pertenescan a la misma obra
            //listaPiezas = listaPiezas.Where(a => a.TipoPieza.Nombre == "Maestra");
            //var listaIDs = listaPiezas.Select(x => x.PiezaID).ToList();

            //var listaObras = listaPiezas.GroupBy(x => x.ObraID).Select(x => x.FirstOrDefault()).ToList();

            listaPiezas = listaPiezas.OrderBy(a => a.ObraID);


            paginaPiezasIDs = listaPiezas.Select(x => x.PiezaID).ToList().ToPagedList(pagIndex, pagTamano);


            /*crear la lista de imprimir*/

            listaImprimir = paginaPiezasIDs.ToList();
            Session[nombreListaImprimir] = listaImprimir;
            ViewBag.nombreListaImprimir = nombreListaImprimir;

            ViewBag.totalRegistros = listaImprimir.Count();

            return PartialView(rutaVista, paginaPiezasIDs);
        }



        //GET: Buscador/AgregarFiltro
        public ActionResult AgregarFiltro()
        {
            var listaTipoAtt = db.TipoAtributos.Where(a => a.Status == true && a.Buscador == true).OrderBy(a => a.BuscadorOrden);

            ViewBag.TipoAtributoID = new SelectList(listaTipoAtt, "TipoAtributoID", "Nombre");


            return PartialView("_AgregarFiltro");
        }

        //POST: Buscador/AgregarFiltro
        public ActionResult GenerarFiltro(Int64 TipoAtributoID, string Filtro, string PalabraExacta)
        {
            var tipoAtt = db.TipoAtributos.Find(TipoAtributoID);

            if (tipoAtt != null || !String.IsNullOrEmpty(Filtro))
            {
                //extraer nombre para el LABEL
                ViewBag.nombre = tipoAtt.Nombre;
                ViewBag.id = tipoAtt.TipoAtributoID;
                ViewBag.valor = Filtro;

                if (PalabraExacta == "true")
                {
                    ViewBag.exacto = "checked";
                }

                return PartialView("_CampoMenuFiltro");
            }

            return PartialView("_CampoMenuFiltroError");
        }





        [HttpPost]
        public ActionResult MostrarResultadosListado(string rutaVista = "_ResultadosBusqueda")
        {
            //paginador
            int? pagina = Convert.ToInt32(Request.Form["pag"].ToString());
            int? regxpag = Convert.ToInt32(Request.Form["NoResultPag"].ToString());
            int pagTamano = 5;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;
            pagTamano = regxpag.HasValue ? Convert.ToInt32(regxpag) : 5;
            //


            IQueryable<Pieza> listaPiezas = db.Piezas;
            List<PiezaEnCarrito> paginaPiezasEnCarrito;

            //campo de Claves
            if (!String.IsNullOrEmpty(Request["claves"]))
            {
                List<string> listaClaves = new List<string>();
                string[] clavesForm = Request["claves"].Split(',');

                foreach (var claves in clavesForm)
                {
                    if (claves.Contains("-"))
                    {
                        string[] clave = claves.Split('-');
                        var claveInicio = Convert.ToInt32(clave[0]);
                        var claveFinal = Convert.ToInt32(clave[1]);
                        int temp = 0;

                        if (claveInicio > claveFinal)
                        {
                            temp = claveInicio;
                            claveInicio = claveFinal;
                            claveFinal = temp;
                        }

                        for (int i = claveInicio; i <= claveFinal; i++)
                        {
                            listaClaves.Add(i.ToString());
                        }

                    }
                    else
                    {
                        listaClaves.Add(claves);
                    }

                    listaPiezas = listaPiezas.Where(a => listaClaves.Contains(a.Obra.Clave));
                }

            }


            //primero saber que campos vienen, y con que caracteristicas
            List<Campo> listaCampos = new List<Campo>();

            foreach (var item in Request.Form.Keys)
            {
                if (item.ToString().StartsWith("id_"))
                {
                    var id = Convert.ToInt32(Request.Form[item.ToString()]);
                    var tipoAtt = db.TipoAtributos.Find(id);
                    var valor = Request["valor_" + id];
                    var exactoText = Request["exacto_" + id];
                    bool exacto = false;
                    if (exactoText == "true")
                    {
                        exacto = true;
                    }
                    var campo = new Campo();
                    campo.TipoAtributo = tipoAtt;
                    campo.Valor = valor;
                    campo.Exacto = exacto;
                    listaCampos.Add(campo);
                }
            }

            //implementar el buscador
            foreach (var campo in listaCampos)
            {
                if (campo.TipoAtributo.NombreID == "Generico")
                {
                    //saber si esLista
                    if (campo.TipoAtributo.EsLista)
                    {
                        //como es lista entonces buscar en ListaValor
                        listaPiezas = listaPiezas.Where(a => a.AtributoPiezas.Any(b => b.ListaValor.TipoAtributoID == campo.TipoAtributo.TipoAtributoID && b.ListaValor.Valor.Contains(campo.Valor)));
                    }
                    else
                    {
                        listaPiezas = listaPiezas.Where(a => a.AtributoPiezas.Any(b => b.Atributo.TipoAtributoID == campo.TipoAtributo.TipoAtributoID && b.Valor.Contains(campo.Valor)));
                    }
                }
                else if (campo.TipoAtributo.DatoHTML == "Obra")
                {
                    //todos los campos de Obra
                    switch (campo.TipoAtributo.NombreID)
                    {
                        case "Propietario":
                            //implementar las otras busquedas especiales en Propietario Nombre
                            listaPiezas = listaPiezas.Where(a => a.Obra.Propietario.Nombre.ToString().Contains(campo.Valor));
                            break;
                        case "Coleccion":
                            listaPiezas = listaPiezas.Where(a => a.Obra.Coleccion.Nombre.Contains(campo.Valor));
                            break;
                        case "TipoAdquisicion":
                            listaPiezas = listaPiezas.Where(a => a.Obra.TipoAdquisicion.Nombre.Contains(campo.Valor));
                            break;
                        case "TipoObra":
                            listaPiezas = listaPiezas.Where(a => a.Obra.TipoObra.Nombre.Contains(campo.Valor));
                            break;
                        case "Status":
                            //implementar la busqueda de Status
                            break;
                        case "FechaRegistro":
                            //implementar la busqueda por Fecha de Registro
                            break;
                    }
                }
                else
                {
                    //todos los campos de Pieza
                    switch (campo.TipoAtributo.NombreID)
                    {
                        case "Tecnica":
                            listaPiezas = listaPiezas.Where(a => a.TecnicaPiezas.Any(b => b.Tecnica.Descripcion.Contains(campo.Valor)));
                            break;
                        case "Autor":
                            listaPiezas = listaPiezas.Where(a => a.AutorPiezas.Any(b => b.Autor.Nombre.Contains(campo.Valor)));
                            break;
                        case "Medida":
                            //implementar la busqueda de medida
                            break;
                        case "Catalogo":
                            listaPiezas = listaPiezas.Where(a => a.CatalogoPiezas.Any(b => b.Catalogo.Nombre.Contains(campo.Valor)));
                            break;
                        case "Exposicion":
                            listaPiezas = listaPiezas.Where(a => a.ExposicionPiezas.Any(b => b.Exposicion.Nombre.Contains(campo.Valor)));
                            break;
                        case "TipoPieza":
                            listaPiezas = listaPiezas.Where(a => a.TipoPieza.Nombre.Contains(campo.Valor));
                            break;
                        case "Ubicacion":
                            //implementar la busqueda de ubicaciones de piezas
                            break;
                    }
                }
            }

            // trae todas las piezas, habra piezas que pertenescan a la misma obra
            //listaPiezas = listaPiezas.Where(a => a.TipoPieza.Nombre == "Maestra");
            //var listaIDs = listaPiezas.Select(x => x.PiezaID).ToList();

            //var listaObras = listaPiezas.GroupBy(x => x.ObraID).Select(x => x.FirstOrDefault()).ToList();

            listaPiezas = listaPiezas.OrderBy(a => a.ObraID);

            ViewBag.totalRegistros = listaPiezas.Count();

            paginaPiezasEnCarrito = listaPiezas.Select(x => new PiezaEnCarrito()
            {
                ObraID = x.ObraID,
                ClaveObra = x.Obra.Clave,
                PiezaID = x.PiezaID,
                ClavePieza = x.Clave,
                Titulo = x.AtributoPiezas.FirstOrDefault(y => y.Atributo.TipoAtributo.AntNombre == "titulo").Valor,
                Autor = x.AutorPiezas.Select(z => z.Autor.Nombre + " " + z.Autor.Apellido).FirstOrDefault(),
                RutaImagen = x.ImagenPiezas.FirstOrDefault() == null ? RutaImagen.RutaMini_Default : RutaImagen.RutaMini + x.ImagenPiezas.FirstOrDefault().ImgNombre
            }).ToList();

            var listaPiezasAgrupada = paginaPiezasEnCarrito.GroupBy(a => a.ObraID).ToList().ToPagedList(pagIndex, pagTamano);
            return PartialView(rutaVista, listaPiezasAgrupada);
        }


        
        
    }
}