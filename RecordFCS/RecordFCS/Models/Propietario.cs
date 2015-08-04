using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class Propietario
    {
        [Key]
        public Int64 PropietarioID { get; set; }


        //[Required]
        [Display(Name = "Propietario")]
        public int? Nombre { get; set; }


        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<Obra> Obras { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}
