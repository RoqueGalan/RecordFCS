using RecordFCS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS.Controllers
{
    public class RestructurarTipoPiezaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: RestructurarTipoPieza
        public JsonResult Restructura_Pieza(string id)
        {

            return Json(new { success = true });
        }

        public ActionResult MenuRestructura(Int64? id)
        {
            bool error = false;
            string tituloMenu = "";
            string descripcion = "";
            int totalPiezas = 0;
            int totalPiezasCompletas = 0;
            int totalPiezasInCompletas = 0;
            double porCompleto = 0;
            double porIncompleto = 0;

            //Calcular el total de piezas que existen en TipoObra
            //y cuales estan faltan
            if (id != null)
            {
                var atributo = db.Atributos.FirstOrDefault(a => a.AtributoID == id);
                if (id != null)
                {
                    totalPiezas = db.Piezas.Where(a => a.TipoPiezaID == atributo.TipoPiezaID).Count();
                    totalPiezasCompletas = db.AtributoPiezas.Where(a => a.AtributoID == atributo.AtributoID).Count();

                    totalPiezasInCompletas = totalPiezas - totalPiezasCompletas;

                    tituloMenu = atributo.TipoPieza.Nombre;
                    descripcion = atributo.TipoAtributo.Nombre;

                    //total / completas
                    if (totalPiezas > 0)
                    {
                        porCompleto = (totalPiezasCompletas * 100) / totalPiezas;
                        porIncompleto = (totalPiezasInCompletas * 100) / totalPiezas;
                    }
                    else
                    {
                        porCompleto = 100;
                        porIncompleto = 0;
                    }

                }
                else
                {
                    error = true;
                    tituloMenu = "Error de Atributo";
                    descripcion = "No existe el Atributo";
                }
            }
            else
            {
                error = true;
                tituloMenu = "Error de Atributo";
                descripcion = "No existe el Atributo";
            }

            ViewBag.atributoID = id;
            ViewBag.totalPiezas = totalPiezas;
            ViewBag.totalPiezasCompletas = totalPiezasCompletas;
            ViewBag.totalPiezasInCompletas = totalPiezasInCompletas;
            ViewBag.error = error;
            ViewBag.tituloMenu = tituloMenu;
            ViewBag.descripcion = descripcion;
            ViewBag.porCompleto = porCompleto + "%";
            ViewBag.porIncompleto = porIncompleto + "%";



            return PartialView("_MenuRestructura");
        }

        //Prueba restructura AUTOR
        public ActionResult Restructura_AtributoPieza(Int64? id)
        {
            string error = "";
            string descripcion = "";


            if (id != null)
            {
                var atributo = db.Atributos.Where(a => a.AtributoID == id).Select(a => new { a.AtributoID, a.TipoAtributoID, a.TipoPiezaID }).FirstOrDefault();
                if (atributo != null)
                {

                    //listar todas las piezas que pertenescan al tipo
                    var listaTotalPiezas = db.Piezas.Where(a => a.TipoPiezaID == atributo.TipoPiezaID).Select(a => new { a.PiezaID, a.ObraID }).ToList();
                    var listaPiezasCompletas = db.AtributoPiezas.Where(a => a.AtributoID == atributo.AtributoID).Select(a => new { a.PiezaID, a.Pieza.ObraID }).ToList();

                    var listaPiezas = listaTotalPiezas.Except(listaPiezasCompletas).ToList();

                    int i = 0;
                    db.Dispose();
                    db = new RecordFCSContext();
                    db.Configuration.AutoDetectChangesEnabled = false;

                    foreach (var pieza in listaPiezas)
                    {

                        //crear el AtributoPieza
                        //exception de PRIMARY KEY DUPLICADA
                        try
                        {
                            i++;
                            db.AtributoPiezas.Add(new AtributoPieza()
                            {
                                AtributoID = atributo.AtributoID,
                                PiezaID = pieza.PiezaID
                            });


                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 2627)
                            {
                                //Violation of primary key. Handle Exception
                                descripcion = string.Format("llave duplicada en ObraID = {0}, PiezaID = {1} ", pieza.ObraID, pieza.PiezaID);
                            }
                        }




                        if (i >= 500)
                        {
                            db.SaveChanges();

                            db.Dispose();
                            db = new RecordFCSContext();
                            db.Configuration.AutoDetectChangesEnabled = false;
                            i = 0;
                        }

                    }


                }
                else
                    error = string.Format("Atributo no existe. TO = {0}", id);
            }
            else
                error = "Atributo es NULL";




            db.SaveChanges();



            bool errorx = false;
            string tituloMenu = "";
            string descripcionx = "";
            int totalPiezas = 0;
            int totalPiezasCompletas = 0;
            int totalPiezasInCompletas = 0;
            double porCompleto = 0;
            double porIncompleto = 0;

            //Calcular el total de piezas que existen en TipoObra
            //y cuales estan faltan
            if (id != null)
            {
                var atributo = db.Atributos.FirstOrDefault(a => a.AtributoID == id);
                if (id != null)
                {
                    totalPiezas = db.Piezas.Where(a => a.TipoPiezaID == atributo.TipoPiezaID).Count();
                    totalPiezasCompletas = db.AtributoPiezas.Where(a => a.AtributoID == atributo.AtributoID).Count();

                    totalPiezasInCompletas = totalPiezas - totalPiezasCompletas;

                    tituloMenu = atributo.TipoPieza.Nombre;
                    descripcion = atributo.TipoAtributo.Nombre;

                    //total / completas
                    porCompleto = (totalPiezasCompletas * 100) / totalPiezas;
                    porIncompleto = (totalPiezasInCompletas * 100) / totalPiezas;

                }
                else
                {
                    errorx = true;
                    tituloMenu = "Error de Atributo";
                    descripcion = "No existe el Atributo";
                }
            }
            else
            {
                errorx = true;
                tituloMenu = "Error de Atributo";
                descripcion = "No existe el Atributo";
            }

            ViewBag.atributoID = id;
            ViewBag.totalPiezas = totalPiezas;
            ViewBag.totalPiezasCompletas = totalPiezasCompletas;
            ViewBag.totalPiezasInCompletas = totalPiezasInCompletas;
            ViewBag.error = errorx;
            ViewBag.tituloMenu = tituloMenu;
            ViewBag.descripcion = descripcionx;
            ViewBag.porCompleto = porCompleto + "%";
            ViewBag.porIncompleto = porIncompleto + "%";

            return PartialView("_fin");
        }
    }
}