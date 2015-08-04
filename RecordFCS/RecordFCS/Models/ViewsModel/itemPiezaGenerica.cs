using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ViewsModel
{
    public class itemPiezaGenerica
    {
        [Key]
        public long PiezaID { get; set; }
        public long ObraID { get; set; }
        public string PiezaClave { get; set; }
        public string ObraClave { get; set; }

        public virtual List<itemPiezaGenericaCampo> itemPiezaGenericaCampos { get; set; }
    }
}