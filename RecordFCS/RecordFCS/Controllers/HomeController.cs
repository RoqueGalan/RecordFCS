using RecordFCS.Helpers;
using RecordFCS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RecordFCS.Controllers
{
    public class HomeController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        [AllowAnonymous]
        public ActionResult Index(string mensaje = "")
        {
            //string FullName = User.Nombre + " " + User.Apellido;
            if (IsAuthenticated)
            {
                return View("IndexUsuario");
            }

            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                AlertaInfo(mensaje,true);
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Página Acerca de.";

            return View();
        }

        [WordDocument]
        public ActionResult AboutDocument()
        {
            ViewBag.Message = "Página Acerca de.";
            ViewBag.WordDocumentFilename = "AboutMeDocument";
            return View("About");
        }



        [AllowAnonymous]
        public ActionResult HomePdf()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult Pdf()
        //{
        //    var lista = db.Autores.Take(10).ToList();

        //    string rutaReporte = Path.Combine(Server.MapPath("~/Content/Reportes"), "ListadoPlano.rpt");

        //    return new CrystalReportPdfResult(rutaReporte, lista);
        //}


    }
}