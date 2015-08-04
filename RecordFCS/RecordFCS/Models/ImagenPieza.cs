using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models
{
    public class ImagenPieza
    {
        [NotMapped]
        public string Ruta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImgNombre))
                {
                    return "holder.js/300x200/text:404";
                }
                else
                {
                    return "http://172.16.24.216/museoNew/_museofotos/" + ImgNombre;
                    //return "/Content/img/pieza/" + ImgNombre;
                }
            }
        }

        [NotMapped]
        public string RutaThumb
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImgNombre))
                {
                    return "holder.js/300x200/text:404";
                }
                else
                {
                    return "http://172.16.24.216/museoNew/_museofotos/" + ImgNombre;
                    //return "/Content/img/pieza/mini/" + ImgNombre;
                }
            }
        }

        [Key]
        public Int64 ImagenPiezaID { get; set; }
        public Int64 PiezaID { get; set; }


        public int Orden { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public bool Status { get; set; }


        public string ImgNombre { get; set; }

        /* Propiedades de navegacion*/
        public virtual Pieza Pieza { get; set; }
    }
}