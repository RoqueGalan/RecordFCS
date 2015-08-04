using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class Catalogo
    {
        [Key]
        public Int64 CatalogoID { get; set; }


        [Display(Name = "Catalogo")]
        [StringLength(256)]
        public string Nombre { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<CatalogoPieza> CatalogoPiezas { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }
    }
}