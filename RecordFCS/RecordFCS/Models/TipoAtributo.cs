using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace RecordFCS.Models
{
    public class TipoAtributo
    {
        [Key]
        public Int64 TipoAtributoID { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo de Atributo")]
        [StringLength(64)]
        [MaxLength(64)]
        [Remote("validarRegistroUnicoNombre", "TipoAtributo", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con este nombre. Intenta con otro.")]
        public string Nombre { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre en HTML")]
        [StringLength(64)]
        [MaxLength(64)]
        [Remote("validarRegistroUnicoNombreHTML", "TipoAtributo", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con este nombre en html. Intenta con otro.")]
        public string NombreHTML { get; set; }

        [Display(Name = "Nombre ID")]
        [StringLength(64)]
        [MaxLength(64)]
        public string NombreID { get; set; }


        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo de Dato en HTML")]
        [StringLength(32)]
        [MaxLength(32)]
        public string DatoHTML { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo de Dato en C#")]
        [StringLength(32)]
        [MaxLength(32)]
        public string DatoCS { get; set; }

        [Display(Name = "¿Es Catalogo?")]
        public bool EsLista { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }

        [Display(Name = "¿Es Buscador?")]
        public bool Buscador { get; set; }

        [Display(Name = "Orden Buscador")]
        public int? BuscadorOrden { get; set; }



        /* Campos Anteriores */
        public string AntNombre { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<ListaValor> ListaValores { get; set; }
        public virtual ICollection<Atributo> Atributos { get; set; }


    }
}
