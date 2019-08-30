using Cotracosan.Models.Cotracosan;
using Cotracosan.Models.WebModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace Cotracosan
{
    /// <summary>
    /// Descripción breve de WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        private Context db = new Context();
        #region ApiAbonos
        [WebMethod]
        public WebResult AddAbono(DateTime FechaDeAbono, decimal MontoDeAbono, int CreditoId)
        {
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    var abono = new Abonos
                    {
                        CodigoAbono = "ABO",
                        CreditoId = CreditoId,
                        Creditos = null,
                        Estado = true,
                        FechaDeAbono = FechaDeAbono,
                        Id = 0,
                        MontoDeAbono = MontoDeAbono
                    };
                    // Guardar el abono.
                    db.Abonos.Add(abono);
                    bool guardado = db.SaveChanges() > 0;
                    bool codigoAbono = false;
                    if (guardado)
                    {
                        // Actualizamos el codigo del abono con el nuevo Id
                        db.Entry(abono).State = EntityState.Detached; // separamos del que tiene seguimiento y buscamos el nuevo para evitar errores.
                        abono = db.Abonos.Find(abono.Id);
                        abono.CodigoAbono = "ABO-" + abono.Id;
                        db.Entry(abono).State = EntityState.Modified;
                        codigoAbono = db.SaveChanges() > 0;
                    }
                    if (guardado && codigoAbono)
                        transact.Commit();

                    return new WebResult
                    {
                        Completado = guardado && codigoAbono,
                        Mensaje = guardado && codigoAbono ? "Se ha registrado el abono." :
                                  !guardado && !codigoAbono ? "Error al guardar el abono" :
                                  guardado && !codigoAbono ? "Se ha guardado el abono, pero no se logro generar el codigo" :
                                  "error general."
                    };
                }
                catch (Exception e)
                {
                    transact.Rollback();
                    return new WebResult { Completado = false, Mensaje = e.Message };
                }
            }
        }
        [WebMethod]
        public List<AbonosWS> AbonosPorCredito(int creditoId)
        {
            var abonos = db.Abonos
                .Where(c => c.CreditoId.Equals(creditoId))
                .Include(c => c.Creditos)
                .Select( x => new AbonosWS {
                    CodigoAbono = x.CodigoAbono,
                    CreditoId = x.CreditoId,
                    FechaDeAbono =x.FechaDeAbono, 
                    Id = x.Id,
                    MontoDeAbono = x.MontoDeAbono
                }
                ).OrderByDescending(a => a.FechaDeAbono)
                .ToList();
            return abonos;
        }
        // Eliminar Abono
        [WebMethod]
        public WebResult DeleteAbono(int abonoId)
        {
            // Buscar el abono
            var abono = db.Abonos.Find(abonoId);
            if (abono == null)
                return new WebResult { Completado = false, Mensaje = "No existe ningun abono con el id: " + abonoId };
            abono.Estado = false;
            db.Entry(abono).State = EntityState.Modified;

            bool guardado = db.SaveChanges() > 0;
            return new
             Cotracosan.WebResult
            {
                Completado = guardado,
                Mensaje = guardado ? "Se ha eliminado el abono" : "No se ha logrado eliminar el abono, error del servidor"
            };
        }
        #endregion
        #region Articulos
        [WebMethod]
        public List<GastosPorArticulo> GetGastosPorArticulos(string fechaInicio, string fechaFin )
        {
            string sql = @"SELECT A.CodigoDeArticulo, A.DescripcionDeArticulo, SUM(D.Cantidad * A.Precio ) as Gasto
                           FROM Articulos A INNER JOIN
                           DetallesDeCreditos D ON A.Id = D.ArticuloId INNER JOIN
	                       Creditos C on C.Id = D.CreditoId
                           ";
            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                sql += string.Format(" WHERE C.FechaDeCredito between '{0}' AND '{1}' ", fechaInicio, fechaFin);
            }
            try
            {
                var result = db.Database.SqlQuery<GastosPorArticulo>(
                    sql + @" GROUP BY A.DescripcionDeArticulo, A.CodigoDeArticulo
                             ORDER BY Gasto desc").ToListAsync().Result;
                return result;
            }
            catch (Exception)
            {
                return new List<GastosPorArticulo>();
            }

        }
        [WebMethod]
        public List<ArticulosSW> GetArticulos(string parametro)
        {
            List<Articulos> articulos;
            if (!string.IsNullOrEmpty(parametro))
                articulos = db.Articulos.Where(x => x.Estado &&
                x.DescripcionDeArticulo.ToUpper().Contains(parametro.ToUpper())
                ).ToListAsync().Result;
            else
                articulos = db.Articulos.Where(x => x.Estado).ToListAsync().Result;
            var result = (from i in articulos
                         orderby i.DescripcionDeArticulo
                         select new ArticulosSW
                         {
                             Id = i.Id,
                             Codigo = i.CodigoDeArticulo,
                             Descripcion = i.DescripcionDeArticulo,
                             Precio = i.Precio
                         }).ToList();
            return result;
        }
        #endregion
        #region ApiCarreras
        [WebMethod]
        public List<CarrerasSW> GetCarrerasPorVehiculo(int VehiculoId, DateTime FechaInicial, DateTime FechaFinal, int Max)
        {
            List<CarrerasSW> carreras = null;
            // Consultar entre fechas
            var data = db.Carreras.Where(
                x => x.VehiculoId.Equals(VehiculoId) &&
                x.FechaDeCarrera.Date >= FechaInicial &&
                x.FechaDeCarrera.Date <= FechaFinal &&
                x.Estado
                ).OrderByDescending(z => z.FechaDeCarrera)
                .ToList();
            if (Max > 0)
                data = data.Take(Max).ToList();

            carreras = data.Select(y => new CarrerasSW
            {
                CarreraAnulada = y.CarreraAnulada,
                CodigoCarrera = y.CodigoCarrera,
                Conductor = y.Conductores.NombreCompleto,
                Estado = y.Estado,
                FechaDeCarrera = y.FechaDeCarrera,
                HoraRealDeLlegada = y.HoraRealDeLlegada,
                Id = y.Id,
                LugarFinalDeRecorrido = y.LugaresFinalesDelosRecorridos.NombreDeLugar,
                MontoRecaudado = y.MontoRecaudado,
                Multa = y.Multa,
                Turno = y.Turnos.HorasTurno,
                Vehiculo = y.Vehiculos.Placa
            }).ToList();
            return carreras;
        }
        [WebMethod]
        public WebResult DeleteCarrera(int CarreraId)
        {
            var carrera = db.Carreras.Find(CarreraId);
            if (carrera == null)
                return new WebResult { Completado = false, Mensaje = "No existe ninguna carrera con el id: " + CarreraId };
            carrera.CarreraAnulada = true;
            db.Entry(carrera).State = EntityState.Modified;
            bool eliminado = db.SaveChanges() > 0;
            return new WebResult
            {
                Completado = eliminado,
                Mensaje = eliminado ? "Se ha eliminado la carrera" : "No se ha eliminado la carrera"
            };
        }
        #endregion
    }


    public class WebResult
    {
        public string Mensaje { get; set; }
        public bool Completado { get; set; }
    }

    public class GastosPorArticulo
    {
        public string CodigoDeArticulo { get; set; }
        public string DescripcionDeArticulo { get; set; }
        public decimal Gasto { get; set; }
    }
}
