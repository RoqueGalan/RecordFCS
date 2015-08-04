using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecordFCS.Models
{
    public enum Status
    {
        Activo, Inactivo, PreRegistro, EnUso
    }
    public class Obra
    {
        [Key]
        [Display(Name = "Obra")]
        public Int64 ObraID { get; set; }

        [Display(Name = "No. Inventario")]
        public string Clave { get; set; }


        [Display(Name = "Tipo de Obra")]
        [ForeignKey("TipoObra")]
        public Int64 TipoObraID { get; set; }


        [Display(Name = "Tipo de Adquisición")]
        [ForeignKey("TipoAdquisicion")]
        public Int64? TipoAdquisicionID { get; set; }


        [Display(Name = "Propietario")]
        [ForeignKey("Propietario")]
        public Int64? PropietarioID { get; set; }


        [Display(Name = "Colección")]
        [ForeignKey("Coleccion")]
        public Int64? ColeccionID { get; set; }


        [Display(Name = "Ubicación")]
        [ForeignKey("Ubicacion")]
        public Int64? UbicacionID { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }


        public Status Status { get; set; }


        /* Campos Anteriores */
        [MaxLength(10)]
        public string AntID { get; set; }


        /* Propiedades de navegacion*/
        public virtual TipoObra TipoObra { get; set; }
        public virtual TipoAdquisicion TipoAdquisicion { get; set; }
        public virtual Propietario Propietario { get; set; }
        public virtual Coleccion Coleccion { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }

        public virtual ICollection<Pieza> Piezas { get; set; }

    }
}