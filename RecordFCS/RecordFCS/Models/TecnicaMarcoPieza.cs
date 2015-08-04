using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class TecnicaMarcoPieza
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Int64 PiezaID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("TecnicaMarco")]
        public Int64 TecnicaMarcoID { get; set; }



        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }



        /* Propiedades de navegacion*/
        public virtual Pieza Pieza { get; set; }
        public virtual TecnicaMarco TecnicaMarco { get; set; }
    }
}
