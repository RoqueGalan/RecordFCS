using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class Matricula
    {
        [Key]
        public Int64 MatriculaID { get; set; }


        [Display(Name = "Clave Siglas")]
        public string ClaveSigla { get; set; }


        [Display(Name = "Clave Texto")]
        public string ClaveTexto { get; set; }


        [Display(Name = "Matricula Padre")]
        public Int64? MatriculaPadreID { get; set; }



        public string MatriculaSigla { get; set; }


        //[Required]
        public string Descripcion { get; set; }


        public bool Status { get; set; }


        /*Propiedades de navegacion*/
        [ForeignKey("MatriculaPadreID")]
        public virtual Matricula MatriculaPadre { get; set; }

        public virtual ICollection<MatriculaPieza> MatriculaPiezas { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
        [MaxLength(10)]
        public string AntPadreID { get; set; }
    }
}