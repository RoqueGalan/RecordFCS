using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;
using System.Threading.Tasks;
using RecordFCS.Helpers;
using RecordFCS.Helpers.Seguridad;

namespace RecordFCS.Controllers
{
    public class UbicacionController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Ubicacion
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Index()
        {
            //ViewBag.PersonID = id;
            //var addresses = db.Addresses.Where(a => a.PersonID == id).OrderBy(a => a.City);

            return View();
        }

        [CustomAuthorize(permiso = "UbiCrear")]
        public ActionResult AgregarValor(string valor, bool aceptar)
        {
            if (aceptar)
            {
                if (valor != null || valor != "")
                {
                    var lista = db.Ubicaciones.Where(a => a.Nombre == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var ubicacion = new Ubicacion()
                        {
                            Nombre = valor,
                            Status = true,
                        };
                        db.Ubicaciones.Add(ubicacion);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Ubicación: <b>{0}</b> se agregó con exitó.", ubicacion.Nombre), true);
                        return Json(new { success = true, valor = ubicacion.UbicacionID, texto = ubicacion.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Ubicación: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().UbicacionID, texto = lista.FirstOrDefault().Nombre });
                    }
                }
            }
            return Json(new { success = false });
        }


        [CustomAuthorize]
        public ActionResult CampoLista(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }

            var valores = db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "UbicacionID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }


        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Ubicacion> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Ubicaciones.Where(a => a.Nombre.StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Ubicaciones.Where(a => a.Nombre.ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
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


        // GET: Ubicacion/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista()
        {
            //ViewBag.PersonID = id;
            //var addresses = db.Addresses.Where(a => a.PersonID == id);

            var ubicaciones = db.Ubicaciones.OrderBy(u => u.Nombre);

            ViewBag.totalRegistros = ubicaciones.Count();

            return PartialView("_Lista", ubicaciones.ToList());
        }


        // GET: Ubicacion/FormLista/busqueda?seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Ubicacion> listaUbicaciones;

            if (busqueda == "")
            {
                listaUbicaciones = db.Ubicaciones.Where(u => u.Status == true).OrderBy(u => u.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaUbicaciones = db.Ubicaciones.Where(u => u.Status == true && u.Nombre.ToLower().Contains(busqueda)).OrderBy(u => u.Nombre);
            }

            var totalValores = listaUbicaciones.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaUbicaciones.FirstOrDefault().UbicacionID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.UbicacionID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }



        // GET: Ubicacion/Crear
        [CustomAuthorize(permiso = "UbiCrear")]
        public ActionResult Crear()
        {
            Ubicacion ubicacion = new Ubicacion();

            return PartialView("_Crear", ubicacion);
        }

        // POST: Ubicacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UbiCrear")]
        public ActionResult Crear([Bind(Include = "UbicacionID,Nombre")] Ubicacion ubicacion)
        {
            if (ModelState.IsValid)
            {
                ubicacion.Status = true;
                db.Ubicaciones.Add(ubicacion);
                db.SaveChanges();

                AlertaSuccess(string.Format("Ubicación: <b>{0}</b> se creo con exitó.", ubicacion.Nombre), true);
                string url = Url.Action("Lista", "Ubicacion");
                return Json(new { success = true, url = url, modelo = "Ubicacion" });
            }

            return PartialView("_Crear", ubicacion);
        }


        // GET: Ubicacion/Editar/5
        [CustomAuthorize(permiso = "UbiEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubicacion ubicacion = db.Ubicaciones.Find(id);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", ubicacion);
        }

        // POST: Ubicacion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UbiEdit")]
        public ActionResult Editar([Bind(Include = "UbicacionID,Nombre,Status")] Ubicacion ubicacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ubicacion).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Ubicación: <b>{0}</b> se edito con exitó.", ubicacion.Nombre), true);
                string url = Url.Action("Lista", "Ubicacion");
                return Json(new { success = true, url = url });
            }
            return PartialView("_Editar", ubicacion);
        }

        // GET: Ubicacion/Eliminar/5
        [CustomAuthorize(permiso = "UbiEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubicacion ubicacion = db.Ubicaciones.Find(id);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", ubicacion);
        }



        // POST: Atributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UbiEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Ubicacion ubicacion = db.Ubicaciones.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    ubicacion.Status = false;
                    db.Entry(ubicacion).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se deshabilito <b>{0}</b>", ubicacion.Nombre), true);

                    break;
                case "eliminar":
                    db.Ubicaciones.Remove(ubicacion);
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se elimino <b>{0}</b>", ubicacion.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }
            string url = Url.Action("Lista", "Ubicacion");
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
