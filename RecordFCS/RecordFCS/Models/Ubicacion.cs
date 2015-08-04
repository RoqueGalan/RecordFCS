using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecordFCS.Models
{
    public class Ubicacion
    {
        [Key]
        public Int64 UbicacionID { get; set; }


        [Required]
        [Display(Name = "Ubicación")]
        public string Nombre { get; set; }


        public bool Status { get; set; }

        /* Propiedades de navegacion*/
        public virtual ICollection<Obra> Obras { get; set; }
        public virtual ICollection<Pieza> Piezas { get; set; }



        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}
