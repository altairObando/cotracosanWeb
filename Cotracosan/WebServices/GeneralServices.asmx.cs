using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Services;
using Cotracosan.WebServices.Models;
using Cotracosan.Models.Cotracosan;

namespace Cotracosan.WebServices
{
    /// <summary>
    /// Descripción breve de GeneralServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class GeneralServices : System.Web.Services.WebService
    {
        // Instanca de base de datos.
        private Context db = new Context();
        // Seccion para los vehiculos
        #region Vehiculos
        /// <summary>
        /// Este metodo permite obtener todos los vehiculos que le pertenecen a un socio
        /// </summary>
        /// <param name="socioId">Llave primaria de la entidad socio</param>
        /// <returns>Lista generica de vehiculos ó una lista vacia</returns>
        [WebMethod]
        public List<VehiculosASMX> TodosLosVehiculosPorSocio(int socioId)
        {
            // Obtenemos los vehiculos
            var data = db.Vehiculos
                .Where(x => x.SocioId == socioId && x.Estado) // Consultamos todos los que estan para el socio
                .Select(y => new VehiculosASMX // Realizamos una proyeccion para evitar referencias circulares
                {                              // en las busquedas por navegacion.
                    Id = y.Id,
                    Placa = y.Placa
                })
                .ToList<VehiculosASMX>();
            return data;
        }

        [WebMethod]
        public decimal IngresoPorVehiculoPorFecha(int vehiculoId, DateTime fechaConsulta)
        {
            // Obtener el vehiculo
            var vehiculo = db.Vehiculos.Find(vehiculoId);
            // si el vehiculo existe
            if (vehiculo != null)
            {
                decimal montoRecaudado = vehiculo // Usando el vehiculo seleccionado
                    .Carreras // Buscamos las careras
                    .Where(x => x.FechaDeCarrera.Equals(fechaConsulta)) // Filtramos por fecha
                    .Sum(y => y.MontoRecaudado); // Hacemos la suma de lo recaudado.
                return montoRecaudado;
            }
            else
            {
                return 0;
            }
        }

        [WebMethod]
        public List<VehiculosMontosSocio> TotalRecaudadoPorVehiculos(int socioId, string fechaInicio, string fechaFin)
        {
            string sqlQuery = string.Format(@"SELECT v.Id, v.Placa, SUM(c.MontoRecaudado) as MontoRecaudado FROM Vehiculos v
                                INNER JOIN Carreras c ON v.Id = c.VehiculoId
                                WHERE v.SocioId = {0} AND (c.FechaDeCarrera >= '{1}' AND c.FechaDeCarrera <= '{2}' )
                                GROUP BY v.Id, v.Placa", socioId, fechaInicio, fechaFin);
            List<VehiculosMontosSocio> data = db.Database.SqlQuery<VehiculosMontosSocio>(sqlQuery).ToList();
            
            //return montoRecaudado;
            return data;
        }
        #endregion

        // Seccion para las carreras
        #region Carreras
        // Lista de Careras realizadas
        [WebMethod]
        public List<CarrerasASMX> CarrerasRealizadas(DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener las carreras usando linq.
            var data = db.Carreras
                .Include(v => v.Conductores)
                .Include(w => w.LugaresFinalesDelosRecorridos)
                .Include(x => x.Penalizaciones)
                .Include(y => y.Turnos)
                .Include(z => z.Vehiculos)
                .Where(a => a.FechaDeCarrera >= fechaInicio && a.FechaDeCarrera <= fechaFin && a.Estado)
                .Select(b => new CarrerasASMX
                {
                    Id = b.Id,
                    CodigoCarrera = b.CodigoCarrera,
                    FechaDeCarrera = b.FechaDeCarrera,
                    HoraRealDeLlegada = b.HoraRealDeLlegada,
                    Conductor = b.Conductores.Nombres + " " + b.Conductores.Apellido1Conductor,
                    LugarFinalDeRecorrido = b.LugaresFinalesDelosRecorridos.NombreDeLugar,
                    Vehiculo = b.Vehiculos.Placa,           
                    Turno = b.Turnos.HoraDeSalida + " - " + b.Turnos.HoraDeLlegada,
                    MontoRecaudado= b.MontoRecaudado,
                    Multa = b.Multa,
                    Penalizacion = b.Penalizaciones.CodigoPenalizacion,
                    CarreraAnulada = b.CarreraAnulada,
                    Estado = b.Estado   
                })
                .ToList();
            return null;
        }
        #endregion

    }
}
