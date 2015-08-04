using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class MatriculaPieza
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Int64 PiezaID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Matricula")]
        public Int64 MatriculaID { get; set; }



        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }



        /* Propiedades de navegacion*/
        public virtual Pieza Pieza { get; set; }
        public virtual Matricula Matricula { get; set; }
    }
}
