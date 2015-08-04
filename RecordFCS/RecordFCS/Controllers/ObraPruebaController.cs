using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS.Models;

namespace RecordFCS.Controllers
{
    public class ObraPruebaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ObraPrueba
        public ActionResult Index()
        {
            var obras = db.Obras.Include(o => o.Coleccion).Include(o => o.Propietario).Include(o => o.TipoAdquisicion).Include(o => o.TipoObra).Include(o => o.Ubicacion);
            return View(obras.ToList());
        }

        // GET: ObraPrueba/Detalles/5
        public ActionResult Details(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }
            return View(obra);
        }

        // GET: ObraPrueba/Crear
        public ActionResult Create()
        {
            ViewBag.ColeccionID = new SelectList(db.Colecciones, "ColeccionID", "Nombre");
            ViewBag.PropietarioID = new SelectList(db.Propietarios, "PropietarioID", "AntID");
            ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones, "TipoAdquisicionID", "Nombre");
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre");
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre");
            return View();
        }

        // POST: ObraPrueba/Crear
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ObraID,Clave,TipoObraID,TipoAdquisicionID,PropietarioID,ColeccionID,UbicacionID,FechaRegistro,Status")] Obra obra)
        {
            if (ModelState.IsValid)
            {
                db.Obras.Add(obra);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColeccionID = new SelectList(db.Colecciones, "ColeccionID", "Nombre", obra.ColeccionID);
            ViewBag.PropietarioID = new SelectList(db.Propietarios, "PropietarioID", "AntID", obra.PropietarioID);
            ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones, "TipoAdquisicionID", "Nombre", obra.TipoAdquisicionID);
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.TipoObraID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", obra.UbicacionID);
            return View(obra);
        }

        // GET: ObraPrueba/Editar/5
        public ActionResult Edit(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColeccionID = new SelectList(db.Colecciones, "ColeccionID", "Nombre", obra.ColeccionID);
            ViewBag.PropietarioID = new SelectList(db.Propietarios, "PropietarioID", "AntID", obra.PropietarioID);
            ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones, "TipoAdquisicionID", "Nombre", obra.TipoAdquisicionID);
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.TipoObraID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", obra.UbicacionID);
            return View(obra);
        }

        // POST: ObraPrueba/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ObraID,Clave,TipoObraID,TipoAdquisicionID,PropietarioID,ColeccionID,UbicacionID,FechaRegistro,Status")] Obra obra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(obra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColeccionID = new SelectList(db.Colecciones, "ColeccionID", "Nombre", obra.ColeccionID);
            ViewBag.PropietarioID = new SelectList(db.Propietarios, "PropietarioID", "AntID", obra.PropietarioID);
            ViewBag.TipoAdquisicionID = new SelectList(db.TipoAdquisiciones, "TipoAdquisicionID", "Nombre", obra.TipoAdquisicionID);
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.TipoObraID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", obra.UbicacionID);
            return View(obra);
        }

        // GET: ObraPrueba/Eliminar/5
        public ActionResult Delete(Int64? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }
            return View(obra);
        }

        // POST: ObraPrueba/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Int64 id)
        {
            Obra obra = db.Obras.Find(id);
            db.Obras.Remove(obra);
            db.SaveChanges();
            return RedirectToAction("Index");
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
