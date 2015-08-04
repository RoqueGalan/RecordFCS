using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecordFCS.Models
{
    public class Coleccion
    {
        [Key]
        public Int64 ColeccionID { get; set; }

        [Required]
        [Display(Name = "Colección")]
        public string Nombre { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<Obra> Obras { get; set; }



        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}