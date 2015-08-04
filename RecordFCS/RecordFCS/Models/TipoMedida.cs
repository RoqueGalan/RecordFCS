using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models
{
    public class TipoMedida
    {
        [Key]
        public Int64 TipoMedidaID { get; set; }


        [Display(Name = "Tipo Médida")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        [MaxLength(32)]
        public string Nombre { get; set; }


        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        public virtual ICollection<Medida> Medidas { get; set; }

    }
}
