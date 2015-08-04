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
    public class TipoPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Lista/5
        [CustomAuthorize]
        public ActionResult Index()
        {
            //redireccionar a Tipo de Obras
            return RedirectToAction("Lista", "TipoObra");
        }


        [CustomAuthorize]
        public ActionResult RenderTipoPiezaMaestra(Int64? id)
        {
            //seleccionar las piezas maestras que tenga el tipo de obra
            if (id == null)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoObra tipoObra = db.TipoObras.Find(id);

            if (tipoObra == null)
            {
                HttpNotFound();
            }

            var listaPiezasMaestra = db.TipoPiezas.Where(tp => tp.TipoObraID == id && tp.EsMaestra).OrderBy(tp => tp.Nombre);

            ViewBag.totalRegistros = listaPiezasMaestra.Count();
            ViewBag.TipoPiezaID = new SelectList(listaPiezasMaestra.ToList(), "TipoPiezaID", "Nombre");


            return PartialView("_RenderCampoMaestra");
        }

        [CustomAuthorize]
        public ActionResult AtributosRequeridos(Int64? id, Int64? obraID)
        {
            if (id == null)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoPieza tipoPieza = db.TipoPiezas.Find(id);

            if (tipoPieza == null)
            {
                HttpNotFound();
            }

            tipoPieza.Atributos = tipoPieza.Atributos.Where(a => a.Requerido).OrderBy(a => a.Orden).ToList();

            ViewBag.totalRegistros = tipoPieza.Atributos.Count;

            if (tipoPieza.EsMaestra)
            {
                return PartialView("_AtributosRequeridosRegistro", tipoPieza);
            }
            else
            {
                Obra obra = db.Obras.Find(obraID);
                var clave = "";

                var listaClaves = obra.Piezas.Where(p => (p.TipoPiezaID == tipoPieza.TipoPiezaID)).ToList();
                ////GENERAR EN AUTOMATICO LA CLAVE
                if (listaClaves.Count == 0)
                {
                    clave = obra.Clave + "-" + tipoPieza.Clave;
                }
                else
                {
                    //extraer los numeros de la clave
                    var listaC = new List<Int32>();

                    foreach (var item in listaClaves)
                    {
                        char x = Convert.ToChar(tipoPieza.Clave);
                        char s = '-';
                        clave = item.Clave.Split(s).Last();

                        clave = clave.Split(x).Last();

                        if (clave != "")
                        {
                            listaC.Add(Convert.ToInt32(clave));
                        }
                    }

                    if (listaC.Count == 0)
                    {
                        clave = obra.Clave + "-" + tipoPieza.Clave + "2";
                    }
                    else
                    {
                        listaC.Sort();
                        var mayor = listaC.LastOrDefault();
                        mayor = mayor + 1;

                        clave = obra.Clave + "-" + tipoPieza.Clave + mayor;
                    }

                }

                ViewBag.folio = clave;
                return PartialView("_AtributosRequeridosOtraPieza", tipoPieza);
            }
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<TipoPieza> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.TipoPiezas.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.TipoPiezas.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }

            }

            List<string> lista = new List<string>();

            if (listaTabla != null)
            {
                foreach (var item in listaTabla)
                {
                    lista.Add(item.Nombre);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        // GET: Lista/TipoObraID
        [CustomAuthorize(permiso = "TipoPiezaVer")]
        public ActionResult Lista(Int64? id)
        {
            if (id == null)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoObra tipoObra = db.TipoObras.Find(id);

            if (tipoObra == null)
            {
                HttpNotFound();
            }

            var tipoPiezas = db.TipoPiezas.Where(tp => tp.TipoObraID == id).OrderBy(tp => tp.Orden);

            ViewBag.totalRegistros = tipoPiezas.Count();
            ViewBag.TipoObraID = id;

            return PartialView("_Lista", tipoPiezas.ToList());

        }


        // GET: TipoPieza/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<TipoPieza> listaTipoPiezas;

            if (busqueda == "")
            {
                listaTipoPiezas = db.TipoPiezas.Where(tp => tp.Status == true).OrderBy(tp => tp.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaTipoPiezas = db.TipoPiezas.Where(tp => tp.Status == true && tp.Nombre.ToLower().Contains(busqueda)).OrderBy(tp => tp.Nombre);
            }

            var totalValores = listaTipoPiezas.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaTipoPiezas.FirstOrDefault().TipoPiezaID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.TipoPiezaID = new SelectList(listaTipoPiezas, "TipoPiezaID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: TipoPieza/Detalles/5
        [CustomAuthorize(permiso = "AtributoVer")]
        public ActionResult Detalles(Int64? id)
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

            ViewBag.tipoPieza = tipoPieza;

            return View(tipoPieza);
        }

        // GET: TipoPieza/Crear
        [CustomAuthorize(permiso = "TipoPiezaCrear")]
        public ActionResult Crear(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Single(to => to.TipoObraID == id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }

            TipoPieza tipoPieza = new TipoPieza()
            {
                TipoObra = tipoObra,
                TipoObraID = tipoObra.TipoObraID,
                Orden = tipoObra.TipoPiezas.Count + 1,
                Status = true
            };

            //Crear logica de CLAVE


            return PartialView("_Crear", tipoPieza);

        }


        // POST: TipoPieza/Crear
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoPiezaCrear")]
        public ActionResult Crear([Bind(Include = "TipoPiezaID,Nombre,Clave,Orden,Status,TipoObraID,EsMaestra")] TipoPieza tipoPieza)
        {
            //hay que volver a revalidar el nombre y la clave



            if (ModelState.IsValid)
            {

                if (db.TipoPiezas.Where(a => a.TipoObraID == tipoPieza.TipoObraID && (a.Nombre == tipoPieza.Nombre || a.Clave == tipoPieza.Clave)).Count() > 0)
                {
                    ModelState.AddModelError("", "Error verifique los campos NOMBRE y CLAVE");
                    return PartialView("_Crear", tipoPieza);

                }

                db.TipoPiezas.Add(tipoPieza);
                db.SaveChanges();




                //AGREGAR LOS ATRIBUTOS BASICOS POR DEFAULT
                //var tipoAttLista = new List<TipoAtributo>()
                //{
                //    new TipoAtributo{ Nombre = "1", NombreHTML = "_Catalogo", Status = true },
                //    new TipoAtributo{ Nombre = "2", NombreHTML = "_Tecnica", Status = false },
                //    new TipoAtributo{ Nombre = "3", NombreHTML = "_Autor", Status = true },
                //    new TipoAtributo{ Nombre = "4", NombreHTML = "EscuelaArtisticaID", Status = true },
                //    new TipoAtributo{ Nombre = "5", NombreHTML = "Titulo", Status = true },
                //    new TipoAtributo{ Nombre = "6", NombreHTML = "TituloOriginal", Status = true },
                //    new TipoAtributo{ Nombre = "7", NombreHTML = "OtrosTitulos", Status = true },
                //    new TipoAtributo{ Nombre = "8", NombreHTML = "Descripcion", Status = true },
                //    new TipoAtributo{ Nombre = "9", NombreHTML = "Bibliografia", Status = true },
                //    new TipoAtributo{ Nombre = "10",NombreHTML = "Asesores", Status = true },
                //    new TipoAtributo{ Nombre = "11",NombreHTML = "FechaEjecucionID", Status = true },
                //    new TipoAtributo{ Nombre = "12",NombreHTML = "FiliacionEstilisticaID", Status = true },
                //    new TipoAtributo{ Nombre = "13",NombreHTML = "_Imagen", Status = false },
                //    new TipoAtributo{ Nombre = "14",NombreHTML = "Inscripcion", Status = true },
                //    new TipoAtributo{ Nombre = "15",NombreHTML = "Marcas", Status = true },
                //    new TipoAtributo{ Nombre = "16",NombreHTML = "Materiales", Status = true },
                //    new TipoAtributo{ Nombre = "17",NombreHTML = "EstadoConservacionID", Status = true },
                //    new TipoAtributo{ Nombre = "18",NombreHTML = "Observaciones", Status = true },
                //    new TipoAtributo{ Nombre = "19",NombreHTML = "ProcedenciaID", Status = true },
                //    new TipoAtributo{ Nombre = "20",NombreHTML = "FormaAdquisicionID", Status = true },
                //    new TipoAtributo{ Nombre = "21",NombreHTML = "CasaComercialID", Status = true },
                //    new TipoAtributo{ Nombre = "22",NombreHTML = "_Medida", Status = true }
                //};




                ////Buscar y agregar Tipo de Atributo a la pieza maestra
                //foreach (var tipoAtt in tipoAttLista)
                //{
                //    var tipoAtributo = db.TipoAtributos.Single(ta => ta.NombreHTML == tipoAtt.NombreHTML);

                //    var atributo = new Atributo()
                //    {
                //        TipoAtributoID = tipoAtributo.TipoAtributoID,
                //        Orden = Convert.ToInt32(tipoAtt.Nombre),
                //        Status = tipoAtt.Status,
                //        TipoPiezaID = tipoPieza.TipoPiezaID
                //    };

                //    db.Atributos.Add(atributo);
                //    db.SaveChanges();
                //}


                AlertaSuccess(string.Format("Tipo Pieza: <b>{0}</b> se creo con exitó.", tipoPieza.Nombre), true);
                string url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoObraID });

                return Json(new { success = true, url = url, modelo = "TipoPieza" });
            }

            return PartialView("_Crear", tipoPieza);
        }


        // GET: TipoPieza/Editar/5
        [CustomAuthorize(permiso = "TipoPiezaEdit")]
        public ActionResult Editar(Int64? id)
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
            return PartialView("_Editar", tipoPieza);
        }


        // POST: TipoPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoPiezaEdit")]
        public ActionResult Editar([Bind(Include = "TipoPiezaID,Nombre,Clave,Orden,Status,TipoObraID,EsMaestra")] TipoPieza tipoPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo Pieza: <b>{0}</b> se edito con exitó.", tipoPieza.Nombre), true);
                string url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoObraID });

                return Json(new { success = true, url = url });

            }
            return PartialView("_Editar", tipoPieza);
        }


        // GET: TipoPieza/Eliminar/5
        [CustomAuthorize(permiso = "TipoPiezaEliminar")]
        public ActionResult Eliminar(Int64? id)
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

            return PartialView("_Eliminar", tipoPieza);
        }


        // POST: TipoPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoPiezaEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            TipoPieza tipoPieza = db.TipoPiezas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoPieza.Status = false;
                    db.Entry(tipoPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoPieza.Nombre), true);

                    break;
                case "eliminar":
                    db.TipoPiezas.Remove(tipoPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoPieza.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }
            string url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoObraID });
            return Json(new { success = true, url = url });
        }


        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnicoNombre(string Nombre, Int64? TipoObraID)
        {
            var lista = db.TipoPiezas.Where(a =>
                (TipoObraID.HasValue) ?
                (a.TipoObraID == TipoObraID && a.Nombre == Nombre) :
                (a.Nombre == Nombre)).ToList();

            return Json(lista.Count == 0);
        }

        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnicoClave(string Clave, Int64? TipoObraID)
        {
            var lista = db.TipoPiezas.Where(a =>
                (TipoObraID.HasValue) ?
                (a.TipoObraID == TipoObraID && a.Clave == Clave) :
                (a.Clave == Clave)).ToList();

            return Json(lista.Count == 0);
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
