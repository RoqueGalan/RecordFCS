using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace RecordFCS.Models
{
    public class RecordFCSContext : DbContext
    {
        public RecordFCSContext()
            : base("name=DefaultConnection")
        {

        }

        /*
                 * Si no se registran aqui los DBSET no se crean en la Base de datos
                 */

        /* Usuarios y Permisos*/
        //public DbSet<Departamento> Departamentos { get; set; }
        //public DbSet<Puesto> Puestos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoPermiso> TipoPermisos { get; set; }

        /* Catalogos */

        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<TipoObra> TipoObras { get; set; }
        public DbSet<TipoPieza> TipoPiezas { get; set; }
        public DbSet<TipoAtributo> TipoAtributos { get; set; }
        public DbSet<TipoAdquisicion> TipoAdquisiciones { get; set; }
        public DbSet<Atributo> Atributos { get; set; }
        public DbSet<ListaValor> ListaValores { get; set; }
        public DbSet<Exposicion> Exposiciones { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<TipoMedida> TipoMedidas { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Tecnica> Tecnicas { get; set; }
        public DbSet<Coleccion> Colecciones { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<TecnicaMarco> TecnicaMarcos { get; set; }


        /* Registro de Obras */
        public DbSet<Obra> Obras { get; set; }
        public DbSet<Pieza> Piezas { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<AtributoPieza> AtributoPiezas { get; set; }




        /* Registro de Piezas */
        public DbSet<AutorPieza> AutorPiezas { get; set; }
        public DbSet<Medida> Medidas { get; set; }
        public DbSet<ImagenPieza> ImagenPiezas { get; set; }
        public DbSet<ExposicionPieza> ExposicionPiezas { get; set; }
        public DbSet<CatalogoPieza> CatalogoPiezas { get; set; }
        public DbSet<TecnicaPieza> TecnicaPiezas { get; set; }
        public DbSet<MatriculaPieza> MatriculaPiezas { get; set; }
        public DbSet<TecnicaMarcoPieza> TecnicaMarcoPiezas { get; set; }



        /* Bitacora de Entradas y Salidas */





        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //restringir eliminacion de cascada
            modelBuilder.Entity<Obra>().
            HasMany(o => o.Piezas).
            WithRequired(p => p.Obra).
            WillCascadeOnDelete(false);

            modelBuilder.Entity<Atributo>().
            HasMany(a => a.AtributoPiezas).
            WithRequired(ap => ap.Atributo).
            WillCascadeOnDelete(false);




            //modelBuilder.Entity<AtributoPieza>().HasOptional(at=>at.ListaValor);
        }


    }

}
