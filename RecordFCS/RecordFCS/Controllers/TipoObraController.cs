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
    public class TipoObraController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoObra
        [CustomAuthorize(permiso = "TipoObraVer,CatConfig")]
        public ActionResult Index()
        {
            //var tipoAtributos = db.TipoObras.OrderBy(to => to.Nombre);
            //ViewBag.totalRegistros = tipoAtributos.Count();

            return View();
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<TipoObra> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.TipoObras.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.TipoObras.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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


        // GET: TipoObra/Lista
        [CustomAuthorize(permiso = "TipoObraVer,CatConfig")]
        public ActionResult Lista()
        {
            var tipoObras = db.TipoObras.OrderBy(to => to.Nombre);

            ViewBag.totalRegistros = tipoObras.Count();

            return PartialView("_Lista", tipoObras.ToList());
        }


        // GET: TipoObra/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<TipoObra> listaTipoObras;

            if (busqueda == "")
            {
                listaTipoObras = db.TipoObras.Where(c => c.Status == true).OrderBy(c => c.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaTipoObras = db.TipoObras.Where(c => c.Status == true && c.Nombre.ToLower().Contains(busqueda)).OrderBy(c => c.Nombre);
            }

            var totalValores = listaTipoObras.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaTipoObras.FirstOrDefault().TipoObraID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.TipoObraID = new SelectList(listaTipoObras, "TipoObraID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }

        // GET: TipoObra/Detalles/5
        [CustomAuthorize(permiso = "TipoPiezaVer")]
        public ActionResult Detalles(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }


            ViewBag.tipoObra = tipoObra;

            return View(tipoObra);
        }




        // GET: TipoObra/Crear
        [CustomAuthorize(permiso = "TipoObraCrear")]
        public ActionResult Crear()
        {
            TipoObra tipoObra = new TipoObra();

            return PartialView("_Crear", tipoObra);
        }

        // POST: TipoObra/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoObraCrear")]
        public ActionResult Crear([Bind(Include = "TipoObraID,Nombre,Status")] TipoObra tipoObra)
        {
            if (ModelState.IsValid)
            {
                //volver a revalidar el nombre
                if (db.TipoObras.Where(a => a.Nombre == tipoObra.Nombre).Count() > 0)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un registro con este nombre. Intenta con otro.");
                    return PartialView("_Crear", tipoObra);

                }

                tipoObra.Status = true;
                db.TipoObras.Add(tipoObra);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Obra: <b>{0}</b> se creo con exitó.", tipoObra.Nombre), true);

                // Regresar el ID del nuevo registro
                // DEspues de guardar los cambios se guarda el ID
                // Crear la pieza maestra con un atributo titulo

                var toID = tipoObra.TipoObraID;

                // crear pieza MAESTRA
                TipoPieza tipoPieza = new TipoPieza
                {
                    TipoObraID = toID,
                    Nombre = "Maestra",
                    Clave = "A",
                    Orden = 1,
                    Status = true,
                    EsMaestra = true
                };

                db.TipoPiezas.Add(tipoPieza);
                db.SaveChanges();


                ////AGREGAR LOS ATRIBUTOS BASICOS POR DEFAULT
                //var tipoAttLista = new List<TipoAtributo>()
                //{
                //    new TipoAtributo{ Nombre = "1", NombreHTML = "_Catalogo", Status = true },
                //    new TipoAtributo{ Nombre = "2", NombreHTML = "_Tecnica", Status = false },
                //    new TipoAtributo{ Nombre = "3", NombreHTML = "_Autor", Status = true },
                //    new TipoAtributo{ Nombre = "4", NombreHTML = "EscuelaArtisticaID", Status = true },
                //    new TipoAtributo{ Nombre = "5", NombreHTML = "Titulo", Status = true },
                //    new TipoAtributo{ Nombre = "6", NombreHTML = "TituloOriginal", Status = true },
                //    new TipoAtributo{ Nombre = "2", NombreHTML = "_Tecnica", Status = false },
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


                AlertaSuccess(string.Format("Pieza <b>{0}</b> agregada a {1}.", tipoPieza.Nombre, tipoObra.Nombre), true);

                string url = Url.Action("Lista", "TipoObra");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoObra);
        }

        // GET: TipoObra/Editar/5
        [CustomAuthorize(permiso = "TipoObraEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoObra);
        }

        // POST: TipoObra/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoObraEdit")]
        public ActionResult Editar([Bind(Include = "TipoObraID,Nombre,Status")] TipoObra tipoObra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoObra).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Obra: <b>{0}</b> se edito con exitó.", tipoObra.Nombre), true);
                string url = Url.Action("Lista", "TipoObra");
                return Json(new { success = true, url = url });

            }
            return PartialView("_Editar", tipoObra);
        }

        // GET: TipoObra/Eliminar/5
        [CustomAuthorize(permiso = "TipoObraEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoObra);
        }

        // POST: TipoObra/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "TipoObraEliminar")]
        public ActionResult EliminarConfirmado(int id)
        {
            string btnValue = Request.Form["accionx"];

            TipoObra tipoObra = db.TipoObras.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoObra.Status = false;
                    db.Entry(tipoObra).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoObra.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoObras.Remove(tipoObra);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoObra.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoObra");
            return Json(new { success = true, url = url });
        }




        [HttpPost]
        public JsonResult validarRegistroUnico(string Nombre)
        {
            var lista = db.TipoObras.Where(a => a.Nombre == Nombre).ToList();

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
