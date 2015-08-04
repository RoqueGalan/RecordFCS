using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace RecordFCS.Models
{
    public class TipoPieza
    {
        [Key]
        public Int64 TipoPiezaID { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo Pieza")]
        [StringLength(128)]
        [Remote("validarRegistroUnicoNombre", "TipoPieza", HttpMethod = "POST", AdditionalFields = "TipoObraID", ErrorMessage = "Ya existe un registro con este nombre. Intenta con otro.")]
        public string Nombre { get; set; }


        [Required(AllowEmptyStrings = false)]
        [StringLength(4, MinimumLength = 1)]
        [MaxLength(4)]
        [Remote("validarRegistroUnicoClave", "TipoPieza", HttpMethod = "POST", AdditionalFields = "TipoObraID", ErrorMessage = "Ya existe un registro con esta clave. Intenta con otra.")]
        public string Clave { get; set; }


        public int Orden { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }

        [ForeignKey("TipoObra")]
        public Int64 TipoObraID { get; set; }

        public bool EsMaestra { get; set; }


        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }


        /*Propiedades de navegacion*/
        public virtual TipoObra TipoObra { get; set; }
        public virtual ICollection<Pieza> Piezas { get; set; }
        public virtual ICollection<Atributo> Atributos { get; set; }
    }
}
