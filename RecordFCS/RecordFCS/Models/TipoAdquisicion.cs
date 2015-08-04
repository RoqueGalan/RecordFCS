using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class TipoAdquisicion
    {
        [Key]
        public Int64 TipoAdquisicionID { get; set; }


        [Required]
        [Display(Name = "Tipo de Adquisición")]
        public string Nombre { get; set; }


        public bool Status { get; set; }

        /* Propiedades de navegacion*/
        public virtual ICollection<Obra> Obras { get; set; }


        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}
