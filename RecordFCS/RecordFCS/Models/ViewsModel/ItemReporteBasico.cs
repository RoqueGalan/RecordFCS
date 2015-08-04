using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ViewsModel
{
    public class ItemReporteBasico
    {
        public string  Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public double Porcentaje { get; set; }

    }
}