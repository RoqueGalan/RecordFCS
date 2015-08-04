using RecordFCS.Models;
using RecordFCS.Models.ModelsMigracion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS.Controllers
{
    public class MigracionController : BaseController
    {
        private SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["BaseViejaRecord"].ConnectionString);
        private SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["BaseViejaRecord"].ConnectionString);
        private SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["BaseViejaRecord"].ConnectionString);
        private SqlConnection con4 = new SqlConnection(ConfigurationManager.ConnectionStrings["BaseViejaRecord"].ConnectionString);
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ActivarMigracion
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PruebaCarga()
        {
            ViewBag.NombreTabla = "PAGINA DE PRUEBA DE CARGA";
            ViewBag.error = "";
            Thread.Sleep(3000);
            return PartialView("_PruebaCarga");
        }


        //IMPORTAR AUTOR
        public ActionResult ImportarCat_Autor()
        {
            ViewBag.NombreTabla = "CATALOGO DE AUTORES";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Autores.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catAutores]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Autor autor = new Autor();

                        //llenar el registro con los valores viejos
                        autor.Nombre = leer["Autor_Descripcion"].ToString();
                        autor.LugarNacimiento = leer["Autor_LugarNacimiento"].ToString();
                        autor.AnioNacimiento = leer["Autor_AnioNacimiento"].ToString();
                        autor.LugarMuerte = leer["Autor_LugarMuerte"].ToString();
                        autor.AnioMuerte = leer["Autor_AnioMuerte"].ToString();
                        autor.Observaciones = leer["Autor_Observaciones"].ToString();
                        autor.Status = Convert.ToBoolean(leer["Autor_Estatus"]);
                        autor.AntID = leer["Autor_Clave"].ToString();

                        db.Autores.Add(autor);
                        db.SaveChanges();
                    }

                    var lista = db.Autores.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Autor", lista);

                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Autor");
        }


        //IMPORTAR CATALOGO
        public ActionResult ImportarCat_Catalogo()
        {
            ViewBag.NombreTabla = "CATALOGO DE CATALOGOS";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Catalogos.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catCatalogo]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Catalogo catalogo = new Catalogo();

                        //llenar el registro con los valores viejos
                        catalogo.Nombre = leer["Catalogo_Descripcion"].ToString();
                        catalogo.Status = Convert.ToBoolean(leer["Catalogo_Estatus"]);
                        catalogo.AntID = leer["Catalogo_Clave"].ToString();

                        db.Catalogos.Add(catalogo);
                        db.SaveChanges();
                    }

                    var lista = db.Catalogos.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Catalogo", lista);

                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Catalogo");

        }


        //IMPORTAR COLECCION
        public ActionResult ImportarCat_Coleccion()
        {
            ViewBag.NombreTabla = "CATALOGO DE COLECCION";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Colecciones.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catColeccion]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Coleccion coleccion = new Coleccion();

                        //llenar el registro con los valores viejos
                        coleccion.Nombre = leer["Coleccion_Descripcion"].ToString();
                        coleccion.Status = Convert.ToBoolean(leer["Coleccion_Estatus"]);
                        coleccion.AntID = leer["Coleccion_Clave"].ToString();

                        db.Colecciones.Add(coleccion);
                        db.SaveChanges();
                    }

                    var lista = db.Colecciones.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Coleccion", lista);

                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Coleccion");
        }


        //IMPORTAR TECNICA
        public ActionResult ImportarCat_Tecnica()
        {
            ViewBag.NombreTabla = "CATALOGO DE MATRICULAS TECNICAS";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Tecnicas.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catMatriculaTecnica]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Tecnica tecnica = new Tecnica();

                        //llenar el registro con los valores viejos
                        tecnica.ClaveSiglas = leer["clave"].ToString();
                        tecnica.ClaveTexto = leer["descripcion_clave"].ToString();

                        tecnica.TecnicaPadreID = null;



                        tecnica.MatriculaSiglas = leer["matricula"].ToString();
                        tecnica.MatriculaTexto = leer["descripcion"].ToString();
                        tecnica.Descripcion = leer["cDescripcion"].ToString();

                        if (leer["estatus"].ToString() == "0")
                        {
                            tecnica.Status = false;
                        }
                        else
                        {
                            tecnica.Status = true;
                        }


                        tecnica.AntID = leer["MatriculaTecnica_Clave"].ToString();
                        tecnica.AntPadreID = leer["cat_padre"].ToString();

                        db.Tecnicas.Add(tecnica);
                        db.SaveChanges();
                    }


                    //ASIGNAR LA TECNICA PADRE ID
                    foreach (var item in db.Tecnicas.ToList())
                    {
                        string padre = item.AntPadreID;
                        if (!string.IsNullOrEmpty(padre))
                        {
                            if (!string.IsNullOrWhiteSpace(padre))
                            {
                                if (padre != "0")
                                {
                                    //buscar el ID de la tecnica que es padre 
                                    var tecnica = db.Tecnicas.Where(a => a.AntID == padre);
                                    if (tecnica.Count() > 0)
                                    {
                                        item.TecnicaPadreID = tecnica.FirstOrDefault().TecnicaID;
                                        db.Entry(item).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    var lista = db.Tecnicas.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Tecnica", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
                ViewBag.error = "error";

            }
            return PartialView("_ImportarCat_Tecnica");
        }


        //IMPORTAR TECNICA
        public ActionResult ImportarCat_TecnicaMarco()
        {
            ViewBag.NombreTabla = "CATALOGO DE TECNICA MARCO";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.TecnicaMarcos.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catMatriculaTecnicaMarco]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        TecnicaMarco tecnicaMarco = new TecnicaMarco();

                        //llenar el registro con los valores viejos
                        tecnicaMarco.ClaveSigla = leer["clave"].ToString();
                        tecnicaMarco.ClaveTexto = leer["descripcion_clave"].ToString();

                        tecnicaMarco.TecnicaMarcoPadreID = null;



                        tecnicaMarco.MatriculaSigla = leer["matricula"].ToString();
                        tecnicaMarco.Descripcion = leer["descripcion"].ToString();


                        tecnicaMarco.Status = true;



                        tecnicaMarco.AntID = leer["MTecnicaMarco_Clave"].ToString();
                        tecnicaMarco.AntPadreID = leer["cat_padre"].ToString();

                        db.TecnicaMarcos.Add(tecnicaMarco);
                        db.SaveChanges();
                    }


                    //ASIGNAR LA TECNICA PADRE ID
                    foreach (var item in db.TecnicaMarcos.ToList())
                    {
                        string padre = item.AntPadreID;
                        if (!string.IsNullOrEmpty(padre))
                        {
                            if (!string.IsNullOrWhiteSpace(padre))
                            {
                                if (padre != "0")
                                {
                                    //buscar el ID de la tecnica que es padre 
                                    var tecnicaMarco = db.TecnicaMarcos.Where(a => a.AntID == padre);
                                    if (tecnicaMarco.Count() > 0)
                                    {
                                        item.TecnicaMarcoPadreID = tecnicaMarco.FirstOrDefault().TecnicaMarcoID;
                                        db.Entry(item).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    var lista = db.TecnicaMarcos.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_TecnicaMarcos", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
                ViewBag.error = "error";

            }
            return PartialView("_ImportarCat_TecnicaMarcos");
        }


        //IMPORTAR MATRICULA
        public ActionResult ImportarCat_Matricula()
        {
            ViewBag.NombreTabla = "CATALOGO DE MATRICULAS";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Matriculas.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catMatricula]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Matricula matricula = new Matricula();

                        //llenar el registro con los valores viejos
                        matricula.ClaveSigla = leer["clave"].ToString();
                        matricula.ClaveTexto = leer["descripcion_clave"].ToString();

                        matricula.MatriculaPadreID = null;



                        matricula.MatriculaSigla = leer["matricula"].ToString();
                        matricula.Descripcion = leer["descripcion"].ToString();


                        matricula.Status = true;



                        matricula.AntID = leer["Matricula_Clave"].ToString();
                        matricula.AntPadreID = leer["cat_padre"].ToString();

                        db.Matriculas.Add(matricula);
                        db.SaveChanges();
                    }


                    //ASIGNAR LA TECNICA PADRE ID
                    foreach (var item in db.Matriculas.ToList())
                    {
                        string padre = item.AntPadreID;
                        if (!string.IsNullOrEmpty(padre))
                        {
                            if (!string.IsNullOrWhiteSpace(padre))
                            {
                                if (padre != "0")
                                {
                                    //buscar el ID de la tecnica que es padre 
                                    var matricula = db.Matriculas.Where(a => a.AntID == padre);
                                    if (matricula.Count() > 0)
                                    {
                                        item.MatriculaPadreID = matricula.FirstOrDefault().MatriculaID;
                                        db.Entry(item).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    var lista = db.Matriculas.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Matricula", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
                ViewBag.error = "error";

            }
            return PartialView("_ImportarCat_Matricula");
        }


        //IMPORTAR PROPIETARIO
        public ActionResult ImportarCat_Propietario()
        {
            ViewBag.NombreTabla = "CATALOGO DE PROPIETARIOS";
            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Propietarios.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catPropietario]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Propietario propietario = new Propietario();

                        //llenar el registro con los valores viejos
                        if (string.IsNullOrEmpty(leer["Propietario_Descripcion"].ToString()))
                        {
                            propietario.Nombre = 0;
                        }
                        else
                        {
                            propietario.Nombre = Convert.ToInt32(leer["Propietario_Descripcion"]);
                        }



                        propietario.Status = Convert.ToBoolean(leer["Propietario_Estatus"]);
                        propietario.AntID = leer["Propietario_Clave"].ToString();

                        db.Propietarios.Add(propietario);
                        db.SaveChanges();
                    }

                    var lista = db.Propietarios.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Propietario", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Propietario");
        }


        //IMPORTAR TIPO ADQUISICION
        public ActionResult ImportarCat_TipoAdquisicion()
        {
            ViewBag.NombreTabla = "CATALOGO DE TIPO DE ADQUISICIONES";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.TipoAdquisiciones.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catTipoAdquisicion]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        TipoAdquisicion tipoAdquisicion = new TipoAdquisicion();

                        //llenar el registro con los valores viejos

                        tipoAdquisicion.Nombre = leer["TipoAdquisicion_Descripcion"].ToString();
                        tipoAdquisicion.Status = Convert.ToBoolean(leer["TipoAdquisicion_Estatus"]);
                        tipoAdquisicion.AntID = leer["TipoAdquisicion_Clave"].ToString();

                        db.TipoAdquisiciones.Add(tipoAdquisicion);
                        db.SaveChanges();
                    }

                    var lista = db.TipoAdquisiciones.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_TipoAdquisicion", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_TipoAdquisicion");
        }


        //IMPORTAR UBICACION
        public ActionResult ImportarCat_Ubicacion()
        {
            ViewBag.NombreTabla = "CATALOGO DE UBICACIONES";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Ubicaciones.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catUbicacion]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Ubicacion ubicacion = new Ubicacion();

                        //llenar el registro con los valores viejos

                        ubicacion.Nombre = leer["Ubicacion_Descripcion"].ToString();
                        ubicacion.Status = Convert.ToBoolean(leer["Ubicacion_Estatus"]);
                        ubicacion.AntID = leer["Ubicacion_Clave"].ToString();

                        db.Ubicaciones.Add(ubicacion);
                        db.SaveChanges();
                    }

                    var lista = db.Ubicaciones.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Ubicacion", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Ubicacion");
        }


        //IMPORTAR EXPOSICION
        public ActionResult ImportarCat_Exposicion()
        {
            ViewBag.NombreTabla = "CATALOGO DE EXPOSICIONES";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.Exposiciones.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [m_guion]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        Exposicion exposicion = new Exposicion();

                        //llenar el registro con los valores viejos

                        exposicion.Nombre = leer["gui_nombre"].ToString();
                        exposicion.Status = true;
                        exposicion.AntID = leer["gui_id"].ToString();

                        db.Exposiciones.Add(exposicion);
                        db.SaveChanges();
                    }

                    var lista = db.Exposiciones.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_Exposicion", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_Exposicion");
        }


        //IMPORTAR CAT GENERICO
        public ActionResult ImportarCat_Generico(string NombreHTML)
        {

            ViewBag.NombreTabla = "CATALOGO DE " + NombreHTML;

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar que el nombreCatalogo exista, NOmbres disponibles en 
                /*
                 *  NombreHTML              ---     TABLA ANTERIOR
                 *  --------------------------------------------------
                 *  CasaComercialID         ---     catCasaComercial
                 *  EstadoConservacionID    ---     catEdoConservacion
                 *  EscuelaArtisticaID      ---     catEscArtistica
                 *  FechaEjecucionID        ---     catFechaEjecucion
                 *  FiliacionEstilisticaID  ---     catFiliacionEstilistica
                 *  FormaAdquisicionID      ---     catFormaAdquisicion
                 *  ProcedenciaID           ---     catProcedencia
                 */

                TipoAtributo tipoAtributo = db.TipoAtributos.Single(a => a.NombreHTML == NombreHTML && a.EsLista);
                if (tipoAtributo != null)
                {
                    //revisar el contador de registros
                    if (db.ListaValores.Where(a => a.TipoAtributoID == tipoAtributo.TipoAtributoID).ToList().Count > 0)
                    {
                        //si hay por lo menos un registro ya se ocupo la tabla
                        ViewBag.error = "error";
                    }
                    else
                    {
                        //tabla de coincidencias
                        string textSql = "";
                        string campo = "";
                        //definir el sql
                        switch (tipoAtributo.NombreHTML)
                        {
                            case "CasaComercialID":
                                textSql = string.Format("SELECT * FROM [catCasaComercial]");
                                campo = "CasaComercial";
                                break;
                            case "EstadoConservacionID":
                                textSql = string.Format("SELECT * FROM [catEdoConservacion]");
                                campo = "EdoConservacion";
                                break;
                            case "EscuelaArtisticaID":
                                textSql = string.Format("SELECT * FROM [catEscArtistica]");
                                campo = "EscArtistica";
                                break;
                            case "FechaEjecucionID":
                                textSql = string.Format("SELECT * FROM [catFechaEjecucion]");
                                campo = "FechaEjecucion";
                                break;
                            case "FiliacionEstilisticaID":
                                textSql = string.Format("SELECT * FROM [catFiliacionEstilistica]");
                                campo = "FiliacionEstilistica";
                                break;
                            case "FormaAdquisicionID":
                                textSql = string.Format("SELECT * FROM [catFormaAdquisicion]");
                                campo = "FormaAdquisicion";
                                break;
                            case "ProcedenciaID":
                                textSql = string.Format("SELECT * FROM [catProcedencia]");
                                campo = "Procedencia";
                                break;
                        }

                        SqlCommand sql = new SqlCommand(textSql, con1);
                        //ejecutar el sql
                        SqlDataReader leer = sql.ExecuteReader();
                        //realizar el foreach
                        while (leer.Read())
                        {
                            //definir el tipo de tabla
                            ListaValor listaValor = new ListaValor();

                            //llenar el registro con los valores viejos
                            listaValor.TipoAtributoID = tipoAtributo.TipoAtributoID;
                            listaValor.Valor = leer[campo + "_Descripcion"].ToString();
                            listaValor.Status = Convert.ToBoolean(leer[campo + "_Estatus"]);
                            listaValor.AntID = leer[campo + "_Clave"].ToString();

                            db.ListaValores.Add(listaValor);
                            db.SaveChanges();
                        }

                        var lista = db.ListaValores.Where(a => a.TipoAtributoID == tipoAtributo.TipoAtributoID).ToList();
                        ViewBag.TotalRegistros = lista.Count;
                        ViewBag.NombreCatalogo = tipoAtributo.Nombre;

                        return PartialView("_ImportarCat_Generico", lista);
                    }

                }
                else
                {
                    ViewBag.error = "error";
                    ViewBag.mensaje = "No existe el nombre del catalogo";
                }




            }
            catch (Exception)
            {
                ViewBag.error = "error";
                ViewBag.mensaje = "Conexión fallida";
            }

            return PartialView("_ImportarCat_Generico");
        }


        //IMPORTAR TIPO DE OBRA Y QUIZAS TIPO DE PIEZA
        public ActionResult ImportarCat_TipoObra()
        {
            ViewBag.NombreTabla = "CATALOGO DE TIPOS DE OBRAS";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //revisar el contador de registros
                if (db.TipoObras.ToList().Count > 0)
                {
                    //si hay por lo menos un registro ya se ocupo la tabla
                    ViewBag.error = "error";
                }
                else
                {
                    //definir el sql
                    string textSql = string.Format("SELECT * FROM [catTipoObjeto] ORDER BY [TipoObjeto_Descripcion]");
                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach
                    while (leer.Read())
                    {
                        //definir el tipo de tabla
                        TipoObra tipoObra = new TipoObra();

                        //llenar el registro con los valores viejos

                        tipoObra.Nombre = leer["TipoObjeto_Descripcion"].ToString();
                        tipoObra.Status = Convert.ToBoolean(leer["TipoObjeto_Estatus"]);
                        tipoObra.AntID = leer["TipoObjeto_Clave"].ToString();

                        db.TipoObras.Add(tipoObra);
                        db.SaveChanges();

                        // despues de guardar el tipo de obra
                        // crear una pieza Maestra

                        TipoPieza tipoPieza = new TipoPieza();
                        tipoPieza.Nombre = "Maestra v1.0";
                        tipoPieza.Clave = "A";
                        tipoPieza.Orden = 1;
                        tipoPieza.Status = true;
                        tipoPieza.TipoObraID = tipoObra.TipoObraID;
                        tipoPieza.EsMaestra = true;
                        tipoPieza.AntID = tipoObra.AntID;

                        db.TipoPiezas.Add(tipoPieza);
                        db.SaveChanges();

                        //Agregar Atributos de registro
                        /*
                         * No Inventario
                         * TipoObra
                         * TipoAdquisicion
                         * Propietario
                         * Ubicacion
                         * FechaRegistro
                         * Status
                         * AntID
                         * Coleccion
                         */

                        //No Inventario
                        List<string> listaAtributos = new List<string>() 
                        {
                            "No. de Inventario",
                            "Clave Pieza",
                            "Tipo de Obra",
                            "Tipo de Adquisición",
                            "Propietario",
                            "Ubicación",
                            "FechaRegistro",
                            "Colección"
                        };

                        int i = 1;
                        foreach (var nombreAtributo in listaAtributos)
                        {
                            var att = new Atributo()
                            {
                                TipoPiezaID = tipoPieza.TipoPiezaID,
                                TipoAtributoID = db.TipoAtributos.Single(a => a.NombreHTML == nombreAtributo).TipoAtributoID,
                                Orden = i,
                                Requerido = true,
                                Status = true,
                                EnFichaBasica = true
                            };

                            db.Atributos.Add(att);
                            db.SaveChanges();

                            i++;
                        }






                    }

                    var lista = db.TipoObras.ToList();
                    ViewBag.TotalRegistros = lista.Count;

                    return PartialView("_ImportarCat_TipoObra", lista);
                }
            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarCat_TipoObra");
        }


        //---------------------------------------------------------------------------

        public ActionResult ImportarObras_Obra()
        {
            // Inicia el contador:
            Stopwatch tiempo = Stopwatch.StartNew();

            ViewBag.NombreTabla = "CATALOGO DE OBRAS - REGISTRO BASICO";

            ViewBag.error = "";

            try
            {
                //abrir conexion


                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //List<Obra> lista = new List<Obra>();

                //definir el sql
                var total = 1000;
                var inicio = 99331; //ultimo registro en la base vieja 
                var fin = inicio + total;

                var limite = 105475;

                while (fin <= limite)
                {
                    con1.Open();
                    string textSql = string.Format("SELECT * FROM [m_pieza] WHERE [id_pieza] > {0} AND [id_pieza] <= {1}", inicio, fin);
                    //string textSql = string.Format("SELECT * FROM [m_pieza]");

                    SqlCommand sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    SqlDataReader leer = sql.ExecuteReader();
                    //realizar el foreach


                    while (leer.Read())
                    {
                        //registros a traer y donde guardarlos
                        /*
                         * OK ---------- O B R A ------------------------------
                         * id_pieza	                = obra.Clave, obra.AntID
                         * TipoAdquisicion_Clave	= obra.TipoAdquisicionID
                         * Ubicacion_Clave	        = obra.UbicacionID
                         * Propietario_Clave	    = obra.PropietarioID
                         * TipoObjeto_Clave	        = obra.TipoObraID
                         * fecha_registro_ORI	    = obra.FechaRegistro
                         * estatus	                = obra.Status
                         * -------------------------------------------------
                         * --------- P I E Z A   M A E S T R A -------------
                         * Matricula_Clave	        = pieza.MatriculaPieza
                         * MatriculaTecnica_Clave	= pieza.TecnicaPieza
                         * UbicacionActual	        = pieza.UbicacionID
                         * fecha_registro	        = pieza.FechaRegistro
                         * 
                         * ClassColeccion_Clave	    = pieza.ColeccionGibranPieza
                         * 
                         * --------- N O   S E   U S A N ------------------
                         * ---pieza_treg	
                         * ---tmp	
                         * ---usu_id_cap	
                         * ---baja	
                         * ---cInventario	
                         * ---cInvestigacion
                         * ---cve_usuario	 
                         * ---otr_ubic	
                         * ---fecha_ingreso	    
                         * 
                         * 
                         */

                        // crear un registro de OBRA--------------------------
                        Obra obra = new Obra();

                        obra.Clave = leer["id_pieza"].ToString();

                        string tipoObraText = leer["TipoObjeto_Clave"].ToString();
                        var xTipoObras = db.TipoObras.Where(a => a.AntID == tipoObraText).Select(a => a.TipoObraID);
                        if (xTipoObras.Count() > 0)
                            obra.TipoObraID = xTipoObras.FirstOrDefault();
                        else
                            obra.TipoObraID = db.TipoObras.Single(a => a.Nombre == "S/D").TipoObraID;


                        string tipoAdqquisicionText = leer["TipoAdquisicion_Clave"].ToString();
                        var xTipoAdquisicion = db.TipoAdquisiciones.Where(a => a.AntID == tipoAdqquisicionText).Select(a => a.TipoAdquisicionID);
                        if (xTipoAdquisicion.Count() > 0)
                            obra.TipoAdquisicionID = xTipoAdquisicion.FirstOrDefault();


                        string propietarioText = leer["Propietario_Clave"].ToString();
                        var xPropietario = db.Propietarios.Where(a => a.AntID == propietarioText).Select(a => a.PropietarioID);
                        if (xPropietario.Count() > 0)
                            obra.PropietarioID = xPropietario.FirstOrDefault();
                        else
                            obra.PropietarioID = db.Propietarios.Single(a => a.AntID == "24006").PropietarioID;


                        string ubicacionText = leer["Ubicacion_Clave"].ToString();
                        var xUbicacion = db.Ubicaciones.Where(a => a.AntID == ubicacionText).Select(a => a.UbicacionID);
                        if (xUbicacion.Count() > 0)
                            obra.UbicacionID = xPropietario.FirstOrDefault();
                        else
                            obra.UbicacionID = db.Ubicaciones.Single(a => a.AntID == "4036").UbicacionID;




                        //cortar la fecha MM/dd/yyy y colocarla como dd/MM/yyy

                        //DateTime fecha=DateTime.ParseExact(leer["fecha_registro_ORI"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                        obra.FechaRegistro = DateTime.Now;
                        obra.Status = Status.Activo;



                        obra.AntID = leer["id_pieza"].ToString();

                        //extraer m_pieza_coleccion
                        //obra.ColeccionID = ;

                        con2.Open();
                        //definir el sql
                        string textSql_int = string.Format("SELECT TOP 1 * FROM [m_pieza_coleccion] WHERE [id_pieza] = {0}", obra.AntID);

                        SqlCommand sql_int = new SqlCommand(textSql_int, con2);
                        //ejecutar el sql
                        SqlDataReader leer_int = sql_int.ExecuteReader();
                        //realizar el foreach

                        leer_int.Read();
                        var coleccionText = leer_int["Coleccion_Clave"].ToString();
                        if (coleccionText != "0" || coleccionText != "")
                        {
                            //realizar la buscqueda en coleccion con el antID
                            var xColeccion = db.Colecciones.Where(a => a.AntID == coleccionText).Select(a => a.ColeccionID);
                            if (xColeccion.Count() > 0)
                                obra.ColeccionID = xColeccion.FirstOrDefault();
                        }

                        con2.Close();

                        db.Obras.Add(obra);
                        db.SaveChanges();

                        //creo su pieza con la Maestra v.1
                        /*
                         * UbicacionActual	        = pieza.UbicacionID
                         * fecha_registro	        = pieza.FechaRegistro
                         */

                        //var tipoPieza = obra.TipoObra.TipoPiezas.FirstOrDefault();
                        var tipoPieza = db.TipoObras.Find(obra.TipoObraID).TipoPiezas.FirstOrDefault();


                        Pieza piezaMaestra = new Pieza()
                        {
                            ObraID = obra.ObraID,
                            Clave = obra.Clave + "-" + tipoPieza.Clave,
                            TipoPiezaID = tipoPieza.TipoPiezaID,
                            FechaRegistro = obra.FechaRegistro,
                            Status = true
                        };

                        string ubicacionActual = leer["UbicacionActual"].ToString();
                        var xUbicacion2 = db.Ubicaciones.Where(a => a.AntID == ubicacionActual).Select(a => a.UbicacionID);
                        if (xUbicacion2.Count() > 0)
                            piezaMaestra.UbicacionID = xUbicacion2.FirstOrDefault();
                        else
                            piezaMaestra.UbicacionID = obra.UbicacionID;


                        db.Piezas.Add(piezaMaestra);
                        db.SaveChanges();

                        //lista.Add(obra);

                        //agregarle los atributos de las piezas


                    }

                    inicio = fin;
                    fin = fin + total;
                    con1.Close();
                }

                ViewBag.TiempoTotal = tiempo.Elapsed.TotalSeconds.ToString();

                var lista = db.Obras.ToList();
                ViewBag.TotalRegistros = lista.Count;

                // Para el contador e imprime el resultado:


                return PartialView("_ImportarObras_Obra", lista);
            }
            catch (Exception)
            {
                ViewBag.error = "error";

                ViewBag.mensaje = "Conexión fallida";
            }

            ViewBag.TiempoTotal = tiempo.Elapsed.TotalSeconds.ToString();
            return PartialView("_ImportarObras_Obra");
        }


        //IMPORTAR PIEZA - AUTOR
        public ActionResult ImportarPieza_Autor()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA AUTOR";
            var tipoAtt_AutorID = dbx.TipoAtributos.Where(a => a.AntNombre == "Autor_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaAutores = dbx.Autores.Select(a => new { a.AutorID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string textSql1 = string.Format("SELECT [id_pieza], [Autor_Clave] FROM [m_pieza_descriptivo]");
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1["id_pieza"].ToString();
                    string clave = leer1["Autor_Clave"].ToString();
                    listaAnt.Add(new AnonimoPiezaTabla()
                    {
                        id_pieza = id,
                        Clave = clave
                    });

                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anoPiezaAutor = listaAnt.FirstOrDefault(a => a.id_pieza == pieza.AntID);
                        //buscar al Autor
                        var autor = listaAutores.FirstOrDefault(a => a.AntID == anoPiezaAutor.Clave);

                        AutorPieza autorPieza = new AutorPieza()
                        {
                            PiezaID = pieza.PiezaID,
                            AutorID = autor.AutorID,
                            Status = true
                        };

                        dbx.AutorPiezas.Add(autorPieza);

                    }
                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA DESCRIPTIVO ALTERNATIVO
        public ActionResult ImportarPieza_Descriptivo_alternativo()
        {
            var tipoAtt_Autor = db.TipoAtributos.Where(a => a.AntNombre == "Autor_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_titulo = db.TipoAtributos.Where(a => a.AntNombre == "titulo").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_EscArt = db.TipoAtributos.Where(a => a.AntNombre == "EscArtistica_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_FormAdq = db.TipoAtributos.Where(a => a.AntNombre == "FormaAdquisicion_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_ProcedenList = db.TipoAtributos.Where(a => a.AntNombre == "Procedencia_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_FiliEst = db.TipoAtributos.Where(a => a.AntNombre == "FiliacionEstilistica_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_CasaComer = db.TipoAtributos.Where(a => a.AntNombre == "CasaComercial_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_EdoConserv = db.TipoAtributos.Where(a => a.AntNombre == "EdoConservacion_Clave").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_descripcion = db.TipoAtributos.Where(a => a.AntNombre == "descripcion").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_otros_mat = db.TipoAtributos.Where(a => a.AntNombre == "otros_materiales").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_catalogo = db.TipoAtributos.Where(a => a.AntNombre == "catalogo").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_num_cat = db.TipoAtributos.Where(a => a.AntNombre == "numero_catalogo").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_num_reg = db.TipoAtributos.Where(a => a.AntNombre == "numero_registro").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_titu_ori = db.TipoAtributos.Where(a => a.AntNombre == "titulo_ori").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();
            var tipoAttGen_Proceden = db.TipoAtributos.Where(a => a.AntNombre == "Procedencia").AsEnumerable().Select(a => new { a.TipoAtributoID, a.AntNombre, a.EsLista }).FirstOrDefault();


            List<string> camposLista = new List<string>()
            { 
                "Autor_Clave",
                "titulo", 
                "EscArtistica_Clave",
                "FormaAdquisicion_Clave",
                "Procedencia_Clave",
                "FiliacionEstilistica_Clave",
                "CasaComercial_Clave",
                "EdoConservacion_Clave",
                "descripcion",
                "otros_materiales",
                "catalogo",
                "numero_catalogo",
                "numero_registro",
                "titulo_ori",
                "Procedencia"
            };



            ViewBag.NombreTabla = "CATALOGO DE OBRAS - PIEZA DESCRIPTIVO ALTERNATIVO";

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 actual = 2528; //colocar el OBRAID
                Int64 limite = 70000;

                while (actual <= limite)
                {
                    //todo sera sobre la obra y pieza default
                    var obra = db.Obras.Find(actual);

                    if (obra != null)
                    {
                        var piezaMaestra = obra.Piezas.FirstOrDefault();
                        if (piezaMaestra != null)
                        {
                            string AnteriorID = obra.AntID;
                            con1.Open();
                            string textSql1 = string.Format("SELECT TOP 1 * FROM [m_pieza_descriptivo] WHERE [id_pieza] = {0}", AnteriorID);
                            SqlCommand sql1 = new SqlCommand(textSql1, con1);
                            SqlDataReader leer1 = sql1.ExecuteReader();

                            if (leer1.FieldCount > 0)
                            {
                                leer1.Read();
                                foreach (var campoText in camposLista)
                                {
                                    TipoAtributo tipoAtributo = null;
                                    switch (campoText)
                                    {
                                        case "Autor_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAtt_Autor.TipoAtributoID, EsLista = tipoAtt_Autor.EsLista, AntNombre = tipoAtt_Autor.AntNombre }; break;
                                        case "titulo": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_titulo.TipoAtributoID, EsLista = tipoAttGen_titulo.EsLista, AntNombre = tipoAttGen_titulo.AntNombre }; break;
                                        case "EscArtistica_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_EscArt.TipoAtributoID, EsLista = tipoAttGen_EscArt.EsLista, AntNombre = tipoAttGen_EscArt.AntNombre }; break;
                                        case "FormaAdquisicion_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_FormAdq.TipoAtributoID, EsLista = tipoAttGen_FormAdq.EsLista, AntNombre = tipoAttGen_FormAdq.AntNombre }; break;
                                        case "Procedencia_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_ProcedenList.TipoAtributoID, EsLista = tipoAttGen_ProcedenList.EsLista, AntNombre = tipoAttGen_ProcedenList.AntNombre }; break;
                                        case "FiliacionEstilistica_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_FiliEst.TipoAtributoID, EsLista = tipoAttGen_FiliEst.EsLista, AntNombre = tipoAttGen_FiliEst.AntNombre }; break;
                                        case "CasaComercial_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_CasaComer.TipoAtributoID, EsLista = tipoAttGen_CasaComer.EsLista, AntNombre = tipoAttGen_CasaComer.AntNombre }; break;
                                        case "EdoConservacion_Clave": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_EdoConserv.TipoAtributoID, EsLista = tipoAttGen_EdoConserv.EsLista, AntNombre = tipoAttGen_EdoConserv.AntNombre }; break;
                                        case "descripcion": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_descripcion.TipoAtributoID, EsLista = tipoAttGen_descripcion.EsLista, AntNombre = tipoAttGen_descripcion.AntNombre }; break;
                                        case "otros_materiales": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_otros_mat.TipoAtributoID, EsLista = tipoAttGen_otros_mat.EsLista, AntNombre = tipoAttGen_otros_mat.AntNombre }; break;
                                        case "catalogo": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_catalogo.TipoAtributoID, EsLista = tipoAttGen_catalogo.EsLista, AntNombre = tipoAttGen_catalogo.AntNombre }; break;
                                        case "numero_catalogo": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_num_cat.TipoAtributoID, EsLista = tipoAttGen_num_cat.EsLista, AntNombre = tipoAttGen_num_cat.AntNombre }; break;
                                        case "numero_registro": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_num_reg.TipoAtributoID, EsLista = tipoAttGen_num_reg.EsLista, AntNombre = tipoAttGen_num_reg.AntNombre }; break;
                                        case "titulo_ori": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_titu_ori.TipoAtributoID, EsLista = tipoAttGen_titu_ori.EsLista, AntNombre = tipoAttGen_titu_ori.AntNombre }; break;
                                        case "Procedencia": tipoAtributo = new TipoAtributo() { TipoAtributoID = tipoAttGen_Proceden.TipoAtributoID, EsLista = tipoAttGen_Proceden.EsLista, AntNombre = tipoAttGen_Proceden.AntNombre }; break;
                                        default:
                                            ViewBag.mensajeError = "No existe la Obra";
                                            break;
                                    };


                                    if (tipoAtributo != null)
                                    {
                                        //Extraer el valor del query leer1
                                        string valorCampoExtra = leer1[campoText].ToString();
                                        //validar el valor del campo
                                        bool todoOK = true;

                                        if (tipoAtributo.EsLista)
                                        {
                                            if (valorCampoExtra == "0")
                                                todoOK = false;
                                        }
                                        else
                                        {
                                            if (valorCampoExtra == "0" || valorCampoExtra == "" || valorCampoExtra == " " || valorCampoExtra == "-" || valorCampoExtra == " -" || valorCampoExtra == "." || valorCampoExtra == " ." || valorCampoExtra == "Pendiente por definir")
                                                todoOK = false;
                                        };

                                        if (todoOK)
                                        {
                                            //Buscar que el ATRIBUTO Exista en TipoPieza con TipoPieza y TipoAtributo
                                            Int64 atributoID = db.Atributos.Where(a => a.TipoAtributoID == tipoAtributo.TipoAtributoID && a.TipoPiezaID == piezaMaestra.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();

                                            if (atributoID == 0)
                                            {
                                                //si ATRIBUTO no existe entonces crearlo, primero crear
                                                //en TIPOOBRA el TipoAtributo

                                                //TIPOPIEZA * ATRIBUTOS
                                                Atributo atributo = new Atributo()
                                                {
                                                    TipoPiezaID = piezaMaestra.TipoPiezaID,
                                                    TipoAtributoID = tipoAtributo.TipoAtributoID,
                                                    NombreAlterno = null,
                                                    Orden = 98,
                                                    Status = true,
                                                    Requerido = false,
                                                    EnFichaBasica = false

                                                };

                                                db.Atributos.Add(atributo);
                                                db.SaveChanges();

                                                atributoID = atributo.AtributoID;
                                            }//fin creacion de atributo null


                                            //comenzar creacion del AtributoPieza con el valor extraido y validado
                                            //se podria generar la validacion de si ya existe, actualizarlo
                                            bool crear = false;
                                            AtributoPieza attPieza = db.AtributoPiezas.Find(piezaMaestra.PiezaID, atributoID);
                                            if (attPieza == null)
                                            {
                                                //crear el AtributoPieza
                                                crear = true;
                                                attPieza = new AtributoPieza()
                                                {
                                                    PiezaID = piezaMaestra.PiezaID,
                                                    AtributoID = atributoID,
                                                };
                                            }

                                            if (campoText == "Autor_Clave")
                                            {
                                                //generar codigo para el registro de AutorPieza
                                                //verificar si ya existe y si no actualizarlo
                                                //buscar el autor

                                                Int64 AutorID = db.Autores.Where(a => a.AntID == valorCampoExtra).Select(a => a.AutorID).FirstOrDefault();

                                                if (crear)
                                                {
                                                    AutorPieza autorPieza = new AutorPieza()
                                                    {
                                                        PiezaID = piezaMaestra.PiezaID,
                                                        AutorID = AutorID,
                                                        Status = true
                                                    };

                                                    db.AutorPiezas.Add(autorPieza);

                                                }
                                            }
                                            else
                                            {
                                                if (tipoAtributo.EsLista)
                                                {
                                                    //buscar el valor en ListaValor en el tipo de Atributo
                                                    Int64 listaValorID = db.ListaValores.Where(a => a.AntID == valorCampoExtra && a.TipoAtributoID == tipoAtributo.TipoAtributoID).Select(a => a.ListaValorID).FirstOrDefault();
                                                    if (listaValorID != 0)
                                                        attPieza.ListaValorID = listaValorID;
                                                }
                                                else
                                                {
                                                    attPieza.Valor = valorCampoExtra;
                                                }

                                                if (crear)
                                                    db.AtributoPiezas.Add(attPieza);
                                                else
                                                    db.Entry(attPieza).State = EntityState.Modified;
                                            }
                                        }//fin del todo ok

                                    }//fin del tipoAtributo null
                                    else
                                    {
                                        ViewBag.mensajeError = "No existe id_pieza en la consulta";
                                    }

                                }//fin del foreach de listaCampos

                                db.SaveChanges();

                            }//fin foreach camposLista
                        }
                        else
                        {
                            ViewBag.mensajeError = "No existe la Pieza";
                        }
                    }
                    else
                    {
                        ViewBag.mensajeError = "No existe la Obra";
                    }

                    actual++;
                    con1.Close();

                }//fin while
            }
            catch (Exception)
            {
                ViewBag.error = "error";
                ViewBag.mensaje = "Conexión fallida";
            }


            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA DESCRIPTIVO 
        public ActionResult ImportarPieza_Descriptivo()
        {
            /*
             * 
             * AutorPieza       Autor_Clave
             * generico         titulo
             * generico_L       EscArtistica_Clave
             * generico_L       FormaAdquisicion_Clave
             * generico_L       Procedencia_Clave
             * generico_L       FiliacionEstilistica_Clave
             * generico_L       CasaComercial_Clave
             * generico_L       EdoConservacion_Clave
             * generico         descripcion
             * generico         grafica
             * generico         otros_materiales
             * generico         catalogo
             * generico         numero_catalogo
             * generico         palabra_clave
             * generico         palabra_clave
             * generico         numero_registro
             * generico         titulo_ori
             * generico         Procedencia
             * 
             * 
             * 
             * 
             * 
             * ------------ IMPLEMENTAR INDEPENDIENTES --------------
             * * ????????         cve_marco
             * 
             * ImagenPieza  	    m_pieza_foto
             * 
             * ColeccionGibranPieza     ClassColeccion_Clave
             * TecnicaMarco             MTecnicaMarco_Clave
             * Médidas	                m_pieza_dimensiones
             * 
             *              * CatalogoPieza	    m_cats
             * ExposicionPieza	    m_guion_det
             * TecnicaPieza         MatriculaTecnica_Clave
             * MatriculaPieza       Matricula_Clave

             */


            // Inicia el contador:
            Stopwatch tiempo = Stopwatch.StartNew();

            var tAtt_Autor = db.TipoAtributos.SingleOrDefault(a => a.AntNombre == "Autor_Clave").TipoAtributoID;
            //TipoAtributo tAtt_Autor = db.TipoAtributos.SingleOrDefault(a => a.AntNombre == "Autor_Clave");

            //TipoAtributo tAtt_Catalogo = db.TipoAtributos.Single(a => a.AntNombre == "Autor_Clave");
            //TipoAtributo tAtt_Exposicion = db.TipoAtributos.Single(a => a.AntNombre == "Autor_Clave");
            //TipoAtributo tAtt_Tecnica = db.TipoAtributos.Single(a => a.AntNombre == "Autor_Clave");
            //TipoAtributo tAtt_Matricula = db.TipoAtributos.Single(a => a.AntNombre == "Autor_Clave");

            //
            ViewBag.NombreTabla = "CATALOGO DE OBRAS - REGISTRO BASICO";

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                var total = 500;
                var inicio = 2520; //colocar AntID no el ObraID
                var fin = inicio + total;

                var limite = 103475;

                while (fin <= limite)
                {
                    con1.Open();
                    string textSql = string.Format("SELECT * FROM [m_pieza_descriptivo] WHERE [id_pieza] > {0} AND [id_pieza] <= {1}", inicio, fin);

                    SqlCommand sql = new SqlCommand(textSql, con1);
                    SqlDataReader leer = sql.ExecuteReader();

                    while (leer.Read())
                    {
                        //Buscar que la Obra Exista
                        string idPiezaText = leer["id_pieza"].ToString();
                        Obra obra = null;
                        var xObra = db.Obras.Where(a => a.AntID == idPiezaText).Select(a => new { a.ObraID, a.AntID, a.Clave });

                        if (xObra.Count() > 0)
                        {
                            obra = new Obra()
                            {
                                ObraID = xObra.FirstOrDefault().ObraID,
                                AntID = xObra.FirstOrDefault().AntID,
                                Clave = xObra.FirstOrDefault().Clave
                            };
                        }


                        if (obra != null)
                        {
                            //traer la PiezaMaestra
                            string claveText = obra.Clave + "-A";
                            Pieza piezaMaestra = null;
                            var xPiezaMaestra = db.Piezas.Where(a => a.Clave == claveText && a.ObraID == obra.ObraID).Select(a => new { a.ObraID, a.PiezaID, a.Clave, a.TipoPiezaID });
                            if (xPiezaMaestra.Count() > 0)
                            {
                                piezaMaestra = new Pieza()
                                {
                                    ObraID = xPiezaMaestra.FirstOrDefault().ObraID,
                                    PiezaID = xPiezaMaestra.FirstOrDefault().PiezaID,
                                    TipoPiezaID = xPiezaMaestra.FirstOrDefault().TipoPiezaID,
                                    Clave = xPiezaMaestra.FirstOrDefault().Clave
                                };
                            }

                            if (piezaMaestra != null)
                            {
                                // AUTOR_PIEZA ------------------------------------------------------------------------------------------------
                                // Autor_Clave ------------------------------------------------------------------------------------------------
                                string autorIDText = leer["Autor_Clave"].ToString();
                                Autor autor = null;
                                var xAutor = db.Autores.Where(a => a.AntID == autorIDText).Select(a => new { a.AutorID });
                                if (xAutor.Count() > 0)
                                {
                                    autor = new Autor()
                                    {
                                        AutorID = xAutor.FirstOrDefault().AutorID
                                    };
                                }

                                //si autor == null o "0" no hacer nada
                                if (autor != null)
                                {
                                    //Buscar que exista Atributo con TipoPieza y TipoAtributo
                                    Atributo att = null;
                                    var xAtt = db.Atributos.Where(a => a.TipoAtributoID == tAtt_Autor && a.TipoPiezaID == piezaMaestra.TipoPiezaID).Select(a => new { a.AtributoID });
                                    if (xAtt.Count() > 0)
                                    {
                                        att = new Atributo()
                                        {
                                            AtributoID = xAtt.FirstOrDefault().AtributoID
                                        };
                                    }
                                    else
                                    {
                                        //no existe entonces crearlo

                                        att.TipoPiezaID = piezaMaestra.TipoPiezaID;
                                        att.TipoAtributoID = tAtt_Autor;
                                        att.NombreAlterno = null;
                                        att.Orden = 100;
                                        att.Status = true;
                                        att.Requerido = true;
                                        att.EnFichaBasica = true;


                                        db.Atributos.Add(att);
                                        db.SaveChanges();

                                    }

                                    //comenzar creacion del AtributoPieza con valores null
                                    AtributoPieza attPiezaGenAutor = new AtributoPieza()
                                    {
                                        PiezaID = piezaMaestra.PiezaID,
                                        AtributoID = att.AtributoID,
                                    };
                                    db.AtributoPiezas.Add(attPiezaGenAutor);


                                    AutorPieza autorPieza = new AutorPieza()
                                    {
                                        PiezaID = piezaMaestra.PiezaID,
                                        AutorID = autor.AutorID,
                                        Status = true
                                    };
                                    db.AutorPiezas.Add(autorPieza);
                                }

                                // GENERICOS --------------------------------------------------------------------------------------------------
                                List<string> campos = new List<string>()
                                { 
                                    "titulo", 
                                    "EscArtistica_Clave",
                                    "FormaAdquisicion_Clave",
                                    "Procedencia_Clave",
                                    "FiliacionEstilistica_Clave",
                                    "CasaComercial_Clave",
                                    "EdoConservacion_Clave",
                                    "descripcion",
                                    "otros_materiales",
                                    "catalogo",
                                    "numero_catalogo",
                                    "numero_registro",
                                    "titulo_ori",
                                    "Procedencia"
                                };

                                foreach (var campoAnt in campos)
                                {
                                    //buscar el tipo de atributo
                                    TipoAtributo tipoAttGen = null;
                                    var xTipoAttGen = db.TipoAtributos.Where(a => a.AntNombre == campoAnt).Select(a => new { a.EsLista, a.TipoAtributoID });
                                    if (xTipoAttGen.Count() > 0)
                                    {
                                        tipoAttGen = new TipoAtributo()
                                        {
                                            EsLista = xTipoAttGen.FirstOrDefault().EsLista,
                                            TipoAtributoID = xTipoAttGen.FirstOrDefault().TipoAtributoID
                                        };
                                    }


                                    if (tipoAttGen != null)
                                    {
                                        //Extraer el valor a buscar o guardar de la base vieja
                                        string valorCampo = leer[campoAnt].ToString();
                                        //Validar el valor del campo
                                        bool todoOk = true;
                                        if (tipoAttGen.EsLista)
                                        {
                                            if (valorCampo == "0")
                                            {
                                                todoOk = false;
                                            }
                                        }
                                        else
                                        {
                                            if (valorCampo == "0" || valorCampo == "" || valorCampo == " " || valorCampo == "-" || valorCampo == " -" || valorCampo == "Pendiente por definir")
                                            {
                                                todoOk = false;
                                            }
                                        }

                                        //paso la validacion
                                        if (todoOk)
                                        {
                                            //Buscar que el Atributo Exista con TipoPieza y TipoAtributo
                                            Atributo attGen = null;
                                            var xAttGen = db.Atributos.Where(a => a.TipoAtributoID == tipoAttGen.TipoAtributoID && a.TipoPiezaID == piezaMaestra.TipoPiezaID).Select(a => new { a.AtributoID });
                                            if (xAttGen.Count() > 0)
                                            {
                                                attGen = new Atributo()
                                                {
                                                    AtributoID = xAttGen.FirstOrDefault().AtributoID
                                                };
                                            }
                                            else
                                            {
                                                //si no existe entonces crearlo
                                                attGen = new Atributo()
                                                {
                                                    TipoPiezaID = piezaMaestra.TipoPiezaID,
                                                    TipoAtributoID = tipoAttGen.TipoAtributoID,
                                                    Orden = 100,
                                                    Status = true,
                                                    Requerido = false,
                                                    EnFichaBasica = false
                                                };

                                                db.Atributos.Add(attGen);
                                                db.SaveChanges();
                                            }

                                            //comenzar creacion del AtributoPieza con el valor extraido y validado
                                            AtributoPieza attPiezaGen = new AtributoPieza()
                                            {
                                                PiezaID = piezaMaestra.PiezaID,
                                                AtributoID = attGen.AtributoID,
                                            };

                                            if (tipoAttGen.EsLista)
                                            {
                                                //buscar el valor en ListaValor en el tipo de atributo
                                                ListaValor listaValorGen = null;
                                                var xListaValorGen = db.ListaValores.Where(a => a.AntID == valorCampo && a.TipoAtributoID == tipoAttGen.TipoAtributoID).Select(a => new { a.ListaValorID });
                                                if (xListaValorGen.Count() > 0)
                                                {
                                                    listaValorGen = new ListaValor()
                                                    {
                                                        ListaValorID = xListaValorGen.FirstOrDefault().ListaValorID
                                                    };

                                                }

                                                if (listaValorGen != null)
                                                {
                                                    attPiezaGen.ListaValorID = listaValorGen.ListaValorID;
                                                    db.AtributoPiezas.Add(attPiezaGen);
                                                    db.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                attPiezaGen.Valor = valorCampo;
                                                db.AtributoPiezas.Add(attPiezaGen);
                                                db.SaveChanges();
                                            }
                                        }

                                    }
                                    else
                                    {

                                        ViewBag.error = "error";
                                        ViewBag.mensaje = "No existe la Tipo de Atributo, intenta resolver el problema antes de continuar";
                                    }
                                }
                                // ------------------------------------------------------------------------------------------------------------
                                // ------------------------------------------------------------------------------------------------------------
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.error = "error";
                                ViewBag.mensaje = "No existe la PiezaMaestra, intenta resolver el problema antes de continuar";
                            }
                        }
                        else
                        {
                            ViewBag.error = "error";
                            ViewBag.mensaje = "No existe la Obra, intenta resolver el problema antes de continuar";
                        }

                    }

                    inicio = fin;
                    fin = fin + total;
                    con1.Close();
                }

                // Para el contador e imprime el resultado:
                ViewBag.TiempoTotal = tiempo.Elapsed.TotalSeconds.ToString();

                var lista = db.Piezas.ToList();
                ViewBag.TotalRegistros = lista.Count;

                return PartialView("_ImportarPieza_Descriptivo", lista);
            }
            catch (Exception)
            {
                ViewBag.error = "error";
                ViewBag.mensaje = "Conexión fallida";
            }

            ViewBag.TiempoTotal = tiempo.Elapsed.TotalSeconds.ToString();
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //PRUEBA MOSTRAR OBRAS
        //IMPORTAR REGISTROS DE OBRAS COMUNES
        public ActionResult ImportarObras_Maestras()
        {
            ViewBag.NombreTabla = "PRUEBA DE OBRAS - MOSTRAR";

            ViewBag.error = "";
            try
            {
                //abrir conexion
                con1.Open();

                // mandar mensaje de conexcion
                ViewBag.mensaje = "Conexión establecida";

                //definir el sql
                string textSql = string.Format("SELECT TOP 10 * FROM [m_pieza] ORDER BY [TipoObjeto_Clave]");
                SqlCommand sql = new SqlCommand(textSql, con1);
                //ejecutar el sql
                SqlDataReader leer = sql.ExecuteReader();
                //realizar el foreach

                List<m_pieza> lista = new List<m_pieza>();

                while (leer.Read())
                {
                    //definir el tipo de tabla
                    m_pieza mpieza = new m_pieza();

                    //llenar el registro con los valores viejos
                    //obra
                    mpieza.Matricula_Clave = Convert.ToInt32(leer["Matricula_Clave"]);
                    mpieza.id_pieza = Convert.ToDecimal(leer["id_pieza"]);
                    mpieza.TipoAdquisicion_Clave = leer["TipoAdquisicion_Clave"].ToString();
                    mpieza.MatriculaTecnica_Clave = Convert.ToInt32(leer["MatriculaTecnica_Clave"]);
                    mpieza.cve_usuario = Convert.ToInt32(leer["cve_usuario"]);
                    mpieza.Ubicacion_Clave = Convert.ToInt32(leer["Ubicacion_Clave"]);
                    mpieza.UbicacionActual = leer["UbicacionActual"].ToString();
                    mpieza.Propietario_Clave = Convert.ToInt32(leer["Propietario_Clave"]);
                    mpieza.TipoObjeto_Clave = Convert.ToInt32(leer["TipoObjeto_Clave"]);
                    mpieza.fecha_registro_ORI = leer["fecha_registro_ORI"].ToString();
                    mpieza.estatus = leer["estatus"].ToString();
                    mpieza.otr_ubic = leer["otr_ubic"].ToString();
                    mpieza.fecha_ingreso = Convert.ToDateTime(leer["fecha_ingreso"]);
                    mpieza.fecha_registro = leer["fecha_registro"].ToString();
                    mpieza.pieza_treg = Convert.ToDecimal(leer["pieza_treg"]);
                    mpieza.tmp = leer["tmp"].ToString();
                    mpieza.usu_id_cap = Convert.ToDecimal(leer["usu_id_cap"]);
                    mpieza.baja = leer["baja"].ToString();
                    mpieza.cInventario = leer["cInventario"].ToString();
                    mpieza.ClassColeccion_Clave = leer["ClassColeccion_Clave"].ToString();
                    mpieza.cInvestigacion = leer["cInvestigacion"].ToString();

                    lista.Add(mpieza);

                }

                con1.Close();

                // * *   D E S C R I P T I V O   * *
                foreach (var mpieza in lista)
                {
                    con1.Open();
                    //definir el sql
                    textSql = string.Format("SELECT TOP 1 * FROM [m_pieza_descriptivo] WHERE [id_pieza] = {0}", mpieza.id_pieza);
                    sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    leer = sql.ExecuteReader();
                    //realizar el foreach

                    leer.Read();

                    mpieza.Autor_Clave = Convert.ToInt32(leer["Autor_Clave"]);
                    mpieza.titulo = leer["titulo"].ToString();
                    mpieza.EscArtistica_Clave = Convert.ToInt32(leer["EscArtistica_Clave"]);
                    mpieza.FormaAdquisicion_Clave = Convert.ToInt32(leer["FormaAdquisicion_Clave"]);
                    mpieza.Procedencia_Clave = Convert.ToInt32(leer["Procedencia_Clave"]);
                    mpieza.FiliacionEstilistica_Clave = Convert.ToInt32(leer["FiliacionEstilistica_Clave"]);
                    mpieza.CasaComercial_Clave = Convert.ToInt32(leer["CasaComercial_Clave"]);
                    mpieza.EdoConservacion_Clave = Convert.ToInt32(leer["EdoConservacion_Clave"]);
                    mpieza.descripcion = leer["descripcion"].ToString();
                    mpieza.grafica = leer["grafica"].ToString();
                    mpieza.otros_materiales = leer["otros_materiales"].ToString();
                    mpieza.catalogo = leer["catalogo"].ToString();
                    mpieza.numero_catalogo = leer["numero_catalogo"].ToString();
                    mpieza.palabra_clave = leer["palabra_clave"].ToString();
                    mpieza.cve_marco = Convert.ToInt32(leer["cve_marco"]);
                    mpieza.numero_registro = leer["numero_registro"].ToString();
                    mpieza.titulo_ori = leer["titulo_ori"].ToString();
                    mpieza.Procedencia = leer["Procedencia"].ToString();

                    con1.Close();

                }

                con1.Close();

                // * *   C A R G A R     O B R A S     C O M U N   * *
                foreach (var mpieza in lista)
                {
                    con1.Open();
                    //definir el sql
                    textSql = string.Format("SELECT TOP 1 * FROM [m_otras_piezas] WHERE [id_pieza] = {0}", mpieza.id_pieza);
                    sql = new SqlCommand(textSql, con1);
                    //ejecutar el sql
                    leer = sql.ExecuteReader();
                    //realizar el foreach

                    leer.Read();

                    mpieza.Autor_Clave = Convert.ToInt32(leer["Autor_Clave"]);
                    mpieza.titulo = leer["titulo"].ToString();
                    mpieza.EscArtistica_Clave = Convert.ToInt32(leer["EscArtistica_Clave"]);
                    mpieza.FormaAdquisicion_Clave = Convert.ToInt32(leer["FormaAdquisicion_Clave"]);
                    mpieza.Procedencia_Clave = Convert.ToInt32(leer["Procedencia_Clave"]);
                    mpieza.FiliacionEstilistica_Clave = Convert.ToInt32(leer["FiliacionEstilistica_Clave"]);
                    mpieza.CasaComercial_Clave = Convert.ToInt32(leer["CasaComercial_Clave"]);
                    mpieza.EdoConservacion_Clave = Convert.ToInt32(leer["EdoConservacion_Clave"]);
                    mpieza.descripcion = leer["descripcion"].ToString();
                    mpieza.grafica = leer["grafica"].ToString();
                    mpieza.otros_materiales = leer["otros_materiales"].ToString();
                    mpieza.catalogo = leer["catalogo"].ToString();
                    mpieza.numero_catalogo = leer["numero_catalogo"].ToString();
                    mpieza.palabra_clave = leer["palabra_clave"].ToString();
                    mpieza.cve_marco = Convert.ToInt32(leer["cve_marco"]);
                    mpieza.numero_registro = leer["numero_registro"].ToString();
                    mpieza.titulo_ori = leer["titulo_ori"].ToString();
                    mpieza.Procedencia = leer["Procedencia"].ToString();

                    con1.Close();

                }


                ViewBag.TotalRegistros = lista.Count;

                return PartialView("_ImportarObras_Maestras", lista);


            }
            catch (Exception)
            {
                ViewBag.mensaje = "Conexión fallida";
            }
            return PartialView("_ImportarObras_Maestras");
        }


        //IMPORTAR PIEZA - CATALOGO
        public ActionResult ImportarPieza_Catalogo()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA AUTOR";
            var tipoAtt_CatalogoID = dbx.TipoAtributos.Where(a => a.AntNombre == "m_cats").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaCatalogos = dbx.Catalogos.Select(a => new { a.CatalogoID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "Catalogo_Clave";
                string tabla = "m_cats";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    listaAnt.Add(new AnonimoPiezaTabla()
                    {
                        id_pieza = id,
                        Clave = clave
                    });

                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar al Autor
                                var catalogo = listaCatalogos.FirstOrDefault(a => a.AntID == apt.Clave);

                                CatalogoPieza catalogoPieza = new CatalogoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    CatalogoID = catalogo.CatalogoID,
                                    Status = true
                                };

                                //buscar que no exista el catalogo
                                if (dbx.CatalogoPiezas.Where(a => a.CatalogoID == catalogoPieza.CatalogoID && a.PiezaID == catalogoPieza.PiezaID).Count() == 0)
                                {
                                    dbx.CatalogoPiezas.Add(catalogoPieza);
                                    dbx.SaveChanges();
                                }

                            }
                        }


                    }

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - EXPOSICION
        public ActionResult ImportarPieza_Exposicion()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA EXPOSICION";
            var tipoAtt_ExposicionID = dbx.TipoAtributos.Where(a => a.AntNombre == "m_guion_det").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaExposiciones = dbx.Exposiciones.Select(a => new { a.ExposicionID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "gui_id";
                string tabla = "m_guion_det";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    listaAnt.Add(new AnonimoPiezaTabla()
                    {
                        id_pieza = id,
                        Clave = clave
                    });

                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar al Autor
                                var exposicion = listaExposiciones.FirstOrDefault(a => a.AntID == apt.Clave);

                                ExposicionPieza expoPieza = new ExposicionPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    ExposicionID = exposicion.ExposicionID,
                                    Status = true
                                };

                                //buscar que no exista el catalogo
                                if (dbx.ExposicionPiezas.Where(a => a.ExposicionID == expoPieza.ExposicionID && a.PiezaID == expoPieza.PiezaID).Count() == 0)
                                {
                                    dbx.ExposicionPiezas.Add(expoPieza);
                                }

                            }
                        }

                        dbx.SaveChanges();
                    }

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - MATRICULA
        public ActionResult ImportarPieza_Matricula()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA MATRICULA";
            var tipoAtt_MatriculaID = dbx.TipoAtributos.Where(a => a.AntNombre == "Matricula_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaMatriculas = dbx.Matriculas.Select(a => new { a.MatriculaID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "Matricula_Clave";
                string tabla = "m_pieza";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    listaAnt.Add(new AnonimoPiezaTabla()
                    {
                        id_pieza = id,
                        Clave = clave
                    });

                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar al Autor
                                var matricula = listaMatriculas.FirstOrDefault(a => a.AntID == apt.Clave);

                                MatriculaPieza matPieza = new MatriculaPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    MatriculaID = matricula.MatriculaID,
                                    Status = true
                                };


                                dbx.MatriculaPiezas.Add(matPieza);

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - TECNICA
        public ActionResult ImportarPieza_Tecnica()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TECNICA";
            var tipoAtt_MatriculaID = dbx.TipoAtributos.Where(a => a.AntNombre == "Matricula_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaTecnicas = dbx.Tecnicas.Select(a => new { a.TecnicaID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "MatriculaTecnica_Clave";
                string tabla = "m_pieza";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    listaAnt.Add(new AnonimoPiezaTabla()
                    {
                        id_pieza = id,
                        Clave = clave
                    });

                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar al Autor
                                var tecnica = listaTecnicas.FirstOrDefault(a => a.AntID == apt.Clave);

                                TecnicaPieza tecPieza = new TecnicaPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    TecnicaID = tecnica.TecnicaID,
                                    Status = true
                                };


                                dbx.TecnicaPiezas.Add(tecPieza);

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - TECNICA MARCO
        public ActionResult ImportarPieza_TecnicaMarco()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TECNICA MARCO";
            var tipoAtt_MatriculaID = dbx.TipoAtributos.Where(a => a.AntNombre == "MTecnicaMarco_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaTecnicaMarcos = dbx.TecnicaMarcos.Select(a => new { a.TecnicaMarcoID, a.AntID }).ToList();
                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();


                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "MTecnicaMarco_Clave";
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    if (clave != "0")
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza Autor
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar al Autor
                                var tecnica = listaTecnicaMarcos.FirstOrDefault(a => a.AntID == apt.Clave);

                                TecnicaMarcoPieza tecMarcoPieza = new TecnicaMarcoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    TecnicaMarcoID = tecnica.TecnicaMarcoID,
                                    Status = true
                                };


                                dbx.TecnicaMarcoPiezas.Add(tecMarcoPieza);

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }

        // -----------------------------------------------------------------------

        //IMPORTAR PIEZA - TITULO
        public ActionResult ImportarPieza_Titulo()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TITULO";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "titulo").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 29100; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "titulo";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (!string.IsNullOrWhiteSpace(clave) || clave != "Sin título")
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }
                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - DESCRIPCION
        public ActionResult ImportarPieza_Descripcion()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA DESCRIPCION";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "descripcion").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "descripcion";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - OTROS MATERIALES
        public ActionResult ImportarPieza_Materiales()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA MATERIALES";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "otros_materiales").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "otros_materiales";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - CATALOGO TEXT
        public ActionResult ImportarPieza_CatalogoText()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA CATALOGO TEXT";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "catalogo").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "catalogo";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - NUMERO DE CATALOGO
        public ActionResult ImportarPieza_NumeroCatalogo()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA NUMERO DE CATALOGO";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "numero_catalogo").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "numero_catalogo";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - NUMERO DE REGISTRO
        public ActionResult ImportarPieza_NumeroRegistro()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA NUMERO DE REGISTRO";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "numero_registro").Select(a => a.TipoAtributoID).FirstOrDefault();


            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "numero_registro";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - TITULO ORIGINAL
        public ActionResult ImportarPieza_TituloOriginal()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TITULO ORIGINAL";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "titulo_ori").Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "titulo_ori";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - PROCEDENCIA TEXT
        public ActionResult ImportarPieza_ProcedenciaText()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TITULO ORIGINAL";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "Procedencia").Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "Procedencia";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - ESCUELA ARTISTICA
        public ActionResult ImportarPieza_EscuelaArtistica()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA TITULO ORIGINAL";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == "EscArtistica_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "EscArtistica_Clave";
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - FORMA DE ADQUISICION
        public ActionResult ImportarPieza_FormaAdquisicion()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA FORMA DE ADQUISICION";
            string nombreCampoAnt = "FormaAdquisicion_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - PROCEDENCIA
        public ActionResult ImportarPieza_Procedencia()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA PROCEDENCIA";
            string nombreCampoAnt = "Procedencia_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - FILIACION ESTILISTICA
        public ActionResult ImportarPieza_FiliacionEstilistica()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA FILIACION ESTILISTICA";
            string nombreCampoAnt = "FiliacionEstilistica_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - CASA COMERCIAL
        public ActionResult ImportarPieza_CasaComercial()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA CASA COMERCIAL";
            string nombreCampoAnt = "CasaComercial_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - ESTADO DE CONSERVACION
        public ActionResult ImportarPieza_EdoConservacion()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA ESTADO DE CONSERVACION";
            string nombreCampoAnt = "EdoConservacion_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_descriptivo";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - FECHA DE EJECUCION
        public ActionResult ImportarPieza_FechaEjecucion()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA FECHA DE EJECUCION";
            string nombreCampoAnt = "FechaEjecucion_Clave";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                var listaValores = dbx.ListaValores.Where(a => a.TipoAtributoID == tipoAttID).Select(a => new { a.ListaValorID, a.AntID }).ToList();

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //buscar el valor

                                var listaValor = listaValores.FirstOrDefault(a => a.AntID == apt.Clave);


                                if (listaValor != null)
                                {
                                    //crear AtributoPieza
                                    AtributoPieza attPieza = new AtributoPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        AtributoID = attID,
                                        ListaValorID = listaValor.ListaValorID

                                    };

                                    dbx.AtributoPiezas.Add(attPieza);
                                }

                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - FECHA DE EJECUCION TEXT
        public ActionResult ImportarPieza_FechaEjecucionText()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA NUMERO DE CATALOGO";
            string nombreCampoAnt = "fecha_ejecucion";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - OTROS TITULOS
        public ActionResult ImportarPieza_otros_titulos()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA OTROS TITULOS";
            string nombreCampoAnt = "otros_titulos";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - MARCAS E INSCRIPCIONES
        public ActionResult ImportarPieza_marcas_inscripciones()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA MARCAS E INSCRIPCIONES";
            string nombreCampoAnt = "marcas_inscripciones";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - MARCAS
        public ActionResult ImportarPieza_marcas()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA MARCAS";
            string nombreCampoAnt = "marcas";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" ||
                        clave == "-" ||
                        clave == " -" ||
                        clave == "-" ||
                        clave == "." ||
                        clave == "" ||
                        clave == " " ||
                        clave == "-No tiene" ||
                        clave == "No tiene" ||
                        clave == "No tiene." ||
                        clave == "Ninguna." ||
                        clave == "-Ninguna" ||
                        clave == "Ninguna" ||
                        clave == "no tiene" ||
                        clave == "- No presenta" ||
                        clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - OBSERVACIONES
        public ActionResult ImportarPieza_Observaciones()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA OBSERVACIONES";
            string nombreCampoAnt = "observaciones";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" ||
                        clave == "-" ||
                        clave == " -" ||
                        clave == "-" ||
                        clave == "." ||
                        clave == "" ||
                        clave == " " ||
                        clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BIBLIOGRAFIA
        public ActionResult ImportarPieza_bibliografia()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BIBLIOGRAFIA";
            string nombreCampoAnt = "bibliografia";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" ||
                        clave == "-" ||
                        clave == " -" ||
                        clave == "-" ||
                        clave == "." ||
                        clave == "" ||
                        clave == " " ||
                        clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - ASESORES
        public ActionResult ImportarPieza_asesores()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA ASESORES";
            string nombreCampoAnt = "asesores";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_pieza_obra_comun";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }

        // -----------------------------------------------------------------------

        //IMPORTAR PIEZA - BILLETE - INICIALES
        public ActionResult ImportarPieza_bi_inic()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE INICIALES";
            string nombreCampoAnt = "bi_inic";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - NOMBRE BANCO
        public ActionResult ImportarPieza_bi_para()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE NOMBRE BANCO";
            string nombreCampoAnt = "bi_para";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - RELACION GEOGRAFICA
        public ActionResult ImportarPieza_bi_geo()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE RELACION GEOGRAFICA";
            string nombreCampoAnt = "bi_geo";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - DENOMINACION
        public ActionResult ImportarPieza_bi_tipodocto()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE DENOMINACION";
            string nombreCampoAnt = "bi_tipodocto";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - FECHAS AGREGADAS
        public ActionResult ImportarPieza_bi_fechadd()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE FECHAS AGREGADAS";
            string nombreCampoAnt = "bi_fechadd";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - FECHAS APROXIMADAS
        public ActionResult ImportarPieza_bi_fechaprox()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE FECHAS APROXIMADAS";
            string nombreCampoAnt = "bi_fechaprox";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - ANVERSO INSCRIPCIONES
        public ActionResult ImportarPieza_bi_inscanv()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE FECHAS APROXIMADAS";
            string nombreCampoAnt = "Mon_LAnv,bi_inscanv";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "bi_inscanv";
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - REVERSO INSCRIPCIONES
        public ActionResult ImportarPieza_bi_inscrev()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE REVERSO INSCRIPCIONES";
            string nombreCampoAnt = "Mon_LRev,bi_inscrev";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "bi_inscrev";
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //IMPORTAR PIEZA - BILLETE - OTROS AUTORES
        public ActionResult ImportarPieza_bi_otrosaut()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA BILLETE OTROS AUTORES";
            string nombreCampoAnt = "bi_otrosaut";
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreCampoAnt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampoAnt;
                string tabla = "m_billetes";
                string textSql1 = string.Format("SELECT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir")
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //-----------------------------------------------------------------
        //IMAGENES QUEDA PENDIENTE
        //IMPORTAR PIEZA - IMAGENES
        public ActionResult ImportarPieza_Imagenes()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA IMAGENES";

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 500;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar, se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoImagenPiezaTabla> listaAnt = new List<AnonimoImagenPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = "Consec";
                string campo2 = "nombre";
                string campo3 = "ruta_imagen";

                string tabla = "m_pieza_foto";
                string textSql1 = string.Format("SELECT [{0}], [{1}],[{2}], [{3}] FROM [{4}] ORDER BY [{5}]", campo0, campo1, campo2, campo3, tabla, campo0);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id_pieza = leer1[campo0].ToString();
                    string consec = leer1[campo1].ToString();
                    string nombre = leer1[campo2].ToString();
                    string ruta_imagen = leer1[campo3].ToString();

                    //ingreso la validacion del campo clave

                    listaAnt.Add(new AnonimoImagenPiezaTabla()
                    {
                        id_pieza = id_pieza,
                        Consec = consec,
                        nombre = nombre,
                        ruta_imagen = ruta_imagen
                    });

                }

                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        var anonimaImagen = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaImagen.Count > 0)
                        {
                            foreach (var anImg in anonimaImagen)
                            {
                                ImagenPieza imgPieza = new ImagenPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    Orden = Convert.ToInt32(anImg.Consec),
                                    Titulo = anImg.nombre,
                                    ImgNombre = anImg.ruta_imagen,
                                    Status = true,
                                };

                                dbx.ImagenPiezas.Add(imgPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }

        //IMPORTAR PIEZA - MONEDA
        public ActionResult ImportarPieza_Monedas(string nombreCampo, string nombreAtt)
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "PIEZA MONEDA " + nombreCampo;
            var tipoAttID = dbx.TipoAtributos.Where(a => a.AntNombre == nombreAtt).Select(a => a.TipoAtributoID).FirstOrDefault();

            ViewBag.error = "";

            try
            {
                // mandar mensaje de conexion
                ViewBag.mensaje = "Conexión establecida";
                //definir el sql
                Int64 limite = 100000;
                Int64 total = 100;

                Int64 inicio = 0; //colocar el PIEZAID del cual comenzar se comienza desde actual+1
                Int64 fin = inicio + total;

                List<AnonimoPiezaTabla> listaAnt = new List<AnonimoPiezaTabla>();
                con1.Open();
                string campo0 = "id_pieza";
                string campo1 = nombreCampo;
                string tabla = "m_monedas";
                string textSql1 = string.Format("SELECT DISTINCT [{0}], [{1}] FROM [{2}]", campo0, campo1, tabla);
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                while (leer1.Read())
                {
                    string id = leer1[campo0].ToString();
                    string clave = leer1[campo1].ToString();
                    //ingreso la validacion del campo clave

                    if (clave == "0" || clave == "" || clave == " " || clave == "-" || clave == " -" || clave == "." || clave == " ." || clave == "Pendiente por definir" || string.IsNullOrWhiteSpace(clave))
                    {

                    }
                    else
                    {
                        listaAnt.Add(new AnonimoPiezaTabla()
                        {
                            id_pieza = id,
                            Clave = clave
                        });
                    }


                }
                con1.Close();

                while (fin <= limite)
                {
                    //tener la lista de pieza
                    var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                    foreach (var pieza in listPiezas)
                    {
                        //registrar una Pieza 
                        var anonimaPiezaTabla = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                        if (anonimaPiezaTabla.Count > 0)
                        {
                            foreach (var apt in anonimaPiezaTabla)
                            {
                                //buscar el Atributo en TipoPieza
                                Int64 attID = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAttID && a.TipoPiezaID == pieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();



                                if (attID == 0)
                                {
                                    //crear el Atributo para ese TipoPieza
                                    Atributo att = new Atributo()
                                    {
                                        TipoAtributoID = tipoAttID,
                                        TipoPiezaID = pieza.TipoPiezaID,
                                        Status = true,
                                        EnFichaBasica = true,
                                        Orden = 1,
                                        Requerido = true
                                    };
                                    dbx.Atributos.Add(att);
                                    dbx.SaveChanges();
                                    attID = att.AtributoID;
                                }

                                //crear AtributoPieza

                                AtributoPieza attPieza = new AtributoPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    AtributoID = attID,
                                    Valor = apt.Clave
                                };

                                dbx.AtributoPiezas.Add(attPieza);
                            }
                        }

                    }

                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

                    inicio = fin;
                    fin = fin + total;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }


        //--------------------------------------------------------------
        //MEDIDAS - TIPO DE MEDIDAS
        public ActionResult ImportarMedida_TipoMedida()
        {
            ViewBag.NombreTabla = "MEDIDAS TIPO DE MEDIDAS";

            ViewBag.error = "";

            List<string> listaTipoMed = new List<string>() 
            {
                "Sin marco ó base",
                "Museográficas",
                "Marco"
            };

            try
            {

                foreach (var item in listaTipoMed)
                {
                    Int64 tipoMed = db.TipoMedidas.Where(a => a.Nombre == item).Select(a => a.TipoMedidaID).FirstOrDefault();

                    if (tipoMed == 0)
                    {

                        TipoMedida tm = new TipoMedida()
                        {
                            Nombre = item,
                            Status = true
                        };

                        db.TipoMedidas.Add(tm);
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ImportarPieza_Descriptivo");
        }

        //MEDIDAS - SIN MARCO O BASE
        public ActionResult ImportarMedida_SinMarcoBase()
        {

            /*
                "Sin marco ó base"
                "Museográficas",
                "Marco"
            */

            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "MEDIDA SIN MARCO O BASE";

            string nombreMedida = "Sin marco ó base";
            Int64 tipoMed = db.TipoMedidas.Where(a => a.Nombre == nombreMedida).Select(a => a.TipoMedidaID).FirstOrDefault();

            ViewBag.error = "";
            if (tipoMed != 0)
            {
                try
                {
                    // mandar mensaje de conexion
                    ViewBag.mensaje = "Conexión establecida";
                    //definir el sql
                    Int64 limite = 100000;
                    Int64 total = 500;

                    Int64 inicio = 0; //colocar el PIEZAID del cual comenzar, se comienza desde actual+1
                    Int64 fin = inicio + total;

                    List<AnonimoMedida> listaAnt = new List<AnonimoMedida>();
                    con1.Open();
                    string tabla = "m_pieza_dimensiones";
                    string textSql1 = string.Format("SELECT * FROM [{0}]", tabla);
                    SqlCommand sql1 = new SqlCommand(textSql1, con1);
                    SqlDataReader leer1 = sql1.ExecuteReader();

                    while (leer1.Read())
                    {
                        string id_pieza = leer1["id_pieza"].ToString();
                        string UMPeso_Clave = leer1["UMPeso_Clave"].ToString();
                        double? alto = Convert.ToDouble(leer1["alto"].ToString());
                        string UMLongitud_Clave = leer1["UMLongitud_Clave"].ToString();
                        double? ancho = Convert.ToDouble(leer1["ancho"].ToString());
                        double? profundo = Convert.ToDouble(leer1["profundo"].ToString());
                        double? diametro = Convert.ToDouble(leer1["diametro"].ToString());
                        double? diametro2 = Convert.ToDouble(leer1["diametro2"].ToString());
                        double? peso = Convert.ToDouble(leer1["peso"].ToString());
                        string cve_tipo_medida = leer1["cve_tipo_medida"].ToString();
                        string otr_med = leer1["otr_med"].ToString();


                        if (alto == 0.0) alto = null;
                        if (ancho == 0.0) ancho = null;
                        if (profundo == 0.0) profundo = null;
                        if (diametro == 0.0) diametro = null;
                        if (diametro2 == 0.0) diametro2 = null;
                        if (peso == 0.0) peso = null;



                        //ingreso la validacion del campo clave
                        AnonimoMedida am = new AnonimoMedida()
                        {
                            id_pieza = id_pieza,
                            UMPeso_Clave = UMPeso_Clave,
                            alto = alto,
                            UMLongitud_Clave = UMLongitud_Clave,
                            ancho = ancho,
                            profundo = profundo,
                            diametro = diametro,
                            diametro2 = diametro2,
                            peso = peso,
                            cve_tipo_medida = cve_tipo_medida

                        };

                        if (string.IsNullOrWhiteSpace(otr_med)) otr_med = null;
                        else am.otr_med = otr_med;



                        listaAnt.Add(am);

                    }

                    con1.Close();

                    while (fin <= limite)
                    {
                        //tener la lista de pieza
                        var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                        foreach (var pieza in listPiezas)
                        {
                            var anonimaMed = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                            if (anonimaMed.Count > 0)
                            {
                                foreach (var anMed in anonimaMed)
                                {
                                    Medida med = new Medida()
                                    {
                                        Ancho = anMed.ancho,
                                        Diametro = anMed.diametro,
                                        Largo = anMed.alto,
                                        Peso = anMed.peso,
                                        PiezaID = pieza.PiezaID,
                                        Profundidad = anMed.profundo,
                                        Status = true,
                                        TipoMedidaID = tipoMed,
                                        Otra = anMed.otr_med
                                    };

                                    switch (anMed.UMLongitud_Clave)
                                    {
                                        case "21001":
                                            med.UMLongitud = UMLongitud.cm;
                                            break;
                                        case "21002":
                                            med.UMLongitud = UMLongitud.mm;
                                            break;
                                        case "21003":
                                            med.UMLongitud = UMLongitud.pulgadas;
                                            break;
                                        case "21004":
                                            med.UMLongitud = UMLongitud.m;
                                            break;
                                    }

                                    switch (anMed.UMPeso_Clave)
                                    {
                                        case "22002":
                                            med.UMMasa = UMMasa.kg;
                                            break;
                                        case "22003":
                                            med.UMMasa = UMMasa.gr;
                                            break;
                                    }



                                    dbx.Medidas.Add(med);
                                }
                            }

                        }

                        dbx.SaveChanges();

                        dbx.Dispose();
                        dbx = new RecordFCSContext();
                        dbx.Configuration.AutoDetectChangesEnabled = false;

                        inicio = fin;
                        fin = fin + total;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return PartialView("_ImportarPieza_Descriptivo");
        }


        //MEDIDAS - MUSEOGRAFICAS
        public ActionResult ImportarMedida_Museograficas()
        {

            /*
                "Sin marco ó base"
                "Museográficas",
                "Marco"
            */

            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "MEDIDA SIN MARCO O BASE";

            string nombreMedida = "Museográficas";
            Int64 tipoMed = db.TipoMedidas.Where(a => a.Nombre == nombreMedida).Select(a => a.TipoMedidaID).FirstOrDefault();

            ViewBag.error = "";
            if (tipoMed != 0)
            {
                try
                {
                    // mandar mensaje de conexion
                    ViewBag.mensaje = "Conexión establecida";
                    //definir el sql
                    Int64 limite = 100000;
                    Int64 total = 500;

                    Int64 inicio = 0; //colocar el PIEZAID del cual comenzar, se comienza desde actual+1
                    Int64 fin = inicio + total;

                    List<AnonimoMedida> listaAnt = new List<AnonimoMedida>();
                    con1.Open();
                    string tabla = "m_pieza_MedidasMuseograficas";
                    string textSql1 = string.Format("SELECT * FROM [{0}]", tabla);
                    SqlCommand sql1 = new SqlCommand(textSql1, con1);
                    SqlDataReader leer1 = sql1.ExecuteReader();

                    while (leer1.Read())
                    {
                        string id_pieza = leer1["id_pieza"].ToString();
                        string UMPeso_Clave = leer1["UMPeso_Clave"].ToString();
                        double? alto = Convert.ToDouble(leer1["alto"].ToString());
                        string UMLongitud_Clave = leer1["UMLongitud_Clave"].ToString();
                        double? ancho = Convert.ToDouble(leer1["ancho"].ToString());
                        double? profundo = Convert.ToDouble(leer1["profundo"].ToString());
                        double? diametro = Convert.ToDouble(leer1["diametro"].ToString());
                        double? diametro2 = Convert.ToDouble(leer1["diametro2"].ToString());
                        double? peso = Convert.ToDouble(leer1["peso"].ToString());
                        string cve_tipo_medida = leer1["cve_tipo_medida"].ToString();
                        string otr_med = leer1["otr_med"].ToString();


                        if (alto == 0.0) alto = null;
                        if (ancho == 0.0) ancho = null;
                        if (profundo == 0.0) profundo = null;
                        if (diametro == 0.0) diametro = null;
                        if (diametro2 == 0.0) diametro2 = null;
                        if (peso == 0.0) peso = null;



                        //ingreso la validacion del campo clave
                        AnonimoMedida am = new AnonimoMedida()
                        {
                            id_pieza = id_pieza,
                            UMPeso_Clave = UMPeso_Clave,
                            alto = alto,
                            UMLongitud_Clave = UMLongitud_Clave,
                            ancho = ancho,
                            profundo = profundo,
                            diametro = diametro,
                            diametro2 = diametro2,
                            peso = peso,
                            cve_tipo_medida = cve_tipo_medida

                        };

                        if (string.IsNullOrWhiteSpace(otr_med)) otr_med = null;
                        else am.otr_med = otr_med;



                        listaAnt.Add(am);

                    }

                    con1.Close();

                    while (fin <= limite)
                    {
                        //tener la lista de pieza
                        var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                        foreach (var pieza in listPiezas)
                        {
                            var anonimaMed = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                            if (anonimaMed.Count > 0)
                            {
                                foreach (var anMed in anonimaMed)
                                {
                                    Medida med = new Medida()
                                    {
                                        Ancho = anMed.ancho,
                                        Diametro = anMed.diametro,
                                        Largo = anMed.alto,
                                        Peso = anMed.peso,
                                        PiezaID = pieza.PiezaID,
                                        Profundidad = anMed.profundo,
                                        Status = true,
                                        TipoMedidaID = tipoMed,
                                        Otra = anMed.otr_med
                                    };

                                    switch (anMed.UMLongitud_Clave)
                                    {
                                        case "21001":
                                            med.UMLongitud = UMLongitud.cm;
                                            break;
                                        case "21002":
                                            med.UMLongitud = UMLongitud.mm;
                                            break;
                                        case "21003":
                                            med.UMLongitud = UMLongitud.pulgadas;
                                            break;
                                        case "21004":
                                            med.UMLongitud = UMLongitud.m;
                                            break;
                                    }

                                    switch (anMed.UMPeso_Clave)
                                    {
                                        case "22002":
                                            med.UMMasa = UMMasa.kg;
                                            break;
                                        case "22003":
                                            med.UMMasa = UMMasa.gr;
                                            break;
                                    }



                                    dbx.Medidas.Add(med);
                                }
                            }

                        }

                        dbx.SaveChanges();

                        dbx.Dispose();
                        dbx = new RecordFCSContext();
                        dbx.Configuration.AutoDetectChangesEnabled = false;

                        inicio = fin;
                        fin = fin + total;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return PartialView("_ImportarPieza_Descriptivo");
        }


        //MEDIDAS - MARCO
        public ActionResult ImportarMedida_Marcos()
        {

            /*
                "Sin marco ó base"
                "Museográficas",
                "Marco"
            */

            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "MEDIDA MARCO";

            string nombreMedida = "Marco";
            Int64 tipoMed = db.TipoMedidas.Where(a => a.Nombre == nombreMedida).Select(a => a.TipoMedidaID).FirstOrDefault();

            ViewBag.error = "";
            if (tipoMed != 0)
            {
                try
                {
                    // mandar mensaje de conexion
                    ViewBag.mensaje = "Conexión establecida";
                    //definir el sql
                    Int64 limite = 100000;
                    Int64 total = 500;

                    Int64 inicio = 0; //colocar el PIEZAID del cual comenzar, se comienza desde actual+1
                    Int64 fin = inicio + total;

                    List<AnonimoMedida> listaAnt = new List<AnonimoMedida>();
                    con1.Open();
                    string tabla = "m_pieza_MedidasMarcos";
                    string textSql1 = string.Format("SELECT * FROM [{0}]", tabla);
                    SqlCommand sql1 = new SqlCommand(textSql1, con1);
                    SqlDataReader leer1 = sql1.ExecuteReader();

                    while (leer1.Read())
                    {
                        string id_pieza = leer1["id_pieza"].ToString();
                        string UMPeso_Clave = leer1["UMPeso_Clave"].ToString();
                        double? alto = Convert.ToDouble(leer1["alto"].ToString());
                        string UMLongitud_Clave = leer1["UMLongitud_Clave"].ToString();
                        double? ancho = Convert.ToDouble(leer1["ancho"].ToString());
                        double? profundo = Convert.ToDouble(leer1["profundo"].ToString());
                        double? diametro = Convert.ToDouble(leer1["diametro"].ToString());
                        double? diametro2 = Convert.ToDouble(leer1["diametro2"].ToString());
                        double? peso = Convert.ToDouble(leer1["peso"].ToString());
                        string cve_tipo_medida = leer1["cve_tipo_medida"].ToString();
                        string otr_med = leer1["otr_med"].ToString();


                        if (alto == 0.0) alto = null;
                        if (ancho == 0.0) ancho = null;
                        if (profundo == 0.0) profundo = null;
                        if (diametro == 0.0) diametro = null;
                        if (diametro2 == 0.0) diametro2 = null;
                        if (peso == 0.0) peso = null;



                        //ingreso la validacion del campo clave
                        AnonimoMedida am = new AnonimoMedida()
                        {
                            id_pieza = id_pieza,
                            UMPeso_Clave = UMPeso_Clave,
                            alto = alto,
                            UMLongitud_Clave = UMLongitud_Clave,
                            ancho = ancho,
                            profundo = profundo,
                            diametro = diametro,
                            diametro2 = diametro2,
                            peso = peso,
                            cve_tipo_medida = cve_tipo_medida

                        };

                        if (string.IsNullOrWhiteSpace(otr_med)) otr_med = null;
                        else am.otr_med = otr_med;



                        listaAnt.Add(am);

                    }

                    con1.Close();

                    while (fin <= limite)
                    {
                        //tener la lista de pieza
                        var listPiezas = dbx.Piezas.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.PiezaID, a.TipoPiezaID, a.Obra.AntID }).ToList();

                        foreach (var pieza in listPiezas)
                        {
                            var anonimaMed = listaAnt.Where(a => a.id_pieza == pieza.AntID).ToList();

                            if (anonimaMed.Count > 0)
                            {
                                foreach (var anMed in anonimaMed)
                                {
                                    Medida med = new Medida()
                                    {
                                        Ancho = anMed.ancho,
                                        Diametro = anMed.diametro,
                                        Largo = anMed.alto,
                                        Peso = anMed.peso,
                                        PiezaID = pieza.PiezaID,
                                        Profundidad = anMed.profundo,
                                        Status = true,
                                        TipoMedidaID = tipoMed,
                                        Otra = anMed.otr_med
                                    };

                                    switch (anMed.UMLongitud_Clave)
                                    {
                                        case "21001":
                                            med.UMLongitud = UMLongitud.cm;
                                            break;
                                        case "21002":
                                            med.UMLongitud = UMLongitud.mm;
                                            break;
                                        case "21003":
                                            med.UMLongitud = UMLongitud.pulgadas;
                                            break;
                                        case "21004":
                                            med.UMLongitud = UMLongitud.m;
                                            break;
                                    }

                                    switch (anMed.UMPeso_Clave)
                                    {
                                        case "22002":
                                            med.UMMasa = UMMasa.kg;
                                            break;
                                        case "22003":
                                            med.UMMasa = UMMasa.gr;
                                            break;
                                    }



                                    dbx.Medidas.Add(med);
                                }
                            }

                        }

                        dbx.SaveChanges();

                        dbx.Dispose();
                        dbx = new RecordFCSContext();
                        dbx.Configuration.AutoDetectChangesEnabled = false;

                        inicio = fin;
                        fin = fin + total;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return PartialView("_ImportarPieza_Descriptivo");
        }

        //---------------------------------------------------------------------
        // OTRAS PIEZAS DE LA OBRA
        public ActionResult ImportarOtrasPiezas()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            ViewBag.NombreTabla = "IMPORTAR OTRAS PIEZAS";

            //buscar los TipoAtributoID

            //try
            //{
            // mandar mensaje de conexion
            ViewBag.mensaje = "Conexión establecida";
            //definir el sql
            Int64 limite = 100000;
            Int64 total = 500;

            Int64 inicio = 31125; //colocar el PIEZAID del cual comenzar, se comienza desde actual+1
            Int64 fin = inicio + total;


            var tipoAtt_TecnicaID = dbx.TipoAtributos.Where(a => a.AntNombre == "MatriculaTecnica_Clave").Select(a => a.TipoAtributoID).FirstOrDefault();
            var tipoAtt_Titulo = dbx.TipoAtributos.Where(a => a.AntNombre == "titulo").Select(a => a.TipoAtributoID).FirstOrDefault();
            var tipoAtt_Descripcion = dbx.TipoAtributos.Where(a => a.AntNombre == "descripcion").Select(a => a.TipoAtributoID).FirstOrDefault();
            var tipoAtt_Foto = dbx.TipoAtributos.Where(a => a.AntNombre == "m_pieza_foto").Select(a => a.TipoAtributoID).FirstOrDefault();

            var listaTecnicas = dbx.Tecnicas.Select(a => new { a.TecnicaID, a.AntID }).ToList();



            List<AnonimoOtraPieza> listaAnt = new List<AnonimoOtraPieza>();
            con1.Open();
            string tabla = "m_otras_piezas";
            string textSql1 = string.Format("SELECT * FROM [{0}] ORDER BY [id_pieza], [Sub_pieza] , [nSubIndex]", tabla);
            SqlCommand sql1 = new SqlCommand(textSql1, con1);
            SqlDataReader leer1 = sql1.ExecuteReader();

            while (leer1.Read())
            {
                string id_pieza = leer1["id_pieza"].ToString();
                int Sub_pieza = Convert.ToInt32(leer1["Sub_pieza"].ToString());
                int Consec = Convert.ToInt32(leer1["Consec"].ToString());
                string MatriculaTecnica_Clave = leer1["MatriculaTecnica_Clave"].ToString();
                string TipoPieza_Clave = leer1["TipoPieza_Clave"].ToString();
                string TipoPieza_Descripcion = leer1["TipoPieza_Descripcion"].ToString();
                string Ubicacion_Clave = leer1["Ubicacion_Clave"].ToString();
                string ruta_imagen = leer1["ruta_imagen"].ToString();
                int nSubIndex = Convert.ToInt32(leer1["nSubIndex"].ToString());
                double? Alto = Convert.ToDouble(leer1["Alto"].ToString());
                double? Ancho = Convert.ToDouble(leer1["Ancho"].ToString());
                double? Fondo = Convert.ToDouble(leer1["Fondo"].ToString());
                double? Diametro = Convert.ToDouble(leer1["Diametro"].ToString());
                double? Diametro2 = Convert.ToDouble(leer1["Diametro2"].ToString());
                string Descripcion = leer1["Descripcion"].ToString();
                string UbicacionActual = leer1["UbicacionActual"].ToString();
                int Jerarquia_Clave = Convert.ToInt32(leer1["Jerarquia_Clave"].ToString());

                if (Alto == 0.0) Alto = null;
                if (Ancho == 0.0) Ancho = null;
                if (Fondo == 0.0) Fondo = null;
                if (Diametro == 0.0) Diametro = null;
                if (Diametro2 == 0.0) Diametro2 = null;

                listaAnt.Add(new AnonimoOtraPieza()
                {
                    id_pieza = id_pieza,
                    Sub_pieza = Sub_pieza,
                    Consec = Consec,
                    MatriculaTecnica_Clave = MatriculaTecnica_Clave,
                    TipoPieza_Clave = TipoPieza_Clave,
                    TipoPieza_Descripcion = TipoPieza_Descripcion,
                    Ubicacion_Clave = Ubicacion_Clave,
                    ruta_imagen = ruta_imagen,
                    nSubIndex = nSubIndex,
                    Alto = Alto,
                    Ancho = Ancho,
                    Fondo = Fondo,
                    Diametro = Diametro,
                    Diametro2 = Diametro2,
                    Descripcion = Descripcion,
                    UbicacionActual = UbicacionActual,
                    Jerarquia_Clave = Jerarquia_Clave
                });
            }
            con1.Close();

            string[] letras = 
                { 
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
                    
                    "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", 
                    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
                    "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
                    "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
                    "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
                    "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ",
                    "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ",
                    "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ", 
                    "IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV", "IW", "IX", "IY", "IZ", 
                    "JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS", "JT", "JU", "JV", "JW", "JX", "JY", "JZ", 
                    "KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KI", "KJ", "KK", "KL", "KM", "KN", "KO", "KP", "KQ", "KR", "KS", "KT", "KU", "KV", "KW", "KX", "KY", "KZ", 
                    "LA", "LB", "LC", "LD", "LE", "LF", "LG", "LH", "LI", "LJ", "LK", "LL", "LM", "LN", "LO", "LP", "LQ", "LR", "LS", "LT", "LU", "LV", "LW", "LX", "LY", "LZ", 
                    "MA", "MB", "MC", "MD", "ME", "MF", "MG", "MH", "MI", "MJ", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", 
                    "NA", "NB", "NC", "ND", "NE", "NF", "NG", "NH", "NI", "NJ", "NK", "NL", "NM", "NN", "NO", "NP", "NQ", "NR", "NS", "NT", "NU", "NV", "NW", "NX", "NY", "NZ", 
                    "OA", "OB", "OC", "OD", "OE", "OF", "OG", "OH", "OI", "OJ", "OK", "OL", "OM", "ON", "OO", "OP", "OQ", "OR", "OS", "OT", "OU", "OV", "OW", "OX", "OY", "OZ", 
                    "PA", "PB", "PC", "PD", "PE", "PF", "PG", "PH", "PI", "PJ", "PK", "PL", "PM", "PN", "PO", "PP", "PQ", "PR", "PS", "PT", "PU", "PV", "PW", "PX", "PY", "PZ", 
                    "QA", "QB", "QC", "QD", "QE", "QF", "QG", "QH", "QI", "QJ", "QK", "QL", "QM", "QN", "QO", "QP", "QQ", "QR", "QS", "QT", "QU", "QV", "QW", "QX", "QY", "QZ", 
                    "RA", "RB", "RC", "RD", "RE", "RF", "RG", "RH", "RI", "RJ", "RK", "RL", "RM", "RN", "RO", "RP", "RQ", "RR", "RS", "RT", "RU", "RV", "RW", "RX", "RY", "RZ", 
                    "SA", "SB", "SC", "SD", "SE", "SF", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SP", "SQ", "SR", "SS", "ST", "SU", "SV", "SW", "SX", "SY", "SZ", 
                    "TA", "TB", "TC", "TD", "TE", "TF", "TG", "TH", "TI", "TJ", "TK", "TL", "TM", "TN", "TO", "TP", "TQ", "TR", "TS", "TT", "TU", "TV", "TW", "TX", "TY", "TZ", 
                    "UA", "UB", "UC", "UD", "UE", "UF", "UG", "UH", "UI", "UJ", "UK", "UL", "UM", "UN", "UO", "UP", "UQ", "UR", "US", "UT", "UU", "UV", "UW", "UX", "UY", "UZ", 
                    "VA", "VB", "VC", "VD", "VE", "VF", "VG", "VH", "VI", "VJ", "VK", "VL", "VM", "VN", "VO", "VP", "VQ", "VR", "VS", "VT", "VU", "VV", "VW", "VX", "VY", "VZ", 
                    "WA", "WB", "WC", "WD", "WE", "WF", "WG", "WH", "WI", "WJ", "WK", "WL", "WM", "WN", "WO", "WP", "WQ", "WR", "WS", "WT", "WU", "WV", "WW", "WX", "WY", "WZ", 
                    "XA", "XB", "XC", "XD", "XE", "XF", "XG", "XH", "XI", "XJ", "XK", "XL", "XM", "XN", "XO", "XP", "XQ", "XR", "XS", "XT", "XU", "XV", "XW", "XX", "XY", "XZ", 
                    "YA", "YB", "YC", "YD", "YE", "YF", "YG", "YH", "YI", "YJ", "YK", "YL", "YM", "YN", "YO", "YP", "YQ", "YR", "YS", "YT", "YU", "YV", "YW", "YX", "YY", "YZ", 
                    "ZA", "ZB", "ZC", "ZD", "ZE", "ZF", "ZG", "ZH", "ZI", "ZJ", "ZK", "ZL", "ZM", "ZN", "ZO", "ZP", "ZQ", "ZR", "ZS", "ZT", "ZU", "ZV", "ZW", "ZX", "ZY", "ZZ",
                    
                    "AAA", "AAB", "AAC", "AAD", "AAE", "AAF", "AAG", "AAH", "AAI", "AAJ", "AAK", "AAL", "AAM", "AAN", "AAO", "AAP", "AAQ", "AAR", "AAS", "AAT", "AAU", "AAV", "AAW", "AAX", "AAY", "AAZ", 
                    "BAA", "BAB", "BAC", "BAD", "BAE", "BAF", "BAG", "BAH", "BAI", "BAJ", "BAK", "BAL", "BAM", "BAN", "BAO", "BAP", "BAQ", "BAR", "BAS", "BAT", "BAU", "BAV", "BAW", "BAX", "BAY", "BAZ", 
                    "CAA", "CAB", "CAC", "CAD", "CAE", "CAF", "CAG", "CAH", "CAI", "CAJ", "CAK", "CAL", "CAM", "CAN", "CAO", "CAP", "CAQ", "CAR", "CAS", "CAT", "CAU", "CAV", "CAW", "CAX", "CAY", "CAZ", 
                    "DAA", "DAB", "DAC", "DAD", "DAE", "DAF", "DAG", "DAH", "DAI", "DAJ", "DAK", "DAL", "DAM", "DAN", "DAO", "DAP", "DAQ", "DAR", "DAS", "DAT", "DAU", "DAV", "DAW", "DAX", "DAY", "DAZ", 
                    "EAA", "EAB", "EAC", "EAD", "EAE", "EAF", "EAG", "EAH", "EAI", "EAJ", "EAK", "EAL", "EAM", "EAN", "EAO", "EAP", "EAQ", "EAR", "EAS", "EAT", "EAU", "EAV", "EAW", "EAX", "EAY", "EAZ", 
                    "FAA", "FAB", "FAC", "FAD", "FAE", "FAF", "FAG", "FAH", "FAI", "FAJ", "FAK", "FAL", "FAM", "FAN", "FAO", "FAP", "FAQ", "FAR", "FAS", "FAT", "FAU", "FAV", "FAW", "FAX", "FAY", "FAZ", 
                    "GAA", "GAB", "GAC", "GAD", "GAE", "GAF", "GAG", "GAH", "GAI", "GAJ", "GAK", "GAL", "GAM", "GAN", "GAO", "GAP", "GAQ", "GAR", "GAS", "GAT", "GAU", "GAV", "GAW", "GAX", "GAY", "GAZ", 
                    "HAA", "HAB", "HAC", "HAD", "HAE", "HAF", "HAG", "HAH", "HAI", "HAJ", "HAK", "HAL", "HAM", "HAN", "HAO", "HAP", "HAQ", "HAR", "HAS", "HAT", "HAU", "HAV", "HAW", "HAX", "HAY", "HAZ", 
                    "IAA", "IAB", "IAC", "IAD", "IAE", "IAF", "IAG", "IAH", "IAI", "IAJ", "IAK", "IAL", "IAM", "IAN", "IAO", "IAP", "IAQ", "IAR", "IAS", "IAT", "IAU", "IAV", "IAW", "IAX", "IAY", "IAZ", 
                    "JAA", "JAB", "JAC", "JAD", "JAE", "JAF", "JAG", "JAH", "JAI", "JAJ", "JAK", "JAL", "JAM", "JAN", "JAO", "JAP", "JAQ", "JAR", "JAS", "JAT", "JAU", "JAV", "JAW", "JAX", "JAY", "JAZ", 
                    "KAA", "KAB", "KAC", "KAD", "KAE", "KAF", "KAG", "KAH", "KAI", "KAJ", "KAK", "KAL", "KAM", "KAN", "KAO", "KAP", "KAQ", "KAR", "KAS", "KAT", "KAU", "KAV", "KAW", "KAX", "KAY", "KAZ", 
                    "LAA", "LAB", "LAC", "LAD", "LAE", "LAF", "LAG", "LAH", "LAI", "LAJ", "LAK", "LAL", "LAM", "LAN", "LAO", "LAP", "LAQ", "LAR", "LAS", "LAT", "LAU", "LAV", "LAW", "LAX", "LAY", "LAZ", 
                    "MAA", "MAB", "MAC", "MAD", "MAE", "MAF", "MAG", "MAH", "MAI", "MAJ", "MAK", "MAL", "MAM", "MAN", "MAO", "MAP", "MAQ", "MAR", "MAS", "MAT", "MAU", "MAV", "MAW", "MAX", "MAY", "MAZ", 
                    "NAA", "NAB", "NAC", "NAD", "NAE", "NAF", "NAG", "NAH", "NAI", "NAJ", "NAK", "NAL", "NAM", "NAN", "NAO", "NAP", "NAQ", "NAR", "NAS", "NAT", "NAU", "NAV", "NAW", "NAX", "NAY", "NAZ", 
                    "OAA", "OAB", "OAC", "OAD", "OAE", "OAF", "OAG", "OAH", "OAI", "OAJ", "OAK", "OAL", "OAM", "OAN", "OAO", "OAP", "OAQ", "OAR", "OAS", "OAT", "OAU", "OAV", "OAW", "OAX", "OAY", "OAZ", 
                    "PAA", "PAB", "PAC", "PAD", "PAE", "PAF", "PAG", "PAH", "PAI", "PAJ", "PAK", "PAL", "PAM", "PAN", "PAO", "PAP", "PAQ", "PAR", "PAS", "PAT", "PAU", "PAV", "PAW", "PAX", "PAY", "PAZ", 
                    "QAA", "QAB", "QAC", "QAD", "QAE", "QAF", "QAG", "QAH", "QAI", "QAJ", "QAK", "QAL", "QAM", "QAN", "QAO", "QAP", "QAQ", "QAR", "QAS", "QAT", "QAU", "QAV", "QAW", "QAX", "QAY", "QAZ", 
                    "RAA", "RAB", "RAC", "RAD", "RAE", "RAF", "RAG", "RAH", "RAI", "RAJ", "RAK", "RAL", "RAM", "RAN", "RAO", "RAP", "RAQ", "RAR", "RAS", "RAT", "RAU", "RAV", "RAW", "RAX", "RAY", "RAZ", 
                    "SAA", "SAB", "SAC", "SAD", "SAE", "SAF", "SAG", "SAH", "SAI", "SAJ", "SAK", "SAL", "SAM", "SAN", "SAO", "SAP", "SAQ", "SAR", "SAS", "SAT", "SAU", "SAV", "SAW", "SAX", "SAY", "SAZ", 
                    "TAA", "TAB", "TAC", "TAD", "TAE", "TAF", "TAG", "TAH", "TAI", "TAJ", "TAK", "TAL", "TAM", "TAN", "TAO", "TAP", "TAQ", "TAR", "TAS", "TAT", "TAU", "TAV", "TAW", "TAX", "TAY", "TAZ", 
                    "UAA", "UAB", "UAC", "UAD", "UAE", "UAF", "UAG", "UAH", "UAI", "UAJ", "UAK", "UAL", "UAM", "UAN", "UAO", "UAP", "UAQ", "UAR", "UAS", "UAT", "UAU", "UAV", "UAW", "UAX", "UAY", "UAZ", 
                    "VAA", "VAB", "VAC", "VAD", "VAE", "VAF", "VAG", "VAH", "VAI", "VAJ", "VAK", "VAL", "VAM", "VAN", "VAO", "VAP", "VAQ", "VAR", "VAS", "VAT", "VAU", "VAV", "VAW", "VAX", "VAY", "VAZ", 
                    "WAA", "WAB", "WAC", "WAD", "WAE", "WAF", "WAG", "WAH", "WAI", "WAJ", "WAK", "WAL", "WAM", "WAN", "WAO", "WAP", "WAQ", "WAR", "WAS", "WAT", "WAU", "WAV", "WAW", "WAX", "WAY", "WAZ", 
                    "XAA", "XAB", "XAC", "XAD", "XAE", "XAF", "XAG", "XAH", "XAI", "XAJ", "XAK", "XAL", "XAM", "XAN", "XAO", "XAP", "XAQ", "XAR", "XAS", "XAT", "XAU", "XAV", "XAW", "XAX", "XAY", "XAZ", 
                    "YAA", "YAB", "YAC", "YAD", "YAE", "YAF", "YAG", "YAH", "YAI", "YAJ", "YAK", "YAL", "YAM", "YAN", "YAO", "YAP", "YAQ", "YAR", "YAS", "YAT", "YAU", "YAV", "YAW", "YAX", "YAY", "YAZ", 
                    "ZAA", "ZAB", "ZAC", "ZAD", "ZAE", "ZAF", "ZAG", "ZAH", "ZAI", "ZAJ", "ZAK", "ZAL", "ZAM", "ZAN", "ZAO", "ZAP", "ZAQ", "ZAR", "ZAS", "ZAT", "ZAU", "ZAV", "ZAW", "ZAX", "ZAY", "ZAZ", 

                    "ABA", "ABB", "ABC", "ABD", "ABE", "ABF", "ABG", "ABH", "ABI", "ABJ", "ABK", "ABL", "ABM", "ABN", "ABO", "ABP", "ABQ", "ABR", "ABS", "ABT", "ABU", "ABV", "ABW", "ABX", "ABY", "ABZ", 
                    "BBA", "BBB", "BBC", "BBD", "BBE", "BBF", "BBG", "BBH", "BBI", "BBJ", "BBK", "BBL", "BBM", "BBN", "BBO", "BBP", "BBQ", "BBR", "BBS", "BBT", "BBU", "BBV", "BBW", "BBX", "BBY", "BBZ", 
                    "CBA", "CBB", "CBC", "CBD", "CBE", "CBF", "CBG", "CBH", "CBI", "CBJ", "CBK", "CBL", "CBM", "CBN", "CBO", "CBP", "CBQ", "CBR", "CBS", "CBT", "CBU", "CBV", "CBW", "CBX", "CBY", "CBZ", 
                    "DBA", "DBB", "DBC", "DBD", "DBE", "DBF", "DBG", "DBH", "DBI", "DBJ", "DBK", "DBL", "DBM", "DBN", "DBO", "DBP", "DBQ", "DBR", "DBS", "DBT", "DBU", "DBV", "DBW", "DBX", "DBY", "DBZ", 
                    "EBA", "EBB", "EBC", "EBD", "EBE", "EBF", "EBG", "EBH", "EBI", "EBJ", "EBK", "EBL", "EBM", "EBN", "EBO", "EBP", "EBQ", "EBR", "EBS", "EBT", "EBU", "EBV", "EBW", "EBX", "EBY", "EBZ", 
                    "FBA", "FBB", "FBC", "FBD", "FBE", "FBF", "FBG", "FBH", "FBI", "FBJ", "FBK", "FBL", "FBM", "FBN", "FBO", "FBP", "FBQ", "FBR", "FBS", "FBT", "FBU", "FBV", "FBW", "FBX", "FBY", "FBZ", 
                    "GBA", "GBB", "GBC", "GBD", "GBE", "GBF", "GBG", "GBH", "GBI", "GBJ", "GBK", "GBL", "GBM", "GBN", "GBO", "GBP", "GBQ", "GBR", "GBS", "GBT", "GBU", "GBV", "GBW", "GBX", "GBY", "GBZ", 
                    "HBA", "HBB", "HBC", "HBD", "HBE", "HBF", "HBG", "HBH", "HBI", "HBJ", "HBK", "HBL", "HBM", "HBN", "HBO", "HBP", "HBQ", "HBR", "HBS", "HBT", "HBU", "HBV", "HBW", "HBX", "HBY", "HBZ", 
                    "IBA", "IBB", "IBC", "IBD", "IBE", "IBF", "IBG", "IBH", "IBI", "IBJ", "IBK", "IBL", "IBM", "IBN", "IBO", "IBP", "IBQ", "IBR", "IBS", "IBT", "IBU", "IBV", "IBW", "IBX", "IBY", "IBZ", 
                    "JBA", "JBB", "JBC", "JBD", "JBE", "JBF", "JBG", "JBH", "JBI", "JBJ", "JBK", "JBL", "JBM", "JBN", "JBO", "JBP", "JBQ", "JBR", "JBS", "JBT", "JBU", "JBV", "JBW", "JBX", "JBY", "JBZ", 
                    "KBA", "KBB", "KBC", "KBD", "KBE", "KBF", "KBG", "KBH", "KBI", "KBJ", "KBK", "KBL", "KBM", "KBN", "KBO", "KBP", "KBQ", "KBR", "KBS", "KBT", "KBU", "KBV", "KBW", "KBX", "KBY", "KBZ", 
                    "LBA", "LBB", "LBC", "LBD", "LBE", "LBF", "LBG", "LBH", "LBI", "LBJ", "LBK", "LBL", "LBM", "LBN", "LBO", "LBP", "LBQ", "LBR", "LBS", "LBT", "LBU", "LBV", "LBW", "LBX", "LBY", "LBZ", 
                    "MBA", "MBB", "MBC", "MBD", "MBE", "MBF", "MBG", "MBH", "MBI", "MBJ", "MBK", "MBL", "MBM", "MBN", "MBO", "MBP", "MBQ", "MBR", "MBS", "MBT", "MBU", "MBV", "MBW", "MBX", "MBY", "MBZ", 
                    "NBA", "NBB", "NBC", "NBD", "NBE", "NBF", "NBG", "NBH", "NBI", "NBJ", "NBK", "NBL", "NBM", "NBN", "NBO", "NBP", "NBQ", "NBR", "NBS", "NBT", "NBU", "NBV", "NBW", "NBX", "NBY", "NBZ", 
                    "OBA", "OBB", "OBC", "OBD", "OBE", "OBF", "OBG", "OBH", "OBI", "OBJ", "OBK", "OBL", "OBM", "OBN", "OBO", "OBP", "OBQ", "OBR", "OBS", "OBT", "OBU", "OBV", "OBW", "OBX", "OBY", "OBZ", 
                    "PBA", "PBB", "PBC", "PBD", "PBE", "PBF", "PBG", "PBH", "PBI", "PBJ", "PBK", "PBL", "PBM", "PBN", "PBO", "PBP", "PBQ", "PBR", "PBS", "PBT", "PBU", "PBV", "PBW", "PBX", "PBY", "PBZ", 
                    "QBA", "QBB", "QBC", "QBD", "QBE", "QBF", "QBG", "QBH", "QBI", "QBJ", "QBK", "QBL", "QBM", "QBN", "QBO", "QBP", "QBQ", "QBR", "QBS", "QBT", "QBU", "QBV", "QBW", "QBX", "QBY", "QBZ", 
                    "RBA", "RBB", "RBC", "RBD", "RBE", "RBF", "RBG", "RBH", "RBI", "RBJ", "RBK", "RBL", "RBM", "RBN", "RBO", "RBP", "RBQ", "RBR", "RBS", "RBT", "RBU", "RBV", "RBW", "RBX", "RBY", "RBZ", 
                    "SBA", "SBB", "SBC", "SBD", "SBE", "SBF", "SBG", "SBH", "SBI", "SBJ", "SBK", "SBL", "SBM", "SBN", "SBO", "SBP", "SBQ", "SBR", "SBS", "SBT", "SBU", "SBV", "SBW", "SBX", "SBY", "SBZ", 
                    "TBA", "TBB", "TBC", "TBD", "TBE", "TBF", "TBG", "TBH", "TBI", "TBJ", "TBK", "TBL", "TBM", "TBN", "TBO", "TBP", "TBQ", "TBR", "TBS", "TBT", "TBU", "TBV", "TBW", "TBX", "TBY", "TBZ", 
                    "UBA", "UBB", "UBC", "UBD", "UBE", "UBF", "UBG", "UBH", "UBI", "UBJ", "UBK", "UBL", "UBM", "UBN", "UBO", "UBP", "UBQ", "UBR", "UBS", "UBT", "UBU", "UBV", "UBW", "UBX", "UBY", "UBZ", 
                    "VBA", "VBB", "VBC", "VBD", "VBE", "VBF", "VBG", "VBH", "VBI", "VBJ", "VBK", "VBL", "VBM", "VBN", "VBO", "VBP", "VBQ", "VBR", "VBS", "VBT", "VBU", "VBV", "VBW", "VBX", "VBY", "VBZ", 
                    "WBA", "WBB", "WBC", "WBD", "WBE", "WBF", "WBG", "WBH", "WBI", "WBJ", "WBK", "WBL", "WBM", "WBN", "WBO", "WBP", "WBQ", "WBR", "WBS", "WBT", "WBU", "WBV", "WBW", "WBX", "WBY", "WBZ", 
                    "XBA", "XBB", "XBC", "XBD", "XBE", "XBF", "XBG", "XBH", "XBI", "XBJ", "XBK", "XBL", "XBM", "XBN", "XBO", "XBP", "XBQ", "XBR", "XBS", "XBT", "XBU", "XBV", "XBW", "XBX", "XBY", "XBZ", 
                    "YBA", "YBB", "YBC", "YBD", "YBE", "YBF", "YBG", "YBH", "YBI", "YBJ", "YBK", "YBL", "YBM", "YBN", "YBO", "YBP", "YBQ", "YBR", "YBS", "YBT", "YBU", "YBV", "YBW", "YBX", "YBY", "YBZ", 
                    "ZBA", "ZBB", "ZBC", "ZBD", "ZBE", "ZBF", "ZBG", "ZBH", "ZBI", "ZBJ", "ZBK", "ZBL", "ZBM", "ZBN", "ZBO", "ZBP", "ZBQ", "ZBR", "ZBS", "ZBT", "ZBU", "ZBV", "ZBW", "ZBX", "ZBY", "ZBZ", 

                    "ACA", "ACB", "ACC", "ACD", "ACE", "ACF", "ACG", "ACH", "ACI", "ACJ", "ACK", "ACL", "ACM", "ACN", "ACO", "ACP", "ACQ", "ACR", "ACS", "ACT", "ACU", "ACV", "ACW", "ACX", "ACY", "ACZ", 
                    "BCA", "BCB", "BCC", "BCD", "BCE", "BCF", "BCG", "BCH", "BCI", "BCJ", "BCK", "BCL", "BCM", "BCN", "BCO", "BCP", "BCQ", "BCR", "BCS", "BCT", "BCU", "BCV", "BCW", "BCX", "BCY", "BCZ", 
                    "CCA", "CCB", "CCC", "CCD", "CCE", "CCF", "CCG", "CCH", "CCI", "CCJ", "CCK", "CCL", "CCM", "CCN", "CCO", "CCP", "CCQ", "CCR", "CCS", "CCT", "CCU", "CCV", "CCW", "CCX", "CCY", "CCZ", 
                    "DCA", "DCB", "DCC", "DCD", "DCE", "DCF", "DCG", "DCH", "DCI", "DCJ", "DCK", "DCL", "DCM", "DCN", "DCO", "DCP", "DCQ", "DCR", "DCS", "DCT", "DCU", "DCV", "DCW", "DCX", "DCY", "DCZ", 
                    "ECA", "ECB", "ECC", "ECD", "ECE", "ECF", "ECG", "ECH", "ECI", "ECJ", "ECK", "ECL", "ECM", "ECN", "ECO", "ECP", "ECQ", "ECR", "ECS", "ECT", "ECU", "ECV", "ECW", "ECX", "ECY", "ECZ", 
                    "FCA", "FCB", "FCC", "FCD", "FCE", "FCF", "FCG", "FCH", "FCI", "FCJ", "FCK", "FCL", "FCM", "FCN", "FCO", "FCP", "FCQ", "FCR", "FCS", "FCT", "FCU", "FCV", "FCW", "FCX", "FCY", "FCZ", 
                    "GCA", "GCB", "GCC", "GCD", "GCE", "GCF", "GCG", "GCH", "GCI", "GCJ", "GCK", "GCL", "GCM", "GCN", "GCO", "GCP", "GCQ", "GCR", "GCS", "GCT", "GCU", "GCV", "GCW", "GCX", "GCY", "GCZ", 
                    "HCA", "HCB", "HCC", "HCD", "HCE", "HCF", "HCG", "HCH", "HCI", "HCJ", "HCK", "HCL", "HCM", "HCN", "HCO", "HCP", "HCQ", "HCR", "HCS", "HCT", "HCU", "HCV", "HCW", "HCX", "HCY", "HCZ", 
                    "ICA", "ICB", "ICC", "ICD", "ICE", "ICF", "ICG", "ICH", "ICI", "ICJ", "ICK", "ICL", "ICM", "ICN", "ICO", "ICP", "ICQ", "ICR", "ICS", "ICT", "ICU", "ICV", "ICW", "ICX", "ICY", "ICZ", 
                    "JCA", "JCB", "JCC", "JCD", "JCE", "JCF", "JCG", "JCH", "JCI", "JCJ", "JCK", "JCL", "JCM", "JCN", "JCO", "JCP", "JCQ", "JCR", "JCS", "JCT", "JCU", "JCV", "JCW", "JCX", "JCY", "JCZ", 
                    "KCA", "KCB", "KCC", "KCD", "KCE", "KCF", "KCG", "KCH", "KCI", "KCJ", "KCK", "KCL", "KCM", "KCN", "KCO", "KCP", "KCQ", "KCR", "KCS", "KCT", "KCU", "KCV", "KCW", "KCX", "KCY", "KCZ", 
                    "LCA", "LCB", "LCC", "LCD", "LCE", "LCF", "LCG", "LCH", "LCI", "LCJ", "LCK", "LCL", "LCM", "LCN", "LCO", "LCP", "LCQ", "LCR", "LCS", "LCT", "LCU", "LCV", "LCW", "LCX", "LCY", "LCZ", 
                    "MCA", "MCB", "MCC", "MCD", "MCE", "MCF", "MCG", "MCH", "MCI", "MCJ", "MCK", "MCL", "MCM", "MCN", "MCO", "MCP", "MCQ", "MCR", "MCS", "MCT", "MCU", "MCV", "MCW", "MCX", "MCY", "MCZ", 
                    "NCA", "NCB", "NCC", "NCD", "NCE", "NCF", "NCG", "NCH", "NCI", "NCJ", "NCK", "NCL", "NCM", "NCN", "NCO", "NCP", "NCQ", "NCR", "NCS", "NCT", "NCU", "NCV", "NCW", "NCX", "NCY", "NCZ", 
                    "OCA", "OCB", "OCC", "OCD", "OCE", "OCF", "OCG", "OCH", "OCI", "OCJ", "OCK", "OCL", "OCM", "OCN", "OCO", "OCP", "OCQ", "OCR", "OCS", "OCT", "OCU", "OCV", "OCW", "OCX", "OCY", "OCZ", 
                    "PCA", "PCB", "PCC", "PCD", "PCE", "PCF", "PCG", "PCH", "PCI", "PCJ", "PCK", "PCL", "PCM", "PCN", "PCO", "PCP", "PCQ", "PCR", "PCS", "PCT", "PCU", "PCV", "PCW", "PCX", "PCY", "PCZ", 
                    "QCA", "QCB", "QCC", "QCD", "QCE", "QCF", "QCG", "QCH", "QCI", "QCJ", "QCK", "QCL", "QCM", "QCN", "QCO", "QCP", "QCQ", "QCR", "QCS", "QCT", "QCU", "QCV", "QCW", "QCX", "QCY", "QCZ", 
                    "RCA", "RCB", "RCC", "RCD", "RCE", "RCF", "RCG", "RCH", "RCI", "RCJ", "RCK", "RCL", "RCM", "RCN", "RCO", "RCP", "RCQ", "RCR", "RCS", "RCT", "RCU", "RCV", "RCW", "RCX", "RCY", "RCZ", 
                    "SCA", "SCB", "SCC", "SCD", "SCE", "SCF", "SCG", "SCH", "SCI", "SCJ", "SCK", "SCL", "SCM", "SCN", "SCO", "SCP", "SCQ", "SCR", "SCS", "SCT", "SCU", "SCV", "SCW", "SCX", "SCY", "SCZ", 
                    "TCA", "TCB", "TCC", "TCD", "TCE", "TCF", "TCG", "TCH", "TCI", "TCJ", "TCK", "TCL", "TCM", "TCN", "TCO", "TCP", "TCQ", "TCR", "TCS", "TCT", "TCU", "TCV", "TCW", "TCX", "TCY", "TCZ", 
                    "UCA", "UCB", "UCC", "UCD", "UCE", "UCF", "UCG", "UCH", "UCI", "UCJ", "UCK", "UCL", "UCM", "UCN", "UCO", "UCP", "UCQ", "UCR", "UCS", "UCT", "UCU", "UCV", "UCW", "UCX", "UCY", "UCZ", 
                    "VCA", "VCB", "VCC", "VCD", "VCE", "VCF", "VCG", "VCH", "VCI", "VCJ", "VCK", "VCL", "VCM", "VCN", "VCO", "VCP", "VCQ", "VCR", "VCS", "VCT", "VCU", "VCV", "VCW", "VCX", "VCY", "VCZ", 
                    "WCA", "WCB", "WCC", "WCD", "WCE", "WCF", "WCG", "WCH", "WCI", "WCJ", "WCK", "WCL", "WCM", "WCN", "WCO", "WCP", "WCQ", "WCR", "WCS", "WCT", "WCU", "WCV", "WCW", "WCX", "WCY", "WCZ", 
                    "XCA", "XCB", "XCC", "XCD", "XCE", "XCF", "XCG", "XCH", "XCI", "XCJ", "XCK", "XCL", "XCM", "XCN", "XCO", "XCP", "XCQ", "XCR", "XCS", "XCT", "XCU", "XCV", "XCW", "XCX", "XCY", "XCZ", 
                    "YCA", "YCB", "YCC", "YCD", "YCE", "YCF", "YCG", "YCH", "YCI", "YCJ", "YCK", "YCL", "YCM", "YCN", "YCO", "YCP", "YCQ", "YCR", "YCS", "YCT", "YCU", "YCV", "YCW", "YCX", "YCY", "YCZ", 
                    "ZCA", "ZCB", "ZCC", "ZCD", "ZCE", "ZCF", "ZCG", "ZCH", "ZCI", "ZCJ", "ZCK", "ZCL", "ZCM", "ZCN", "ZCO", "ZCP", "ZCQ", "ZCR", "ZCS", "ZCT", "ZCU", "ZCV", "ZCW", "ZCX", "ZCY", "ZCZ", 

                    "ADA", "ADB", "ADC", "ADD", "ADE", "ADF", "ADG", "ADH", "ADI", "ADJ", "ADK", "ADL", "ADM", "ADN", "ADO", "ADP", "ADQ", "ADR", "ADS", "ADT", "ADU", "ADV", "ADW", "ADX", "ADY", "ADZ", 
                    "BDA", "BDB", "BDC", "BDD", "BDE", "BDF", "BDG", "BDH", "BDI", "BDJ", "BDK", "BDL", "BDM", "BDN", "BDO", "BDP", "BDQ", "BDR", "BDS", "BDT", "BDU", "BDV", "BDW", "BDX", "BDY", "BDZ", 
                    "CDA", "CDB", "CDC", "CDD", "CDE", "CDF", "CDG", "CDH", "CDI", "CDJ", "CDK", "CDL", "CDM", "CDN", "CDO", "CDP", "CDQ", "CDR", "CDS", "CDT", "CDU", "CDV", "CDW", "CDX", "CDY", "CDZ", 
                    "DDA", "DDB", "DDC", "DDD", "DDE", "DDF", "DDG", "DDH", "DDI", "DDJ", "DDK", "DDL", "DDM", "DDN", "DDO", "DDP", "DDQ", "DDR", "DDS", "DDT", "DDU", "DDV", "DDW", "DDX", "DDY", "DDZ", 
                    "EDA", "EDB", "EDC", "EDD", "EDE", "EDF", "EDG", "EDH", "EDI", "EDJ", "EDK", "EDL", "EDM", "EDN", "EDO", "EDP", "EDQ", "EDR", "EDS", "EDT", "EDU", "EDV", "EDW", "EDX", "EDY", "EDZ", 
                    "FDA", "FDB", "FDC", "FDD", "FDE", "FDF", "FDG", "FDH", "FDI", "FDJ", "FDK", "FDL", "FDM", "FDN", "FDO", "FDP", "FDQ", "FDR", "FDS", "FDT", "FDU", "FDV", "FDW", "FDX", "FDY", "FDZ", 
                    "GDA", "GDB", "GDC", "GDD", "GDE", "GDF", "GDG", "GDH", "GDI", "GDJ", "GDK", "GDL", "GDM", "GDN", "GDO", "GDP", "GDQ", "GDR", "GDS", "GDT", "GDU", "GDV", "GDW", "GDX", "GDY", "GDZ", 
                    "HDA", "HDB", "HDC", "HDD", "HDE", "HDF", "HDG", "HDH", "HDI", "HDJ", "HDK", "HDL", "HDM", "HDN", "HDO", "HDP", "HDQ", "HDR", "HDS", "HDT", "HDU", "HDV", "HDW", "HDX", "HDY", "HDZ", 
                    "IDA", "IDB", "IDC", "IDD", "IDE", "IDF", "IDG", "IDH", "IDI", "IDJ", "IDK", "IDL", "IDM", "IDN", "IDO", "IDP", "IDQ", "IDR", "IDS", "IDT", "IDU", "IDV", "IDW", "IDX", "IDY", "IDZ", 
                    "JDA", "JDB", "JDC", "JDD", "JDE", "JDF", "JDG", "JDH", "JDI", "JDJ", "JDK", "JDL", "JDM", "JDN", "JDO", "JDP", "JDQ", "JDR", "JDS", "JDT", "JDU", "JDV", "JDW", "JDX", "JDY", "JDZ", 
                    "KDA", "KDB", "KDC", "KDD", "KDE", "KDF", "KDG", "KDH", "KDI", "KDJ", "KDK", "KDL", "KDM", "KDN", "KDO", "KDP", "KDQ", "KDR", "KDS", "KDT", "KDU", "KDV", "KDW", "KDX", "KDY", "KDZ", 
                    "LDA", "LDB", "LDC", "LDD", "LDE", "LDF", "LDG", "LDH", "LDI", "LDJ", "LDK", "LDL", "LDM", "LDN", "LDO", "LDP", "LDQ", "LDR", "LDS", "LDT", "LDU", "LDV", "LDW", "LDX", "LDY", "LDZ", 
                    "MDA", "MDB", "MDC", "MDD", "MDE", "MDF", "MDG", "MDH", "MDI", "MDJ", "MDK", "MDL", "MDM", "MDN", "MDO", "MDP", "MDQ", "MDR", "MDS", "MDT", "MDU", "MDV", "MDW", "MDX", "MDY", "MDZ", 
                    "NDA", "NDB", "NDC", "NDD", "NDE", "NDF", "NDG", "NDH", "NDI", "NDJ", "NDK", "NDL", "NDM", "NDN", "NDO", "NDP", "NDQ", "NDR", "NDS", "NDT", "NDU", "NDV", "NDW", "NDX", "NDY", "NDZ", 
                    "ODA", "ODB", "ODC", "ODD", "ODE", "ODF", "ODG", "ODH", "ODI", "ODJ", "ODK", "ODL", "ODM", "ODN", "ODO", "ODP", "ODQ", "ODR", "ODS", "ODT", "ODU", "ODV", "ODW", "ODX", "ODY", "ODZ", 
                    "PDA", "PDB", "PDC", "PDD", "PDE", "PDF", "PDG", "PDH", "PDI", "PDJ", "PDK", "PDL", "PDM", "PDN", "PDO", "PDP", "PDQ", "PDR", "PDS", "PDT", "PDU", "PDV", "PDW", "PDX", "PDY", "PDZ", 
                    "QDA", "QDB", "QDC", "QDD", "QDE", "QDF", "QDG", "QDH", "QDI", "QDJ", "QDK", "QDL", "QDM", "QDN", "QDO", "QDP", "QDQ", "QDR", "QDS", "QDT", "QDU", "QDV", "QDW", "QDX", "QDY", "QDZ", 
                    "RDA", "RDB", "RDC", "RDD", "RDE", "RDF", "RDG", "RDH", "RDI", "RDJ", "RDK", "RDL", "RDM", "RDN", "RDO", "RDP", "RDQ", "RDR", "RDS", "RDT", "RDU", "RDV", "RDW", "RDX", "RDY", "RDZ", 
                    "SDA", "SDB", "SDC", "SDD", "SDE", "SDF", "SDG", "SDH", "SDI", "SDJ", "SDK", "SDL", "SDM", "SDN", "SDO", "SDP", "SDQ", "SDR", "SDS", "SDT", "SDU", "SDV", "SDW", "SDX", "SDY", "SDZ", 
                    "TDA", "TDB", "TDC", "TDD", "TDE", "TDF", "TDG", "TDH", "TDI", "TDJ", "TDK", "TDL", "TDM", "TDN", "TDO", "TDP", "TDQ", "TDR", "TDS", "TDT", "TDU", "TDV", "TDW", "TDX", "TDY", "TDZ", 
                    "UDA", "UDB", "UDC", "UDD", "UDE", "UDF", "UDG", "UDH", "UDI", "UDJ", "UDK", "UDL", "UDM", "UDN", "UDO", "UDP", "UDQ", "UDR", "UDS", "UDT", "UDU", "UDV", "UDW", "UDX", "UDY", "UDZ", 
                    "VDA", "VDB", "VDC", "VDD", "VDE", "VDF", "VDG", "VDH", "VDI", "VDJ", "VDK", "VDL", "VDM", "VDN", "VDO", "VDP", "VDQ", "VDR", "VDS", "VDT", "VDU", "VDV", "VDW", "VDX", "VDY", "VDZ", 
                    "WDA", "WDB", "WDC", "WDD", "WDE", "WDF", "WDG", "WDH", "WDI", "WDJ", "WDK", "WDL", "WDM", "WDN", "WDO", "WDP", "WDQ", "WDR", "WDS", "WDT", "WDU", "WDV", "WDW", "WDX", "WDY", "WDZ", 
                    "XDA", "XDB", "XDC", "XDD", "XDE", "XDF", "XDG", "XDH", "XDI", "XDJ", "XDK", "XDL", "XDM", "XDN", "XDO", "XDP", "XDQ", "XDR", "XDS", "XDT", "XDU", "XDV", "XDW", "XDX", "XDY", "XDZ", 
                    "YDA", "YDB", "YDC", "YDD", "YDE", "YDF", "YDG", "YDH", "YDI", "YDJ", "YDK", "YDL", "YDM", "YDN", "YDO", "YDP", "YDQ", "YDR", "YDS", "YDT", "YDU", "YDV", "YDW", "YDX", "YDY", "YDZ", 
                    "ZDA", "ZDB", "ZDC", "ZDD", "ZDE", "ZDF", "ZDG", "ZDH", "ZDI", "ZDJ", "ZDK", "ZDL", "ZDM", "ZDN", "ZDO", "ZDP", "ZDQ", "ZDR", "ZDS", "ZDT", "ZDU", "ZDV", "ZDW", "ZDX", "ZDY", "ZDZ"

                };

            while (fin <= limite)
            {
                //traer la lista de piezas
                var listObras = dbx.Obras.Where(a => a.ObraID > inicio && a.ObraID <= fin).Select(a => new { a.ObraID, a.TipoObraID, a.AntID, a.Clave, a.UbicacionID }).ToList();

                foreach (var obra in listObras)
                {
                    var anonimaOtraPieza = listaAnt.Where(a => a.id_pieza == obra.AntID).ToList();
                    if (anonimaOtraPieza.Count > 0)
                    {
                        foreach (var aop in anonimaOtraPieza)
                        {
                            //buscar tipoPieza
                            var tipoPieza = dbx.TipoPiezas.Where(a => a.TipoObraID == obra.TipoObraID && a.AntID == aop.TipoPieza_Clave).Select(a => new { a.TipoObraID, a.TipoPiezaID, a.Clave, a.AntID }).FirstOrDefault();
                            if (tipoPieza == null)
                            {
                                int orden = dbx.TipoPiezas.Where(a => a.TipoObraID == obra.TipoObraID).Count() + 1;
                                string clave = letras[orden - 1];
                                string nombre = "Pendiente por definir";

                                if (aop.TipoPieza_Clave != "0")
                                {
                                    nombre = aop.TipoPieza_Descripcion;
                                }

                                //crear el TipoPieza con AntID = antID
                                TipoPieza tp = new TipoPieza()
                                {
                                    Nombre = nombre,
                                    Clave = clave,
                                    Orden = orden,
                                    Status = true,
                                    TipoObraID = obra.TipoObraID,
                                    EsMaestra = false,
                                    AntID = aop.TipoPieza_Clave
                                };

                                dbx.TipoPiezas.Add(tp);
                                dbx.SaveChanges();
                                tipoPieza = dbx.TipoPiezas.Where(a => a.TipoObraID == obra.TipoObraID && a.AntID == aop.TipoPieza_Clave).Select(a => new { a.TipoObraID, a.TipoPiezaID, a.Clave, a.AntID }).FirstOrDefault();
                            }

                            //ya tenemos el TipoPieza
                            //buscar si ya existe la Pieza con el TipoPieza si no crearlo
                            string antID = aop.id_pieza + "-" + aop.Sub_pieza;
                            var pieza = dbx.Piezas.Where(a => a.AntID == antID).Select(a => new { a.PiezaID, a.AntID, a.Clave, a.ObraID, a.TipoPiezaID }).FirstOrDefault();
                            if (pieza == null)
                            {
                                var clave = obra.Clave + "-" + tipoPieza.Clave;
                                //CONTAR PIEZAS QUE EXISTEN EN ESTA OBRA CON ESE TIPO DE PIEZA
                                int totalPiezas = dbx.Piezas.Where(a => a.TipoPiezaID == tipoPieza.TipoPiezaID && a.ObraID == obra.ObraID).Count();

                                if (totalPiezas != 0)
                                {
                                    clave = clave + (totalPiezas + 1);
                                }

                                Pieza p = new Pieza()
                                {
                                    ObraID = obra.ObraID,
                                    Clave = clave,
                                    TipoPiezaID = tipoPieza.TipoPiezaID,
                                    UbicacionID = obra.UbicacionID,
                                    FechaRegistro = DateTime.Now,
                                    Status = true,
                                    AntID = antID
                                };

                                dbx.Piezas.Add(p);
                                dbx.SaveChanges();
                                //registrar todo lo que tenga la tabla
                                //TecnicaPieza - MatriculaTecnica_Clave
                                var tecnica = listaTecnicas.FirstOrDefault(a => a.AntID == aop.MatriculaTecnica_Clave);
                                if (tecnica != null)
                                {
                                    TecnicaPieza tecPieza = new TecnicaPieza()
                                    {
                                        PiezaID = p.PiezaID,
                                        TecnicaID = tecnica.TecnicaID,
                                        Status = true
                                    };
                                    dbx.TecnicaPiezas.Add(tecPieza);
                                }

                                //Titulo - TipoPieza_Descripcion
                                if (!string.IsNullOrWhiteSpace(aop.TipoPieza_Descripcion))
                                {
                                    Int64 attID_Titulo = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAtt_Titulo && a.TipoPiezaID == tipoPieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();
                                    if (attID_Titulo == 0)
                                    {
                                        Atributo attTitulo = new Atributo()
                                        {
                                            TipoAtributoID = tipoAtt_Titulo,
                                            TipoPiezaID = tipoPieza.TipoPiezaID,
                                            Status = true,
                                            EnFichaBasica = true,
                                            Orden = 1,
                                            Requerido = false
                                        };
                                        dbx.Atributos.Add(attTitulo);
                                        dbx.SaveChanges();
                                        attID_Titulo = attTitulo.AtributoID;
                                    }
                                    AtributoPieza attPiezaTitulo = new AtributoPieza()
                                    {
                                        PiezaID = p.PiezaID,
                                        AtributoID = attID_Titulo,
                                        Valor = aop.TipoPieza_Descripcion
                                    };
                                    dbx.AtributoPiezas.Add(attPiezaTitulo);
                                }

                                //Descripcion - Descripcion
                                if (!string.IsNullOrWhiteSpace(aop.Descripcion))
                                {
                                    Int64 attID_Descripcion = dbx.Atributos.Where(a => a.TipoAtributoID == tipoAtt_Descripcion && a.TipoPiezaID == tipoPieza.TipoPiezaID).Select(a => a.AtributoID).FirstOrDefault();
                                    if (attID_Descripcion == 0)
                                    {
                                        Atributo attDescripcion = new Atributo()
                                        {
                                            TipoAtributoID = tipoAtt_Descripcion,
                                            TipoPiezaID = tipoPieza.TipoPiezaID,
                                            Status = true,
                                            EnFichaBasica = true,
                                            Orden = 1,
                                            Requerido = false
                                        };
                                        dbx.Atributos.Add(attDescripcion);
                                        dbx.SaveChanges();
                                        attID_Descripcion = attDescripcion.AtributoID;
                                    }
                                    AtributoPieza attPiezaDescripcion = new AtributoPieza()
                                    {
                                        PiezaID = p.PiezaID,
                                        AtributoID = attID_Descripcion,
                                        Valor = aop.Descripcion
                                    };
                                    dbx.AtributoPiezas.Add(attPiezaDescripcion);
                                }


                                //Medidas - Sin cuadro o marco - Alto, Ancho, Fondo, Diametro,Diametro2
                                string nombreMedida = "Sin marco ó base";
                                Int64 tipoMed = db.TipoMedidas.Where(a => a.Nombre == nombreMedida).Select(a => a.TipoMedidaID).FirstOrDefault();

                                Medida med = new Medida()
                                {
                                    Ancho = aop.Ancho,
                                    Diametro = aop.Diametro,
                                    Largo = aop.Alto,
                                    PiezaID = p.PiezaID,
                                    Profundidad = aop.Fondo,
                                    Status = true,
                                    TipoMedidaID = tipoMed,
                                };

                                dbx.Medidas.Add(med);
                                pieza = dbx.Piezas.Where(a => a.AntID == antID).Select(a => new { a.PiezaID, a.AntID, a.Clave, a.ObraID, a.TipoPiezaID }).FirstOrDefault();
                            }


                            //agregar la foto
                            //ImagenPieza - ruta_imagen
                            if (!string.IsNullOrWhiteSpace(aop.ruta_imagen))
                            {
                                ImagenPieza imgPieza = new ImagenPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    Orden = aop.nSubIndex,
                                    Titulo = aop.TipoPieza_Descripcion,
                                    ImgNombre = aop.ruta_imagen,
                                    Status = true,
                                };

                                dbx.ImagenPiezas.Add(imgPieza);
                            }
                        }
                    }
                }

                dbx.SaveChanges();

                dbx.Dispose();
                dbx = new RecordFCSContext();
                dbx.Configuration.AutoDetectChangesEnabled = false;

                inicio = fin;
                fin = fin + total;
            }

            //}
            //catch (Exception)
            //{

            //    throw;
            //}


            return View();
        }

        //---------------------------------------------------------------------
        //AGREGAR ATRIBUTOS A TIPOOBRAS
        public ActionResult ModificarTiposObrasAtt()
        {
            RecordFCSContext dbx = new RecordFCSContext();
            var listaTipoAtt = db.TipoAtributos.OrderBy(a => a.BuscadorOrden).Select(a => new { a.TipoAtributoID, a.EsLista, a.BuscadorOrden, a.AntNombre }).ToList();
            var listaTipoPiezas = db.TipoPiezas.Select(a => new { a.TipoPiezaID, a.TipoObraID, a.Nombre, a.EsMaestra, a.Clave, a.AntID }).ToList();


            foreach (var tipoPieza in listaTipoPiezas)
            {
                foreach (var tipoAtt in listaTipoAtt)
                {
                    bool esNuevo = false;
                    bool guardar = false;
                    var att = dbx.Atributos.SingleOrDefault(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.TipoPiezaID == tipoPieza.TipoPiezaID);

                    if (att == null)
                    {
                        esNuevo = true;
                        att = new Atributo()
                        {
                            TipoAtributoID = tipoAtt.TipoAtributoID,
                            TipoPiezaID = tipoPieza.TipoPiezaID
                        };
                    }

                    att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);

                    switch (tipoAtt.AntNombre)
                    {
                        case "m_pieza_foto":
                            att.Requerido = false;
                            att.EnFichaBasica = true;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;
                        case "clave2":
                            att.Requerido = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;

                            if (tipoPieza.EsMaestra)
                            {
                                att.EnFichaBasica = false;
                            }
                            else
                            {
                                att.EnFichaBasica = true;
                            }

                            guardar = true;
                            break;
                        case "TipoPieza_Clave":
                            att.Requerido = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;

                            if (tipoPieza.EsMaestra)
                            {
                                att.EnFichaBasica = false;
                            }
                            else
                            {
                                att.EnFichaBasica = true;
                            }

                            guardar = true;
                            break;
                        case "fecha_registro":
                            if (!tipoPieza.EsMaestra)
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = true;
                                guardar = true;
                            }
                            break;
                        case "estatus_pieza":
                            if (!tipoPieza.EsMaestra)
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = true;
                                guardar = true;
                            }
                            break;

                        case "titulo":
                            att.Requerido = true;
                            att.EnFichaBasica = true;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;

                        case "m_pieza_dimensiones":
                            att.Requerido = false;
                            att.EnFichaBasica = true;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;
                        case "descripcion":
                            att.Requerido = true;
                            att.EnFichaBasica = true;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;
                        case "m_guion_det":
                            att.Requerido = false;
                            att.EnFichaBasica = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;

                        case "EdoConservacion_Clave":
                            att.Requerido = false;
                            att.EnFichaBasica = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;

                        case "Ubicacion_Clave (OBRA)":
                            att.Requerido = false;
                            att.EnFichaBasica = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;

                        case "observaciones":
                            att.Requerido = false;
                            att.EnFichaBasica = false;
                            att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                            att.Status = true;
                            guardar = true;
                            break;

                        case "estatus":
                            if (tipoPieza.EsMaestra)
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = true;
                                guardar = true;
                            }
                            else
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = false;
                                guardar = true;
                            }
                            break;

                                                  

                                                  
                        case "TipoObjeto_Clave":
                        case "clave1":
                        case "m_pieza_coleccion":
                        case "Autor_Clave":
                        case "FechaEjecucion_Clave":
                        case "m_cats":
                        case "MatriculaTecnica_Clave":
                        case "fecha_registro_ORI":
                        case "catTipoAdquisicion":
                        case "FormaAdquisicion_Clave":
                        case "Propietario_Clave":

                            if (tipoPieza.EsMaestra)
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = true;
                                guardar = true;
                            }

                            break;

                        default:
                            if (!esNuevo)
                            {
                                att.Requerido = false;
                                att.EnFichaBasica = false;
                                att.Orden = Convert.ToInt32(tipoAtt.BuscadorOrden);
                                att.Status = true;
                                guardar = true;
                            }
                            break;

                    }

                    if (guardar)
                    {
                        if (esNuevo)
                        {
                            //save
                            dbx.Atributos.Add(att);
                        }
                        else
                        {
                            //update
                            dbx.Entry(att).State = EntityState.Modified;
                        }
                    }

 


                }
                dbx.SaveChanges();
                dbx.Dispose();
                dbx = new RecordFCSContext();
            }
          return View();
        }



    }


}