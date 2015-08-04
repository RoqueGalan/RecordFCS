using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class Atributo
    {
        [Key]
        public Int64 AtributoID { get; set; }
        public Int64 TipoPiezaID { get; set; }
        public Int64 TipoAtributoID { get; set; }


        [Display(Name = "Nombre Alternativo")]
        [StringLength(64)]
        [MaxLength(64)]
        public string NombreAlterno { get; set; }

        public int Orden { get; set; }

        [Display(Name = "¿Requerido?")]
        public bool Requerido { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }

        [Display(Name = "¿En Ficha Básica?")]
        public bool EnFichaBasica { get; set; }

        /* Propiedades de navegacion*/
        public virtual TipoPieza TipoPieza { get; set; }
        public virtual TipoAtributo TipoAtributo { get; set; }

        public virtual ICollection<AtributoPieza> AtributoPiezas { get; set; }
    }
}
