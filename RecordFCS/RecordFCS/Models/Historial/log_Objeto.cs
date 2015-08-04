using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecordFCS.Models.Historial
{
    public class log_Objeto
    {
        [Key]
        public Guid log_ObjetoID { get; set; }

        [Required]
        [MaxLength(128)]
        public string Descripcion { get; set; }
        public virtual ICollection<log_ObjetoAtributosAdicional> AtributosAdicionales { get; set; }



    }
}