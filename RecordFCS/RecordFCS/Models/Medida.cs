using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecordFCS.Models
{
    public enum UMLongitud
    {
        mm, cm, pulgadas, m, km
    }

    public enum UMMasa
    {
        gr, kg
    }

    public class Medida
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Int64 PiezaID { get; set; }


        [Key]
        [Column(Order = 2)]
        [ForeignKey("TipoMedida")]
        public Int64 TipoMedidaID { get; set; }

        //LLxAAxPPxDDxD2D2
        public double? Largo { get; set; }
        public double? Ancho { get; set; }
        public double? Profundidad { get; set; }
        public double? Diametro { get; set; }
        public double? Diametro2 { get; set; }


        [Display(Name = "Unidad Médida Longitud")]
        public UMLongitud? UMLongitud { get; set; }


        public double? Peso { get; set; }


        [Display(Name = "Unidad Médida Peso")]
        public UMMasa? UMMasa { get; set; }

        public bool Status { get; set; }

        public string Otra { get; set; }

        public virtual Pieza Pieza { get; set; }
        public virtual TipoMedida TipoMedida { get; set; }
    }
}