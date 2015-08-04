using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class ListaValor
    {
        [Key]
        public Int64 ListaValorID { get; set; }

        [ForeignKey("TipoAtributo")]
        public Int64 TipoAtributoID { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string Valor { get; set; }


        [Display(Name = "¿Activo?")]
        //[DefaultValue("true")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual TipoAtributo TipoAtributo { get; set; }

        public virtual ICollection<AtributoPieza> AtributoPiezas { get; set; }



        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }

    }
}
