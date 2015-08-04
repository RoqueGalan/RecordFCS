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
    public class AtributoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Lista/5
        [CustomAuthorize]
        public ActionResult Index()
        {
            //redireccionar a Tipo de Obras
            return View("Lista", "Lista", "TipoPieza");
        }


        // GET: Atributo
        [CustomAuthorize(permiso = "AtributoVer")]
        public ActionResult Lista(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPieza tipoPieza = db.TipoPiezas.Find(id);
            if (tipoPieza == null)
            {
                return HttpNotFound();
            }

            tipoPieza.Atributos = tipoPieza.Atributos.OrderBy(a => a.Orden).ToList();

            ViewBag.totalRegistros = tipoPieza.Atributos.Count;
            ViewBag.TipoPiezaID = id;


            return PartialView("_Lista", tipoPieza.Atributos.ToList());
        }



        //// GET: Atributo/Detalles/5
        //public ActionResult Detalles(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Atributo atributo = db.Atributos.Find(id);
        //    if (atributo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(atributo);
        //}


        // GET: Atributo/Crear
        [CustomAuthorize(permiso = "AtributoCrear")]
        public ActionResult Crear(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPieza tipoPieza = db.TipoPiezas.Single(tp => tp.TipoPiezaID == id);
            if (tipoPieza == null)
            {
                return HttpNotFound();
            }

            Atributo atributo = new Atributo()
            {
                TipoPieza = tipoPieza,
                TipoPiezaID = tipoPieza.TipoPiezaID,
                Orden = tipoPieza.Atributos.Count + 1,
                Status = true,
                EnFichaBasica = true
            };


            // llenar el select personalizado
            // solo muestra los tipos de atributos no asignados al tipo de pieza
            var listaCompleta = db.TipoAtributos.Where(a => a.Status).OrderBy(a => a.BuscadorOrden).ToList().Except(tipoPieza.Atributos.Select(a => a.TipoAtributo));

            var listaTipoAtributos = new List<object>();

            foreach (var ta in listaCompleta)
            {
                var esLista = ta.EsLista ? "[+]" : "";
                var datoHTML = " -- [" + ta.DatoHTML + "]";
                var descripcion = ta.Descripcion == null ? "" : "[" + ta.Descripcion + "]";
                listaTipoAtributos.Add(new
                {
                    TipoAtributoID = ta.TipoAtributoID,
                    Nombre = ta.Nombre + esLista + datoHTML + descripcion
                });
            }



            ViewBag.TipoAtributoID = new SelectList(listaTipoAtributos, "TipoAtributoID", "Nombre");

            return PartialView("_Crear", atributo);
        }


        // POST: Atributo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AtributoCrear")]
        public ActionResult Crear([Bind(Include = "AtributoID,TipoPiezaID,TipoAtributoID,NombreAlterno,Orden,Status,EnFichaBasica,Requerido")] Atributo atributo, Int64? id)
        {
            if (ModelState.IsValid)
            {
                var tipoAtt = db.TipoAtributos.SingleOrDefault(a=> a.TipoAtributoID == atributo.TipoAtributoID);

                if (tipoAtt != null)
                {
                    atributo.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                }

                db.Atributos.Add(atributo);
                db.SaveChanges();

                //al crear un nuevo atributo hay que agregarlo en todas las obras ya registradas
                atributo.TipoAtributo = db.TipoAtributos.Find(atributo.TipoAtributoID);

                var piezas = db.Piezas.Where(p => p.TipoPiezaID == atributo.TipoPiezaID).ToList();
                
                int i = 0;
                db.Dispose();
                db = new RecordFCSContext();
                db.Configuration.AutoDetectChangesEnabled = false;

                foreach (var pieza in piezas)
                {
                    AtributoPieza atributoPieza = new AtributoPieza
                    {
                        AtributoID = atributo.AtributoID,
                        PiezaID = pieza.PiezaID
                    };
                    //error tratar si no existe ningun registro
                    ////if (atributo.TipoAtributo.EsLista)
                    ////{
                    ////    atributoPieza.ListaValorID = db.ListaValores.Single(lv => (lv.TipoAtributoID == atributo.TipoAtributo.TipoAtributoID && lv.Valor == "- Sin definir -")).ListaValorID;
                    ////}

                    db.AtributoPiezas.Add(atributoPieza);
                    i++;

                    if (i == 500 )
                    {
                        db.SaveChanges();
                        db.Dispose();
                        db = new RecordFCSContext();
                        db.Configuration.AutoDetectChangesEnabled = false;
                        i = 0;
                    }
                }
                db.SaveChanges();

                AlertaSuccess(string.Format("Atributo: <b>{0}</b> se agrego con exitó.", atributo.TipoAtributo.Nombre), true);
                string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });

                return Json(new { success = true, url = url, modelo = "Atributo" });
            }

            // llenar el select personalizado
            var query = db.TipoAtributos.Where(ta => ta.Status == true);
            var listaTipoAtributos = new List<object>();
            foreach (var ta in query)
            {
                var esLista = ta.EsLista ? "[+]" : "";
                var datoHTML = " -- [" + ta.DatoHTML + "]";
                var descripcion = ta.Descripcion == null ? "" : "[" + ta.Descripcion + "]";
                listaTipoAtributos.Add(new
                {
                    TipoAtributoID = ta.TipoAtributoID,
                    Nombre = ta.Nombre + esLista + datoHTML + descripcion
                });
            }

            ViewBag.TipoAtributoID = new SelectList(listaTipoAtributos, "TipoAtributoID", "Nombre", atributo.TipoAtributoID);

            return PartialView("_Crear", atributo);
        }


        // GET: Atributo/Editar/5
        [CustomAuthorize(permiso = "AtributoEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atributo atributo = db.Atributos.Find(id);
            if (atributo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", atributo);
        }


        // POST: Atributo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AtributoEdit")]
        public ActionResult Editar([Bind(Include = "AtributoID,TipoPiezaID,TipoAtributoID,NombreAlterno,Orden,Status,EnFichaBasica,Requerido")] Atributo atributo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atributo).State = EntityState.Modified;
                db.SaveChanges();

                atributo.TipoAtributo = db.TipoAtributos.Find(atributo.TipoAtributoID);

                AlertaInfo(string.Format("Atributo: <b>{0}</b> se edito con exitó.", atributo.TipoAtributo.Nombre), true);
                string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });

                return Json(new { success = true, url = url });

            }




            return PartialView("_Editar", atributo);
        }

        // GET: Atributo/Eliminar/5
        [CustomAuthorize(permiso = "AtributoEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atributo atributo = db.Atributos.Find(id);
            if (atributo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", atributo);
        }


        // POST: Atributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "AtributoEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Atributo atributo = db.Atributos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    atributo.Status = false;
                    db.Entry(atributo).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", atributo.TipoAtributo.Nombre), true);

                    break;
                case "eliminar":
                    var tipoAtt = db.TipoAtributos.Find(atributo.TipoAtributoID);
                    db.Atributos.Remove(atributo);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoAtt.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });
            return Json(new { success = true, url = url });
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
