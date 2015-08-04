using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ModelsMigracion
{
    public class m_pieza
    {
        
        public decimal id_pieza { get; set; }

        public int Matricula_Clave { get; set; }
        public string TipoAdquisicion_Clave { get; set; }
        public int MatriculaTecnica_Clave { get; set; }
        public int cve_usuario { get; set; }
        public int Ubicacion_Clave { get; set; }
        public string UbicacionActual { get; set; }
        public int Propietario_Clave { get; set; }
        public int TipoObjeto_Clave { get; set; }
        public string fecha_registro_ORI { get; set; }
        public string estatus { get; set; }
        public string otr_ubic { get; set; }
        public DateTime fecha_ingreso { get; set; }
        public string fecha_registro { get; set; }
        public decimal pieza_treg { get; set; }
        public string tmp { get; set; }
        public decimal usu_id_cap { get; set; }
        public string baja { get; set; }
        public string cInventario { get; set; }
        public string ClassColeccion_Clave { get; set; }
        public string cInvestigacion { get; set; }

        //descriptivo
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

        //obra comun
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

        //public virtual m_pieza_descriptivo m_pieza_descriptivo { get; set; }

    }
}