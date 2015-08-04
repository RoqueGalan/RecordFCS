using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RecordFCS.Models
{
    public class AtributoPieza
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Int64 PiezaID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Atributo")]
        public Int64 AtributoID { get; set; }

        public string Valor { get; set; }

        [ForeignKey("ListaValor")]
        public Int64? ListaValorID { get; set; }

        /* Propiedades de navegacion*/
        public virtual Pieza Pieza { get; set; }

        public virtual Atributo Atributo { get; set; }

        public virtual ListaValor ListaValor { get; set; }

    }
}