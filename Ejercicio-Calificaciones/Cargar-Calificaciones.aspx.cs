using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Services;
using Calificaciones.Dto;

public partial class Cargar_Calificaciones : System.Web.UI.Page
{
    #region EVENTOS DE LA PÁGINA

    //Nos indica la ruta del archivo que ocupamos abrir.
    static string Ruta = "";

    //Nos indica la extensión del archivo del excel para abrirlo de manera satisfactoria.
    static string Extension = "";

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region EVENTOS DE LOS CONTROLES

    //Cargar y guarda el archivo de excel con las calificaciones en una ruta especifica.
    protected void btnSubir_Click(object sender, EventArgs e)
    {
        if (fuArchivo.HasFile)
        {
            try
            {
                string filename = Path.GetFileName(fuArchivo.FileName);
                fuArchivo.SaveAs(Server.MapPath("Archivos/") + filename);
                lblStatus.Text = "Archivo subido con éxito, ahora puede consultar su información.";
                Ruta = Server.MapPath("Archivos/") + filename;
                Extension = Path.GetExtension(fuArchivo.PostedFile.FileName);
                btnGenerarInformacion.Visible = true;

            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al intentar cargar el archivo, intente de nuevo por favor.";
                btnGenerarInformacion.Visible = false;
            }
        }
        else
        {
            lblStatus.Text = "Favor de seleccionar un archivo.";
            btnGenerarInformacion.Visible = false;
        }

    }
    #endregion

    #region MÉTODOS DEL MÓDULO

