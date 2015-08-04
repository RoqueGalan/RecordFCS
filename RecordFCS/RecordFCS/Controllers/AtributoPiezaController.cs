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
    public class AtributoPiezaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: AtributoPieza/Detalles/5
        [CustomAuthorize]
        public ActionResult Detalles(Int64? idPieza, Int64? idAtributo)
        {
            if (idPieza == null || idAtributo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(idPieza, idAtributo);
            var valor = "";
            if (atributoPieza != null)
            {
                if (atributoPieza.Atributo.TipoAtributo.EsLista)
                {
                    if (atributoPieza.ListaValorID != null)
                    {
                        valor = atributoPieza.ListaValor.Valor;
                    }
                }
                else
                {
                    valor = atributoPieza.Valor;
                }
            }








            ViewBag.valor = valor;

            return PartialView("_Detalles");
        }


        // GET: AtributoPieza/Editar/5
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar(Int64? idPieza, Int64? idAtributo)
        {
            if (idPieza == null || idAtributo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(idPieza, idAtributo);

            if (atributoPieza == null)
            {
                //crear el atributoPieza
                atributoPieza = new AtributoPieza()
                {
                    AtributoID = Convert.ToInt64(idAtributo),
                    PiezaID = Convert.ToInt64(idPieza)
                };

                db.AtributoPiezas.Add(atributoPieza);
                db.SaveChanges();
            }

            var att = db.Atributos.Find(idAtributo);

            ViewBag.esLista = att.TipoAtributo.EsLista;

            if (att.TipoAtributo.EsLista)
            {
                ViewBag.ListaValorID = new SelectList(att.TipoAtributo.ListaValores.ToList(), "ListaValorID", "Valor", atributoPieza.ListaValorID);
            }

            return PartialView("_Editar", atributoPieza);
        }


        // POST: AtributoPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AttPiezaEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,AtributoID,Valor,ListaValorID")] AtributoPieza atributoPieza)
        {
            var atributo = db.Atributos.Find(atributoPieza.AtributoID);

            //validar que sea LISTA
            if (atributo.TipoAtributo.EsLista)
            {
                // valor de ListaValorID no sea NULL o 0
                if (atributoPieza.ListaValorID == null || atributoPieza.ListaValorID == 0)
                {
                    //validar que BuscarDato no sea "" o NULL
                    var text_BuscarDato = Request.Form["BuscarDato"].ToString();

                    if (!String.IsNullOrEmpty(text_BuscarDato))
                    {

                        var lv_existe = db.ListaValores.Where(lv => (lv.TipoAtributoID == atributo.TipoAtributoID && lv.Valor == text_BuscarDato)).ToList();

                        // si es repetido agregar su id al ListaValorID de AtributoPieza
                        if (lv_existe.Count > 0)
                        {
                            //ya existe
                            atributoPieza.ListaValorID = lv_existe.FirstOrDefault().ListaValorID;
                        }
                        else
                        {
                            //no existe
                            var listValorNew = new ListaValor()
                            {
                                Valor = text_BuscarDato,
                                TipoAtributoID = atributo.TipoAtributoID,
                                Status = true,
                            };

                            db.ListaValores.Add(listValorNew);
                            db.SaveChanges();

                            atributoPieza.ListaValorID = listValorNew.ListaValorID;
                        }

                        db.Entry(atributoPieza).State = EntityState.Modified;
                        db.SaveChanges();
                        string url = Url.Action("Detalles", "AtributoPieza", new { idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });

                        return Json(new { success = true, url = url, idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });

                    }
                }
                else
                {
                    db.Entry(atributoPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    string url = Url.Action("Detalles", "AtributoPieza", new { idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });
                    return Json(new { success = true, url = url, idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });
                }
            }
            else
            {
                //logica para un campo sencillo
                db.Entry(atributoPieza).State = EntityState.Modified;
                db.SaveChanges();
                string url = Url.Action("Detalles", "AtributoPieza", new { idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });
                return Json(new { success = true, url = url, idPieza = atributoPieza.PiezaID, idAtributo = atributoPieza.AtributoID });

            }

            ViewBag.esLista = atributoPieza.Atributo.TipoAtributo.EsLista;

            if (ViewBag.esLista)
            {
                ViewBag.ListaValorID = new SelectList(db.ListaValores, "ListaValorID", "Valor", atributoPieza.ListaValorID);
            }

            return PartialView("_Editar", atributoPieza);
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
