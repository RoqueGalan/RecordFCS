using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ModelsMigracion
{
    public class m_pieza_descriptivo
    {
        public decimal id_pieza { get; set; }

        public int Autor_Clave { get; set; }
        public string titulo { get; set; }
        public int EscArtistica_Clave { get; set; }
        public int FormaAdquisicion_Clave { get; set; }
        public int Procedencia_Clave { get; set; }
        public int FiliacionEstilistica_Clave { get; set; }
        public int CasaComercial_Clave { get; set; }
        public int EdoConservacion_Clave { get; set; }
        public string descripcion { get; set; }
        public string grafica { get; set; }
        public string otros_materiales { get; set; }
        public string catalogo { get; set; }
        public string numero_catalogo { get; set; }
        public string palabra_clave { get; set; }
        public int cve_marco { get; set; }
        public string numero_registro { get; set; }
        public string titulo_ori { get; set; }
        public string Procedencia { get; set; }


        //public virtual m_pieza m_pieza { get; set; }

    }
}