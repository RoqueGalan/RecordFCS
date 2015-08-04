using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS.Models.ViewsModel
{
    public class itemPiezaGenericaCampo
    {
        [ForeignKey("itemPiezaGenerica")]
        public long PiezaID { get; set; }
        public string NombreCampo { get; set; }
        public string ValorCampo { get; set; }

        public int Orden { get; set; }

        public virtual itemPiezaGenerica itemPiezaGenerica { get; set; }
    }
}