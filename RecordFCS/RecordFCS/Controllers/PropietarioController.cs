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
    public class PropietarioController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Propietario
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize(permiso = "PropiCrear")]
        public ActionResult AgregarValor(string valor, bool aceptar)
        {
            if (aceptar)
            {
                if (valor != null || valor != "")
                {
                    var lista = db.Propietarios.Where(a => a.Nombre.ToString() == valor).ToList();
                    if (lista.Count == 0)
                    {
                        var propietario = new Propietario()
                        {
                            Nombre = Convert.ToInt32(valor),
                            Status = true,
                        };
                        db.Propietarios.Add(propietario);
                        db.SaveChanges();
                        AlertaSuccess(string.Format("Propietario: <b>{0}</b> se agregó con exitó.", propietario.Nombre), true);
                        return Json(new { success = true, valor = propietario.PropietarioID, texto = propietario.Nombre });
                    }
                    else
                    {
                        AlertaSuccess(string.Format("Propietario: <b>{0}</b> se agregó con exitó.", lista.FirstOrDefault().Nombre), true);
                        return Json(new { success = true, valor = lista.FirstOrDefault().PropietarioID, texto = lista.FirstOrDefault().Nombre });
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

            var valores = db.Propietarios.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            string nombreLista = "req_list_" + id;

            ViewData[nombreLista] = new SelectList(valores, "PropietarioID", "Nombre");

            ViewBag.TipoAtributoID = tipoAtributo.TipoAtributoID;

            return PartialView("_CampoLista");
        }

        [CustomAuthorize]
        public ActionResult ListaString(string busqueda, bool exacta)
        {
            IQueryable<Propietario> listaTabla;

            if (String.IsNullOrWhiteSpace(busqueda))
            {
                listaTabla = null;
            }
            else
            {
                if (exacta)
                {
                    listaTabla = db.Propietarios.Where(a => a.Nombre.ToString().StartsWith(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }
                else
                {
                    busqueda = busqueda.ToLower();
                    listaTabla = db.Propietarios.Where(a => a.Nombre.ToString().ToLower().Contains(busqueda)).OrderBy(a => a.Nombre).Take(10);
                }

            }

            List<string> lista = new List<string>();

            if (listaTabla != null)
            {
                foreach (var item in listaTabla)
                {
                    lista.Add(item.Nombre.ToString());
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }


        // GET: Propietario/Lista
        [CustomAuthorize(permiso = "CatConfig")]
        public ActionResult Lista()
        {
            var propietarios = db.Propietarios;

            ViewBag.totalRegistros = propietarios.Count();

            return PartialView("_Lista", propietarios.ToList());
        }


        // GET: Propietario/FormLista/busqueda?Seleccion
        [CustomAuthorize]
        public ActionResult FormLista(string busqueda, Int64? seleccion)
        {
            IQueryable<Propietario> listaPropietarios;
            if (busqueda == "")
            {
                listaPropietarios = db.Propietarios.Where(p => p.Status == true).OrderBy(p => p.Nombre);
            }
            else
            {
                busqueda = busqueda.ToLower();
                listaPropietarios = db.Propietarios.Where(p => p.Status == true && p.Nombre.ToString().Contains(busqueda)).OrderBy(p => p.Nombre);
            }

            var totalValores = listaPropietarios.Count();

            if (totalValores > 0)
            {
                if (seleccion == null)
                {
                    seleccion = listaPropietarios.FirstOrDefault().PropietarioID;
                }
                if (seleccion == 0)
                {
                    seleccion = null;
                }

                ViewBag.PropietarioID = new SelectList(listaPropietarios, "PropietarioID", "Nombre", seleccion);
            }

            ViewBag.totalValores = totalValores;

            return PartialView("_FormLista");
        }


        // GET: Propietario/Crear
        [CustomAuthorize(permiso = "PropiCrear")]
        public ActionResult Crear()
        {
            Propietario propietario = new Propietario();

            return PartialView("_Crear", propietario);
        }


        // POST: Propietario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PropiCrear")]
        public ActionResult Crear([Bind(Include = "PropietarioID,Nombre")] Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                propietario.Status = true;
                db.Propietarios.Add(propietario);
                db.SaveChanges();

                AlertaSuccess(string.Format("Propietario: <b>{0}</b> se creo con exitó.", propietario.Nombre), true);
                string url = Url.Action("Lista", "Propietario");
                return Json(new { success = true, url = url, modelo = "Propietario" });
            }

            return PartialView("_Crear", propietario);
        }


        // GET: Propietario/Editar/5
        [CustomAuthorize(permiso = "PropiEdit")]
        public ActionResult Editar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Propietario propietario = db.Propietarios.Find(id);
            if (propietario == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", propietario);
        }


        // POST: Propietario/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PropiEdit")]
        public ActionResult Editar([Bind(Include = "PropietarioID,Nombre,Status,AntID")] Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(propietario).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Propietario: <b>{0}</b> se edito con exitó.", propietario.Nombre), true);
                string url = Url.Action("Lista", "Propietario");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", propietario);
        }


        // GET: Propietario/Eliminar/5
        [CustomAuthorize(permiso = "PropiEliminar")]
        public ActionResult Eliminar(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Propietario propietario = db.Propietarios.Find(id);
            if (propietario == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", propietario);
        }

        // POST: Propietario/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "PropiEliminar")]
        public ActionResult EliminarConfirmado(Int64 id)
        {
            string btnValue = Request.Form["accionx"];

            Propietario propietario = db.Propietarios.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    propietario.Status = false;
                    db.Entry(propietario).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se deshabilito <b>{0}</b>", propietario.Nombre), true);

                    break;
                case "eliminar":
                    db.Propietarios.Remove(propietario);
                    db.SaveChanges();
                    AlertaSuccess(string.Format("Se elimino <b>{0}</b>", propietario.Nombre), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }

            string url = Url.Action("Lista", "Propietario");
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
