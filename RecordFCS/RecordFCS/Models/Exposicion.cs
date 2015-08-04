using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecordFCS.Models
{
    public class Exposicion
    {
        [Key]
        [Display(Name = "Exposición")]
        public Int64 ExposicionID { get; set; }


        [Display(Name = "Exposición")]
        [StringLength(256)]
        public string Nombre { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<ExposicionPieza> ExposicionPiezas { get; set; }


        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}