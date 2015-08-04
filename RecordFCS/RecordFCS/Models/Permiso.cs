using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class Permiso
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Usuario")]
        public Int64 UsuarioID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("TipoPermiso")]
        public Int64 TipoPermisoID { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }

        /* Propiedades de navegacion*/
        public virtual Usuario Usuario { get; set; }
        public virtual TipoPermiso TipoPermiso { get; set; }
    }
}
