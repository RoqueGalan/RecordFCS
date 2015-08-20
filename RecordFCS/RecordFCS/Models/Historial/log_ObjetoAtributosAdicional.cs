using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RecordFCS.Models.Historial
{
    public class log_ObjetoAtributosAdicional
    {
        [Key]
        public Guid log_ObjetoAtributosAdicionalID { get; set; }

        [Required]
        public Guid log_ObjetoID { get; set; }

        [Required]
        [MaxLength(64)]
        public string AtributoNombre { get; set; }

        public string AtributoValor { get; set; }

        [ForeignKey("log_ObjetoID")]
        public virtual log_Objeto log_Objeto { get; set; }
    }
}
