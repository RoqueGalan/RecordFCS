using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecordFCS.Models
{
    public class TecnicaMarco
    {
        [Key]
        public Int64 TecnicaMarcoID { get; set; }


        [Display(Name = "Clave Siglas")]
        public string ClaveSigla { get; set; }


        [Display(Name = "Clave Texto")]
        public string ClaveTexto { get; set; }


        [Display(Name = "Tecnica Marco Padre")]
        public Int64? TecnicaMarcoPadreID { get; set; }



        public string MatriculaSigla { get; set; }


        //[Required]
        public string Descripcion { get; set; }


        public bool Status { get; set; }


        /*Propiedades de navegacion*/
        [ForeignKey("TecnicaMarcoPadreID")]
        public virtual TecnicaMarco TecnicaMarcoPadre { get; set; }

        public virtual ICollection<TecnicaMarcoPieza> TecnicaMarcoPiezas { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
        [MaxLength(10)]
        public string AntPadreID { get; set; }
    }
}