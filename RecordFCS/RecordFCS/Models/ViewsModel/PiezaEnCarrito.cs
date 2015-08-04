using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ViewsModel
{
    public class PiezaEnCarrito
    {
        public Int64 PiezaID { get; set; }
        public Int64 ObraID { get; set; }
        public string ClavePieza { get; set; }
        public string ClaveObra { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string RutaImagen { get; set; }
    }
}