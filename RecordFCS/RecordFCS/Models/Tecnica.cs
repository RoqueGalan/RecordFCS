using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class Tecnica
    {
        [Key]
        [Display(Name = "Técnica")]
        public Int64 TecnicaID { get; set; }


        [Display(Name = "Clave Siglas")]
        public string ClaveSiglas { get; set; }


        [Display(Name = "Clave Texto")]
        public string ClaveTexto { get; set; }


        [Display(Name = "Técnica Padre")]
        public Int64? TecnicaPadreID { get; set; }



        public string MatriculaSiglas { get; set; }



        public string MatriculaTexto { get; set; }


        //[Required]
        public string Descripcion { get; set; }


        public bool Status { get; set; }


        /*Propiedades de navegacion*/
        [ForeignKey("TecnicaPadreID")]
        public virtual Tecnica TecnicaPadre { get; set; }

        public virtual ICollection<TecnicaPieza> TecnicaPiezas { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
        [MaxLength(10)]
        public string AntPadreID { get; set; }
    }
}