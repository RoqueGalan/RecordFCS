﻿using RecordFCS.Models.Historial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace RecordFCS.Models
{
    public class Usuario
    {
        [Key]
        public Int64 UsuarioID { get; set; }

        [Required]
        //[Remote("EsUsuarioDisponible", "Validacion")]
        [Display(Name = "Nombre de Usuario")]
        [Remote("validarRegistroUnico", "Usuario", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con este nombre. Intenta con otro.")]
        [StringLength(64)]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_])[0-9a-zA-Z!@#$%^&*_0-9]{8,128}$", ErrorMessage = "Contraseña debe contener, Mayuscula, Número, Caracter Especial !@#$%^&*_ y 8 Caracteres Mínimo.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [StringLength(64)]
        public string Password { get; set; }

        [NotMapped]
        [Remote("validarCompararPassword", "Usuario", HttpMethod = "POST", AdditionalFields = "Password", ErrorMessage = "La contraseña no coincide.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar")]
        [StringLength(64)]
        public string ConfirmPassword { get; set; }

                [StringLength(64)]
        public string Nombre { get; set; }
                [StringLength(64)]
        public string Apellido { get; set; }

        [Display(Name = "Correo Eléctronico")]
        [EmailAddress]
        [StringLength(64)]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<log_Historial> Historial { get; set; }

    }
}