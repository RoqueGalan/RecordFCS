using RecordFCS.Models;
using RecordFCS.Models.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS.Controllers
{
    public class ReporteController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Reporte
        public ActionResult Index()
        {
            //Mostrar los atributos que sean lista para tratarlos
            var lista = db.TipoAtributos.Where(a => a.EsLista && a.Status).OrderBy(a => a.BuscadorOrden);

            return View(lista);
        }

        //ReporteAttGenerico
        public ActionResult ReporteBasico(Int64? tipoAtributoID)
        {
            if (tipoAtributoID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoAtt = db.TipoAtributos.Find(tipoAtributoID);

            if (tipoAtt == null)
                return HttpNotFound();

            int totalRegistros = 0;
            int totalObras = db.Obras.Count();
            int totalPiezasMaestras = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
            int totalPiezasOtras = db.Piezas.Where(a => !a.TipoPieza.EsMaestra && a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();

            int estaEnPiezaMaestra = 0;
            int estaEnPiezaMaestraComodato = 0;
            int estaEnPiezaMaestraInactiva = 0;

            List<ItemReporteBasico> lista = new List<ItemReporteBasico>();
            List<ItemReporteBasico> listaComodato = new List<ItemReporteBasico>();
            List<ItemReporteBasico> listaInactiva = new List<ItemReporteBasico>();
            List<ObraReporteBasico> listaObraSinCampo = new List<ObraReporteBasico>();

            int subTotal = 0;
            int subTotalComodato = 0;
            int subTotalInactiva = 0;
            int subTotalObraSinCampo = 0;
            int totalPiezasSinElCampo = 0;


            ViewBag.Soportado = "si";

            switch (tipoAtt.AntNombre)
            {
                case "Autor_Clave":
                    totalRegistros = db.Autores.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.AutorPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Autores.Select(a => new { a.AutorID, a.Nombre, a.Apellido, a.AutorPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre + " " + item.Apellido,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestra = db.AutorPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Autores.Select(a => new { a.AutorID, a.Nombre, a.Apellido, a.AutorPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre + " " + item.Apellido,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.AutorPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Autores.Select(a => new { a.AutorID, a.Nombre, a.Apellido, a.AutorPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre + " " + item.Apellido,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }
                    //OBRAS SIN CAMPO "AUTOR" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && a.AutorPiezas.Any(c => c.Autor.AntID == "0" || c.Autor.Nombre.Contains("definir"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID) && a.AutorPiezas.Any(b => b.Autor.AntID == "0" || b.Autor.Nombre.Contains("definir"))).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;

                // -----------------------------------------------------------------------------------------------

                case "m_cats":
                    totalRegistros = db.Catalogos.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.CatalogoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Catalogos.Select(a => new { a.CatalogoID, a.Nombre, a.CatalogoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.CatalogoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Catalogos.Select(a => new { a.CatalogoID, a.Nombre, a.CatalogoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.CatalogoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Catalogos.Select(a => new { a.CatalogoID, a.Nombre, a.CatalogoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.CatalogoPiezas.Any(c => c.Catalogo.Nombre == ""))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID) && a.CatalogoPiezas.Any(b =>b.Catalogo.Nombre == "")).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;




                // -----------------------------------------------------------------------------------------------

                case "m_guion_det":
                    totalRegistros = db.Exposiciones.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.ExposicionPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Exposiciones.Select(a => new { a.ExposicionID, a.Nombre, a.ExposicionPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.ExposicionPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Exposiciones.Select(a => new { a.ExposicionID, a.Nombre, a.ExposicionPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.ExposicionPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Exposiciones.Select(a => new { a.ExposicionID, a.Nombre, a.ExposicionPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.ExposicionPiezas.Any(c => c.Exposicion.Nombre == ""))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID) && a.ExposicionPiezas.Any(b => b.Exposicion.Nombre == "")).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;


                // -----------------------------------------------------------------------------------------------

                case "Matricula_Clave":
                    totalRegistros = db.Matriculas.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.MatriculaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Matriculas.Select(a => new { a.MatriculaID, a.Descripcion, a.MatriculaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.MatriculaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Matriculas.Select(a => new { a.MatriculaID, a.Descripcion, a.MatriculaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.MatriculaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Matriculas.Select(a => new { a.MatriculaID, a.Descripcion, a.MatriculaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.MatriculaPieza.Any(c => c.Matricula.AntID == "0" || c.Matricula.ClaveSigla == "S/D"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID) && a.MatriculaPieza.Any(b => b.Matricula.AntID == "0" || b.Matricula.ClaveSigla == "S/D")).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;




                // -----------------------------------------------------------------------------------------------

                case "MTecnicaMarco_Clave":
                    totalRegistros = db.TecnicaMarcos.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.TecnicaMarcoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.TecnicaMarcos.Select(a => new { a.TecnicaMarcoID, a.Descripcion, a.TecnicaMarcoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.TecnicaMarcoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.TecnicaMarcos.Select(a => new { a.TecnicaMarcoID, a.Descripcion, a.TecnicaMarcoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.TecnicaMarcoPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.TecnicaMarcos.Select(a => new { a.TecnicaMarcoID, a.Descripcion, a.TecnicaMarcoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.TecnicaMarcoPieza.Any(c => c.TecnicaMarco.AntID == "0"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TecnicaMarcoPieza.Any(b => b.TecnicaMarco.AntID == "0" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID))).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;



                // -----------------------------------------------------------------------------------------------

                case "MatriculaTecnica_Clave":
                    totalRegistros = db.Tecnicas.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.TecnicaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID != "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Tecnicas.Select(a => new { a.TecnicaID, Descripcion = a.MatriculaTexto, a.TecnicaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID != "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.TecnicaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && a.Pieza.Status && a.Pieza.Obra.TipoAdquisicion.AntID == "C" && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Tecnicas.Select(a => new { a.TecnicaID, Descripcion = a.MatriculaTexto, a.TecnicaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.Obra.TipoAdquisicion.AntID == "C" && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.TecnicaPiezas.Where(a => a.Pieza.TipoPieza.EsMaestra && !a.Pieza.Status && a.Pieza.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Tecnicas.Select(a => new { a.TecnicaID, Descripcion = a.MatriculaTexto, a.TecnicaPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Descripcion))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Descripcion,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.TecnicaPiezas.Any(c => c.Tecnica.AntID == "0"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TecnicaPiezas.Any(b => b.Tecnica.AntID == "0") && a.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;


                // -----------------------------------------------------------------------------------------------

                case "Ubicacion_Clave (OBRA)":
                    totalRegistros = db.Ubicaciones.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Ubicaciones.Select(a => new { a.UbicacionID, a.Nombre, a.Piezas.Where(b => b.TipoPieza.EsMaestra && b.Status && b.Obra.TipoAdquisicion.AntID != "C" && b.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Ubicaciones.Select(a => new { a.UbicacionID, a.Nombre, a.Piezas.Where(b => b.TipoPieza.EsMaestra && b.Status && b.Obra.TipoAdquisicion.AntID == "C" && b.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Ubicaciones.Select(a => new { a.UbicacionID, a.Nombre, a.Piezas.Where(b => b.TipoPieza.EsMaestra && !b.Status && b.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO                                                               //listaPiezas = listaTotalPiezas.Except(listaPiezasCompletas).ToList()
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.Ubicacion.AntID == "0" || a.Ubicacion.Nombre.Contains("pendiente"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && (a.Ubicacion.AntID == "0" || a.Ubicacion.Nombre.Contains("pendiente")) && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;


                // -----------------------------------------------------------------------------------------------

                case "Propietario_Clave":
                    totalRegistros = db.Propietarios.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Propietarios.Select(a => new { a.PropietarioID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre.ToString(),
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Propietarios.Select(a => new { a.PropietarioID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre.ToString(),
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Propietarios.Select(a => new { a.PropietarioID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && !c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID))).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre.ToString(),
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && a.Obra.Propietario.Nombre == 0).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Obra.Propietario.Nombre == 0 && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;

                // -----------------------------------------------------------------------------------------------

                case "TipoObjeto_Clave":
                    totalRegistros = db.TipoObras.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C").Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.TipoObras.Select(a => new { a.TipoObraID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status) && b.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C").Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.TipoObras.Select(a => new { a.TipoObraID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status) && b.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.TipoObras.Select(a => new { a.TipoObraID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && !c.Status)).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }
                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && (a.Obra.TipoObra.Nombre == "S/D")).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && (a.Obra.TipoObra.Nombre == "S/D")).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;



                // -----------------------------------------------------------------------------------------------

                case "TipoPieza_Clave":
                    totalRegistros = db.TipoPiezas.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.Status && a.Obra.TipoAdquisicion.AntID != "C").Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.TipoPiezas.Select(a => new { a.TipoPiezaID, NombreTP = a.Nombre, NombreTO = a.TipoObra.Nombre, a.Piezas.Where(b => b.Status && b.Obra.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.NombreTP))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.NombreTP + " (" + item.NombreTO + ")",
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.Status && a.Obra.TipoAdquisicion.AntID == "C").Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.TipoPiezas.Select(a => new { a.TipoPiezaID, NombreTP = a.Nombre, NombreTO = a.TipoObra.Nombre, a.Piezas.Where(b => b.Status && b.Obra.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.NombreTP))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.NombreTP + " (" + item.NombreTO + ")",
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => !a.Status).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.TipoPiezas.Select(a => new { a.TipoPiezaID, NombreTP = a.Nombre, NombreTO = a.TipoObra.Nombre, a.Piezas.Where(b => !b.Status).ToList().Count }).OrderBy(a => a.NombreTP))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.NombreTP + " (" + item.NombreTO + ")",
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO                                                               //listaPiezas = listaTotalPiezas.Except(listaPiezasCompletas).ToList()
                    totalPiezasSinElCampo = db.Piezas.Where(a => (a.TipoPieza.AntID == "0" || a.TipoPieza.Nombre.Contains("definir"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => (a.TipoPieza.AntID == "0" || a.TipoPieza.Nombre.Contains("definir"))).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;




                // -----------------------------------------------------------------------------------------------

                case "m_pieza_coleccion":
                    totalRegistros = db.Colecciones.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.Colecciones.Select(a => new { a.ColeccionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.Colecciones.Select(a => new { a.ColeccionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.Colecciones.Select(a => new { a.ColeccionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && !c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID))).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO 
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.Obra.Coleccion.AntID == "0" || a.Obra.Coleccion.Nombre.Contains("Sin dato"))).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.Obra.Coleccion.AntID == "0" || a.Obra.Coleccion.Nombre.Contains("Sin dato"))).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;



                // -----------------------------------------------------------------------------------------------

                case "catTipoAdquisicion":
                    totalRegistros = db.TipoAdquisiciones.Count();
                    //ACTIVAS Y NO COMODATO
                    estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestra > 0)
                    {
                        foreach (var item in db.TipoAdquisiciones.Select(a => new { a.TipoAdquisicionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                lista.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                });
                                subTotal = subTotal + item.Count;
                            }
                        }
                    }
                    //ACTIVAS Y COMODATO
                    estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraComodato > 0)
                    {
                        foreach (var item in db.TipoAdquisiciones.Select(a => new { a.TipoAdquisicionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)) && b.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaComodato.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                });
                                subTotalComodato = subTotalComodato + item.Count;
                            }
                        }
                    }
                    //INACTIVAS
                    estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                    if (estaEnPiezaMaestraInactiva > 0)
                    {
                        foreach (var item in db.TipoAdquisiciones.Select(a => new { a.TipoAdquisicionID, a.Nombre, a.Obras.Where(b => b.Piezas.Any(c => c.TipoPieza.EsMaestra && !c.Status && c.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID))).ToList().Count }).OrderBy(a => a.Nombre))
                        {
                            if (item.Count > 0)
                            {
                                listaInactiva.Add(new ItemReporteBasico()
                                {
                                    Nombre = item.Nombre,
                                    Cantidad = item.Count,
                                    Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                });
                                subTotalInactiva = subTotalInactiva + item.Count;
                            }
                        }
                    }

                    //OBRAS SIN CAMPO "" ESPECIFICADO
                    totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && a.Obra.TipoAdquisicionID == null).Count();
                    if (totalPiezasSinElCampo > 0)
                    {
                        foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && (a.Obra.TipoAdquisicionID == null) && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                        {
                            listaObraSinCampo.Add(new ObraReporteBasico()
                            {
                                ObraID = item.ObraID,
                                Clave = item.Clave,
                                Titulo = item.Valor
                            });
                            subTotalObraSinCampo++;
                        }
                    }
                    break;

                default:

                    if (tipoAtt.NombreID == "Generico" && tipoAtt.EsLista)
                    {
                        totalRegistros = db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID).Count();
                        //ACTIVAS Y NO COMODATO
                        estaEnPiezaMaestra = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID != "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                        if (estaEnPiezaMaestra > 0)
                        {
                            foreach (var item in db.ListaValores.Where(a=>a.TipoAtributoID == tipoAtt.TipoAtributoID).Select(a => new { a.ListaValorID, a.Valor, a.AtributoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID) && b.Pieza.Obra.TipoAdquisicion.AntID != "C").ToList().Count }).OrderBy(a => a.Valor))
                            {
                                if (item.Count > 0)
                                {
                                    lista.Add(new ItemReporteBasico()
                                    {
                                        Nombre = item.Valor,
                                        Cantidad = item.Count,
                                        Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestra, 3)
                                    });
                                    subTotal = subTotal + item.Count;
                                }
                            }
                        }
                        //ACTIVAS Y COMODATO
                        estaEnPiezaMaestraComodato = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.Status && a.Obra.TipoAdquisicion.AntID == "C" && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                        if (estaEnPiezaMaestraComodato > 0)
                        {
                            foreach (var item in db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID).Select(a => new { a.ListaValorID, a.Valor, a.AtributoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID) && b.Pieza.Obra.TipoAdquisicion.AntID == "C").ToList().Count }).OrderBy(a => a.Valor))
                            {
                                if (item.Count > 0)
                                {
                                    listaComodato.Add(new ItemReporteBasico()
                                    {
                                        Nombre = item.Valor,
                                        Cantidad = item.Count,
                                        Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraComodato, 3)
                                    });
                                    subTotalComodato = subTotalComodato + item.Count;
                                }
                            }
                        }
                        //INACTIVAS
                        estaEnPiezaMaestraInactiva = db.Piezas.Where(a => a.TipoPieza.EsMaestra && !a.Status && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
                        if (estaEnPiezaMaestraInactiva > 0)
                        {
                            foreach (var item in db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID).Select(a => new { a.ListaValorID, a.Valor, a.AtributoPiezas.Where(b => b.Pieza.TipoPieza.EsMaestra && !b.Pieza.Status && b.Pieza.TipoPieza.Atributos.Any(d => d.TipoAtributoID == tipoAtt.TipoAtributoID)).ToList().Count }).OrderBy(a => a.Valor))
                            {
                                if (item.Count > 0)
                                {
                                    listaInactiva.Add(new ItemReporteBasico()
                                    {
                                        Nombre = item.Valor,
                                        Cantidad = item.Count,
                                        Porcentaje = Math.Round((double)(item.Count * 100) / estaEnPiezaMaestraInactiva, 3)
                                    });
                                    subTotalInactiva = subTotalInactiva + item.Count;
                                }
                            }
                        }

                        //OBRAS SIN CAMPO "" ESPECIFICADO 
                        totalPiezasSinElCampo = db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.AtributoPiezas.Any(b => b.ListaValorID == null || b.ListaValor.AntID == "0" || b.ListaValor.Valor.Contains("definir")))).Count();
                        if (totalPiezasSinElCampo > 0)
                        {
                            foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsMaestra && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && (a.AtributoPiezas.Any(c => c.ListaValorID == null || c.ListaValor.AntID == "0" || c.ListaValor.Valor.Contains("definir")))).Select(a => new { a.Obra.ObraID, a.Obra.Clave, a.AtributoPiezas.FirstOrDefault(c => c.Atributo.TipoAtributo.AntNombre == "titulo").Valor }).OrderBy(a => a.ObraID))
                            {
                                listaObraSinCampo.Add(new ObraReporteBasico()
                                {
                                    ObraID = item.ObraID,
                                    Clave = item.Clave,
                                    Titulo = item.Valor
                                });
                                subTotalObraSinCampo++;
                            }
                        }
                        break;

                    }
                    else
                    {
                        ViewBag.Soportado = "No";
                    }

                    break;
            }







            /* vista */
            //Titulo
            ViewBag.Titulo = tipoAtt.Nombre;
            //Cuantas obras existen en total
            ViewBag.TotalObras = totalObras;
            //Cuantas obras deben tener autor
            ViewBag.ObrasMaestrasObligatorias = totalPiezasMaestras;

            //Cuantas obras estan con autor
            ViewBag.ObrasMaestras = estaEnPiezaMaestra;
            ViewBag.ObrasMaestrasComodato = estaEnPiezaMaestraComodato;
            ViewBag.ObrasMaestrasInactiva = estaEnPiezaMaestraInactiva;

            //SubTotal
            ViewBag.ObrasMaestrasSubTotal = subTotal;
            ViewBag.ObrasMaestrasSubTotalComodato = subTotalComodato;
            ViewBag.ObrasMaestrasSubTotalInactiva = subTotalInactiva;
            ViewBag.ObraMaestraSubTotalObraSinCampo = subTotalObraSinCampo;

            //Total
            ViewBag.Total = subTotal + subTotalComodato + subTotalInactiva;

            //Listas
            ViewBag.listaMaestra = lista;
            ViewBag.listaMaestraComodato = listaComodato;
            ViewBag.listaMaestraInactiva = listaInactiva;
            ViewBag.listaObraSinCampo = listaObraSinCampo.Take(500).ToList();




            return PartialView("_ReporteBasico");
        }

    }
}