    /// <summary>
    /// Manda a crear un datatable y con este una lista con los componentes necesarios para formar una gráfica de barras
    /// acerca de los alumnos con respecto a sus calificaciones.
    /// </summary>
    /// <returns>Regresa una lista con la información necesaria para generar el JSON que llenará la gráfica.</returns>
    private List<ComponenteGraficaBarra> ObtenerListaGraficaInformacion()
    {
        try
        {
            DataTable dtInformacion = CrearDatatable("Informacion");
            List<ComponenteGraficaBarra> lstInformacionPorCalificacion = GenerarComponentesGraficaBarra(dtInformacion);
            return lstInformacionPorCalificacion;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Manda a crear un datatable y con este una lista con los componentes necesarios para formar una gráfica lineal
    /// con los promedios de cada grado.
    /// </summary>
    /// <returns>Regresa una lista con la información necesaria para generar el JSON que llenará la gráfica.</returns>
    private List<ComponenteGraficaLineal> ObtenerListaGraficaPromedio()
    {
        try
        {
            DataTable dtInformacion = CrearDatatable("Promedio");
            List<ComponenteGraficaLineal> lstInformacionPorCalificacion = GenerarComponentesGraficaLineal(dtInformacion);
            return lstInformacionPorCalificacion;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Genera la lista con la información necesaria para armar el JSON que llenará la gráfica de barras.
    /// </summary>
    /// <param name="dtGrafica">Datatable que contiene los datos que se ocupan para generar la lista.</param>
    /// <returns>Regresa la lista que posteriormente haremos JSON con los componentes para crear la gráfica.</returns>
    private List<ComponenteGraficaBarra> GenerarComponentesGraficaBarra(DataTable dtGrafica)
    {
        var random = new Random();
        List<ComponenteGraficaBarra> list = new List<ComponenteGraficaBarra>();
        list = (from DataRow row in dtGrafica.Rows

                select new ComponenteGraficaBarra()
                {
                    EjeY = Convert.ToInt32(row["Cantidad"]),
                    EjeX = Convert.ToDecimal(row["Calificacion"]),
                    Color = String.Format("#{0:X6}", random.Next(0x1000000))
                }).ToList();

        return list;
    }

    /// <summary>
    /// Genera la lista con la información necesaria para armar el JSON que llenará la gráfica líneal.
    /// </summary>
    /// <param name="dtGrafica">Datatable que contiene los datos que se ocupan para generar la lista.</param>
    /// <returns>Regresa la lista que posteriormente haremos JSON con los componentes para crear la gráfica.</returns>
    private List<ComponenteGraficaLineal> GenerarComponentesGraficaLineal(DataTable dtGrafica)
    {
        List<ComponenteGraficaLineal> list = new List<ComponenteGraficaLineal>();
        list = (from DataRow row in dtGrafica.Rows

                select new ComponenteGraficaLineal()
                {
                    EjeY = Convert.ToDecimal(row["Promedio"]),
                    EjeX = "Grado " + Convert.ToString(row["Grado"])
                }).ToList();

        return list;
    }

    /// <summary>
    /// Manda a crear un datatable con la información de un alumno y lo regresa transformado en un objeto de tipo Estudiante.
    /// </summary>
    /// <param name="tipoConsulta">Puede ser MejorCalificacion o PeorCalificacion, 
    /// nos indica el tipo de dato que queremos 
    /// recuperar en el datatable</param>
    /// <returns>Regresa un objeto de tipo Estudiante que posteriormente usaremos para imprimirlo en pantalla.</returns>
    private Estudiante ObtenerEstudiante(string tipoConsulta)
    {
        try
        {
            DataTable dtInformacion = CrearDatatable(tipoConsulta);
            Estudiante estudiante = GenerarEstudiante(dtInformacion);
            return estudiante;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Crea y regresa un objeto de tipo Estudiante a partir de un datatable.
    /// </summary>
    /// <param name="dt">Datatable con la información del estudiante con la que llenaremos el objeto.</param>
    /// <returns></returns>
    private Estudiante GenerarEstudiante(DataTable dt)
    {
        DataRow row = dt.Rows[0];

        Estudiante estudiante = new Estudiante()
        {
            Nombres = row[0].ToString(),
            ApellidoPaterno = row[1].ToString(),
            ApellidoMaterno = row[2].ToString()
        };

        return estudiante;
    }

    /// <summary>
    /// Obtiene un datatable con la información de todos los estudiantes y obtiene el promedio de las calificaciones. 
    /// El valor es redondeado a dos decimales en el archivo de JavaScript.
    /// </summary>
    /// <returns>Variable con el promedio de las calificaciones de todos los estudiantes.</returns>
    private object ObtenerValorPromedio()
    {
        try
        {
            DataTable dtInformacion = CrearDatatable("PromedioCalificaciones");
            double promedioCalificaciones = dtInformacion.Rows.Cast<DataRow>().Select(r => r["Calificacion"]).OfType<double>().Where(v => v >= 0).Average();
            return promedioCalificaciones;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Crea la cadena de conexión y hace lo necesario para abrir el archivo de excel con las calificaciones 
    /// y regresa un datatable con la información necesaria dependiendo de los parametros que haya recibido.
    /// </summary>
    /// <param name="tipoConsulta">Define cuál será el comando de texto que se ejecutará en el archivo de 
    /// excel para obtener la información que necesitamos.</param>
    /// <returns>Regresa un datatable con la información solicitada.</returns>
    private DataTable CrearDatatable(string tipoConsulta)
    {
        string conStr = "";
        switch (Extension)
        {
            case ".xls": //Excel 97-03
                conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"]
                         .ConnectionString;
                break;
            case ".xlsx": //Excel 07
                conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"]
                          .ConnectionString;
                break;
        }
        conStr = String.Format(conStr, Ruta, "Yes");
        OleDbConnection connExcel = new OleDbConnection(conStr);
        OleDbCommand cmdExcel = new OleDbCommand();
        OleDbDataAdapter oda = new OleDbDataAdapter();
        DataTable dtInformacion = new DataTable();
        cmdExcel.Connection = connExcel;

        try
        {
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();


            switch (tipoConsulta)
            {
                case "Informacion":
                    cmdExcel.CommandText = "SELECT COUNT(Calificacion) AS Cantidad, Calificacion FROM [" + SheetName + "] GROUP BY Calificacion";
                    break;
                case "Promedio":
                    cmdExcel.CommandText = "SELECT Grado, AVG(Calificacion) AS Promedio FROM [" + SheetName + "] GROUP BY Grado";
                    break;
                case "MejorCalificacion":
                    cmdExcel.CommandText = "SELECT * FROM [" + SheetName + "] WHERE Calificacion = (SELECT MAX(Calificacion) FROM [" + SheetName + "] )";
                    break;
                case "PeorCalificacion":
                    cmdExcel.CommandText = "SELECT * FROM [" + SheetName + "] WHERE Calificacion = (SELECT MIN(Calificacion) FROM [" + SheetName + "] )";
                    break;
                case "PromedioCalificaciones":
                    cmdExcel.CommandText = "SELECT * FROM [" + SheetName + "]";
                    break;
            }
            oda.SelectCommand = cmdExcel;
            oda.Fill(dtInformacion);
            connExcel.Close();

            return dtInformacion;
        }
        catch (Exception ex)
        {
            connExcel.Close();
            return null;

        }


    }

    #endregion

    #region MÉTODOS WEB

    [WebMethod]
    public static string ObtenerGraficaInformacionAlumnos()
    {
        Cargar_Calificaciones cargarCalificaciones = new Cargar_Calificaciones();
        var informacionCalificacion = cargarCalificaciones.ObtenerListaGraficaInformacion();
        return new JavaScriptSerializer().Serialize(informacionCalificacion);
    }

    [WebMethod]
    public static string ObtenerGraficaPromedio()
    {
        Cargar_Calificaciones cargarCalificaciones = new Cargar_Calificaciones();
        var informacionCalificacion = cargarCalificaciones.ObtenerListaGraficaPromedio();

        return new JavaScriptSerializer().Serialize(informacionCalificacion);
    }

    [WebMethod]
    public static string ObtenerMayorCalificacion()
    {
        Cargar_Calificaciones cargarCalificaciones = new Cargar_Calificaciones();
        var mejorEstudiante = cargarCalificaciones.ObtenerEstudiante("MejorCalificacion");

        return new JavaScriptSerializer().Serialize(mejorEstudiante);

    }

    [WebMethod]
    public static string ObtenerPeorCalificacion()
    {
        Cargar_Calificaciones cargarCalificaciones = new Cargar_Calificaciones();
        var peorEstudiante = cargarCalificaciones.ObtenerEstudiante("PeorCalificacion");

        return new JavaScriptSerializer().Serialize(peorEstudiante);
    }

    [WebMethod]
    public static string ObtenerPromedioCalificaciones()
    {
        Cargar_Calificaciones cargarCalificaciones = new Cargar_Calificaciones();
        var promedio = cargarCalificaciones.ObtenerValorPromedio();

        return new JavaScriptSerializer().Serialize(promedio);
    }
    #endregion
}