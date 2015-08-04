using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ModelsMigracion
{
    public class m_pieza_obra_comun
    {
        public decimal id_pieza { get; set; }

        public int FechaEjecucion_Clave { get; set; }
        public string fecha_ejecucion { get; set; }
        public int MTecnicaMarco_Clave { get; set; }
        public string otros_titulos { get; set; }
        public int cve_materiales { get; set; }
        public int numero_piezas { get; set; }
        public string marcas_inscripciones { get; set; }
        public string marcas { get; set; }
        public string observaciones { get; set; }
        public string numeros_anteriores_reg { get; set; }
        public string descripcion_marco { get; set; }
        public int cve_tecnica_marco { get; set; }
        public string bibliografia { get; set; }
        public string asesores { get; set; }
        public string asesores2 { get; set; }

        //public virtual m_pieza m_pieza { get; set; }
    }
}