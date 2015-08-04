using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecordFCS.Models.Historial
{
    public enum TipoHistorial
    {
        Eliminar, Registrar, Actualizar
    }

    public class log_Historial
    {
        [Key]
        public Guid log_HistorialID { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public Int64 UsuarioID { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public TipoHistorial TipoHistorial { get; set; }

        [Required]
        [MaxLength(128)]
        public string Tabla { get; set; }

        [Required]
        [MaxLength(128)]
        public string ColumnaNombre { get; set; }

        public string ValorOriginal { get; set; }

        public string ValorNuevo { get; set; }

        [ForeignKey("HistorialAnterior")]
        public Guid log_HistorialAnteriorID { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual log_Historial HistorialAnterior { get; set; }
    }
}