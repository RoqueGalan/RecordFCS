using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class Pieza
    {
        [Key]
        public Int64 PiezaID { get; set; }
        public Int64 ObraID { get; set; }

        [Display(Name = "No. Interno")]
        public string Clave { get; set; }


        [Display(Name = "Tipo de Pieza")]
        public Int64 TipoPiezaID { get; set; }


        [Display(Name = "Ubicación Actual")]
        public Int64? UbicacionID { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        public bool Status { get; set; }

        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }


        /* Propiedades de navegacion*/
        public virtual Obra Obra { get; set; }
        public virtual TipoPieza TipoPieza { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }

        public virtual ICollection<AutorPieza> AutorPiezas { get; set; }

        public virtual ICollection<ImagenPieza> ImagenPiezas { get; set; }
        public virtual ICollection<Medida> Medidas { get; set; }
        public virtual ICollection<AtributoPieza> AtributoPiezas { get; set; }
        public virtual ICollection<CatalogoPieza> CatalogoPiezas { get; set; }
        public virtual ICollection<TecnicaPieza> TecnicaPiezas { get; set; }
        public virtual ICollection<MatriculaPieza> MatriculaPieza { get; set; }
        public virtual ICollection<TecnicaMarcoPieza> TecnicaMarcoPieza { get; set; }
        public virtual ICollection<ExposicionPieza> ExposicionPiezas { get; set; }


    }
}
