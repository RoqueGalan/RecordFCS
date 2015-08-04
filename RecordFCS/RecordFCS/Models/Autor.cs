using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecordFCS.Models
{
    public class Autor
    {
        [Key]
        public Int64 AutorID { get; set; }


        [Display(Name = "Nombre(s)")]
        [StringLength(128)]
        public string Nombre { get; set; }


        [Display(Name = "Apellido(s)")]
        [StringLength(128)]
        public string Apellido { get; set; }


        [Display(Name = "Lugar de Nacimiento")]
        [StringLength(128)]
        public string LugarNacimiento { get; set; }


        [Display(Name = "Año de Nacimiento")]
        public string AnioNacimiento { get; set; }


        [Display(Name = "Lugar de Muerte")]
        [StringLength(128)]
        public string LugarMuerte { get; set; }


        [Display(Name = "Año de Nacimiento")]
        public string AnioMuerte { get; set; }

        public string Observaciones { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<AutorPieza> AutorPiezas { get; set; }





        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}