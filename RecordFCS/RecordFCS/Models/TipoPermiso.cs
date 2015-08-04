using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace RecordFCS.Models
{
    public class TipoPermiso
    {
        [Key]
        public Int64 TipoPermisoID { get; set; }

        [Remote("validarRegistroUnicoClave", "TipoPermiso", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con esta clave. Intenta con otro.")]
        public string Clave { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }

        /* Propiedades de navegacion*/
        public virtual ICollection<Permiso> Permisos { get; set; }
    }
}
