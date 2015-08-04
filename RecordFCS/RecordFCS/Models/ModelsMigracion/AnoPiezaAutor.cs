using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ModelsMigracion
{
    public class AnonimoPiezaTabla
    {
        public string id_pieza { get; set; }
        public string Clave { get; set; }
    }


    public class AnonimoImagenPiezaTabla
    {
        public string id_pieza { get; set; }
        public string Consec { get; set; }
        public string ruta_imagen { get; set; }
        public string nombre { get; set; }
    }

    public class AnonimoMedida
    {
        public string id_pieza { get; set; }
        public string UMPeso_Clave { get; set; }
        public double? alto { get; set; }
        public string UMLongitud_Clave { get; set; }
        public double? ancho { get; set; }
        public double? profundo { get; set; }
        public double? diametro { get; set; }
        public double? diametro2 { get; set; }
        public double? peso { get; set; }
        public string cve_tipo_medida { get; set; }
        public string otr_med { get; set; }
    }

    public class AnonimoOtraPieza
    {
        public string id_pieza { get; set; }
        public int Sub_pieza { get; set; }
        public int Consec { get; set; }
        public string MatriculaTecnica_Clave { get; set; }
        public string TipoPieza_Clave { get; set; }
        public string TipoPieza_Descripcion { get; set; }
        public string Ubicacion_Clave { get; set; }
        public string ruta_imagen { get; set; }
        public int nSubIndex { get; set; }
        public double? Alto { get; set; }
        public double? Ancho { get; set; }
        public double? Fondo { get; set; }
        public double? Diametro { get; set; }
        public double? Diametro2 { get; set; }
        public string Descripcion { get; set; }
        public string UbicacionActual { get; set; }
        public int Jerarquia_Clave { get; set; }
    }


}