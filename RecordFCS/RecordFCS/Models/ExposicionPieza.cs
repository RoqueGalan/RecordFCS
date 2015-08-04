using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class ExposicionPieza
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Int64 PiezaID { get; set; }


        [Key]
        [Column(Order = 2)]
        [ForeignKey("Exposicion")]
        public Int64 ExposicionID { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual Pieza Pieza { get; set; }
        public virtual Exposicion Exposicion { get; set; }
    }
}
