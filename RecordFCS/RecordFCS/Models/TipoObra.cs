using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace RecordFCS.Models
{
    public class TipoObra
    {
        [Key]
        public Int64 TipoObraID { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo de Obra")]
        [StringLength(64)]
        [Remote("validarRegistroUnico", "TipoObra", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con este nombre. Intenta con otro.")]
        public string Nombre { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }


        /*Propiedades de navegacion*/
        public virtual ICollection<TipoPieza> TipoPiezas { get; set; }
        public virtual ICollection<Obra> Obras { get; set; }
        //public virtual ICollection<Atributo> Atributos { get; set; }

    }
}