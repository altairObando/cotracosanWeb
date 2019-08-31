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
    [WebService(Namespace = "http://cotracosan.somee.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        private Context db = new Context();
        #region ApiAbonos
        [WebMethod]
        public WebResult AgregarAbono(DateTime FechaDeAbono, decimal MontoDeAbono, int CreditoId)
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
        #region ApiCreditos
        [WebMethod]
        public List<CreditosSW> GetCreditos(int? idBus, int? idCredito)
        {
            List<Creditos> creditos = idBus == null ?
                                    db.Creditos.ToListAsync().Result :
                                    db.Creditos.Where(y => y.VehiculoId.Equals((int)idBus)).ToList();
            if (idCredito != null)
            {
                creditos = creditos.Where(x => x.Id == idCredito).ToList();
            }
            var result =
               (
                   from item in creditos
                   orderby item.FechaDeCredito descending
                   select new CreditosSW
                   {
                       Id = item.Id,
                       CodigoCredito = item.CodigoCredito,
                       Fecha = item.FechaDeCredito.ToShortDateString(),
                       MontoTotal = item.MontoTotal,
                       TotalAbonado = item.Abonos.Sum(x => x.MontoDeAbono),
                       NumeroAbonos = item.Abonos.Count(),
                       CreditoAnulado = item.CreditoAnulado,
                       EstadoDeCredito = item.EstadoDeCredito,
                       DetallesDeCreditos = item.DetallesDeCreditos.Select(
                           x => new DetallesDeCreditosSW
                           {
                               Id = x.Id,
                               ArticuloId = x.ArticuloId,
                               CodigoArticulo = x.Articulos.CodigoDeArticulo,
                               Articulo = x.Articulos.DescripcionDeArticulo,
                               Cantidad = x.Cantidad,
                               Precio = x.Articulos.Precio
                           }).ToList()
                   }
               ).ToList<CreditosSW>();
            return result;
        }
        [WebMethod]
        public List<CreditosSW> GetCreditosPendientesPorBus(int idBus, DateTime fechaInicio, DateTime fechaFin)
        {
            List<Creditos> creditos = db.Creditos
                .Where(
                x => x.VehiculoId.Equals(idBus) &&
                x.MontoTotal > x.Abonos.Where(y => y.Estado).Sum(a => a.MontoDeAbono))
                .Where(z => z.FechaDeCredito > fechaInicio && z.FechaDeCredito < fechaFin)
                .ToList();
            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new CreditosSW
                    {
                        Id = item.Id,
                        CodigoCredito = item.CodigoCredito,
                        Fecha = item.FechaDeCredito.ToShortDateString(),
                        MontoTotal = item.MontoTotal,
                        TotalAbonado = item.Abonos.Sum(x => x.MontoDeAbono),
                        NumeroAbonos = item.Abonos.Count(),
                        CreditoAnulado = item.CreditoAnulado,
                        EstadoDeCredito = item.EstadoDeCredito,
                        DetallesDeCreditos = item.DetallesDeCreditos.Select(
                            x => new DetallesDeCreditosSW
                            {
                                Id = x.Id,
                                Cantidad = x.Cantidad,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Precio = x.Articulos.Precio
                            }).ToList()
                    }
                ).ToList();
            return result;
        }
        [WebMethod]
        public List<CreditosSW> GetCreditosPendientesPorSocio(int id)
        {
            var creditos = db.Creditos
                .Include(x => x.Vehiculos)
                .Where(c => c.Vehiculos.SocioId == id)
                .Where(c =>
                c.MontoTotal > c.Abonos.Where(x => x.Estado).Sum(a => a.MontoDeAbono))
                .ToList();
            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new CreditosSW
                    {
                        Id = item.Id,
                        CodigoCredito = item.CodigoCredito,
                        Fecha = item.FechaDeCredito.ToShortDateString(),
                        MontoTotal = item.MontoTotal,
                        TotalAbonado = item.Abonos.Where(y => y.Estado).Sum(x => x.MontoDeAbono),
                        NumeroAbonos = item.Abonos.Count(),
                        CreditoAnulado = item.CreditoAnulado,
                        EstadoDeCredito = item.EstadoDeCredito,
                        DetallesDeCreditos = item.DetallesDeCreditos.Select(
                            x => new DetallesDeCreditosSW
                            {
                                Id = x.Id,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Cantidad = x.Cantidad,
                                Precio = x.Articulos.Precio
                            }).ToList()
                    }
                ).ToList();
            return result;
        }
        [WebMethod]
        public List<CreditosSW> GetUltimosCreditos(int? Maximo)
        {
            var creditos = db.Creditos
                .OrderByDescending(x => x.FechaDeCredito)
                .ToList();
            if (Maximo != null)
                creditos = creditos.Take((int)Maximo).ToList();
            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new CreditosSW
                    {
                        Id = item.Id,
                        CodigoCredito = item.CodigoCredito,
                        Fecha = item.FechaDeCredito.ToShortDateString(),
                        MontoTotal = item.MontoTotal,
                        TotalAbonado = item.Abonos.Sum(x => x.MontoDeAbono),
                        NumeroAbonos = item.Abonos.Count(),
                        CreditoAnulado = item.CreditoAnulado,
                        EstadoDeCredito = item.EstadoDeCredito,
                        DetallesDeCreditos = item.DetallesDeCreditos.Select(
                            x => new DetallesDeCreditosSW
                            {
                                Id = x.Id,
                                Cantidad = x.Cantidad,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Precio = x.Articulos.Precio
                            }).ToList()
                    }
                ).ToList();
            return result;
        }
        [WebMethod]
        public WebResult DeleteCredito(int creditoId)
        {
            var credito = db.Creditos.Find(creditoId);
            if (credito == null)
                return new WebResult { Completado = false, Mensaje = "No se ha encontrado el credito con el id: " + creditoId };
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    credito.CreditoAnulado = true;
                    db.Entry(credito).State = EntityState.Modified;
                    bool creditoAnulado = db.SaveChanges() > 0;
                    bool abonosAnulados = false;
                    foreach (var abono in credito.Abonos)
                    {
                        abono.Estado = false;
                        db.Entry(abono).State = EntityState.Modified;
                        abonosAnulados = db.SaveChanges() > 0;
                    }
                    if (creditoAnulado && abonosAnulados)
                        transact.Commit();
                    return new WebResult
                    {
                        Completado = creditoAnulado && abonosAnulados,
                        Mensaje = creditoAnulado && abonosAnulados ? "Se ha eliminado el credito solicitado"
                                      : creditoAnulado && !abonosAnulados ? "No se ha logrado eliminar los abonos del credito"
                                      : !creditoAnulado && abonosAnulados ? "No se ha logrado eliminar el credito"
                                      : "No se ha eliminado ningun registro del sistema remoto"
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
        public string GetCodigoNuevo()
        {
            var list = db.Creditos.ToList();
            int id = list.Count > 0 ? list.OrderByDescending(x => x.Id).First().Id : 0;
            return "CRED-" + (id + 1);
        }
        [WebMethod]
        public WebResult AgregarCredito(CreditosSW cred, List<DetallesDeCreditosSW> detallesw)
        {
            var creditos = new Creditos {
                CodigoCredito = cred.CodigoCredito,
                CreditoAnulado = false,
                EstadoDeCredito = true,
                FechaDeCredito = DateTime.Parse(cred.Fecha),
                Id = 0,
                MontoTotal = cred.MontoTotal,
                VehiculoId = cred.VehiculoId
            };
            bool gCredito = false; // se ha guardado el credito ? 
            bool gDetalle = false; // se ha guardado el detalle ?
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    creditos.EstadoDeCredito = true;
                    db.Creditos.Add(creditos);
                    gCredito = db.SaveChanges() > 0;
                    if (gCredito)
                    {
                        var detalle = detallesw.Select(z => new DetallesDeCreditos
                        {
                            ArticuloId = z.ArticuloId,
                            Cantidad =z.Cantidad,
                            CreditoId = creditos.Id,
                            Id = 0
                        }).ToList();
                        //detalle.ForEach(x => x.CreditoId = creditos.Id);
                        db.DetallesDeCreditos.AddRange(detalle);
                        gDetalle = db.SaveChanges() > 0;
                        // Guardamos la transaccion solo si se guardo el detalle
                        transact.Commit();
                    }
                }
                catch (Exception)
                {
                    transact.Rollback();
                }
            }
            string mensaje = gCredito && gDetalle ? "Credito Guardado Correctamente" :
                gCredito && !gDetalle ? "No se ha logrado guardar los articulos del credito" :
                !gCredito && gDetalle ? "No se ha podido guardar el credito" : "Error en la validacion del modelo de datos";
            return new WebResult { Completado = gCredito && gDetalle, Mensaje = mensaje };
        }
        #endregion
        #region ApiSocios
        [WebMethod]
        public List<SociosSW> GetAbonosPorSocio(int socioId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Filtrar todos los abonos por socioId
            var abonos = db.Abonos
                .Include(v => v.Creditos)
                .Include(w => w.Creditos.Vehiculos)
                .Include(x => x.Creditos.Vehiculos.Socios)
                .Where(y =>
                  y.Creditos.Vehiculos.SocioId.Equals(socioId) && // Seleccionar el socio
                  y.FechaDeAbono.Date >= fechaInicio &&
                  y.FechaDeAbono.Date <= fechaFin)             // Aplicar el filtro de las fechas.
                .ToList();

            // Proyeccion para evitar referencias circulares.
            // y solo seleccionar los datos requeridos.
            var result = (from i in abonos
                          orderby i.FechaDeAbono descending
                          select new SociosSW
                          {
                              IdAbono = i.Id,
                              CreditoId = i.CreditoId,
                              VehiculoId = i.Creditos.VehiculoId,
                              Placa = i.Creditos.Vehiculos.Placa,
                              FechaDeAbono = i.FechaDeAbono.ToShortDateString(),
                              CodigoAbono = i.CodigoAbono,
                              MontoDeAbono = i.MontoDeAbono,
                              AbonoAnulado = i.Estado
                          }).ToList();
            return result;
        }
        #endregion
        #region ApiVehiculos
        [WebMethod]
        public List<VehiculosSW> GetVehiculos()
        {
            var vehiculos = (from v in db.Vehiculos
                             where v.Estado
                             select new VehiculosSW
                             {
                                 Id = v.Id,
                                 SocioId = v.SocioId,
                                 Placa = v.Placa
                             }
                          ).ToList();
            return vehiculos;
        }
        [WebMethod]
        public List<VehiculosSW> GetVehiculosPorSocio(int socioId)
        {
            var vehiculos = (from vehiculo in db.Vehiculos
                             where vehiculo.SocioId == socioId && vehiculo.Estado
                             select new VehiculosSW
                             {
                                 Id = vehiculo.Id,
                                 Estado = vehiculo.Estado,
                                 Placa = vehiculo.Placa,
                                 SocioId = vehiculo.SocioId,
                             }
                          ).ToList();
            return vehiculos;
        }
        // 2. Cuánto dinero hizo un bus en un determinado día.
        [WebMethod]
        public decimal GetMontoRecaudado(int vehiculoId, DateTime fecha)
        {
            decimal totalRecaudado = 0;
            var bus = db.Vehiculos.Find(vehiculoId);
            if (bus != null)
            {
                totalRecaudado = bus.Carreras.Where(x => !x.CarreraAnulada && x.FechaDeCarrera.Equals(fecha)).Sum(m => m.MontoRecaudado);
            }
            return totalRecaudado;
        }
        [WebMethod]
        public List<MontoRecaudado> GetMontoTotalRecaudado(int socioId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener todas las carreras de los vehiculos de los socios en las fechas indicadas
            var carreras = db.Carreras
                .Include(v => v.Vehiculos)
                .Where(f => f.FechaDeCarrera >= fechaInicio && f.FechaDeCarrera <= fechaFin)
                .Where(v => v.Vehiculos.SocioId.Equals(socioId))
                .ToList();
            // Agrupar por vehiculo
            var result =
                (
                from carrera in carreras
                orderby carrera.VehiculoId ascending
                group carrera by carrera.VehiculoId into vehiculo
                select new MontoRecaudado
                {
                    Id = vehiculo.Key,
                    Monto = vehiculo.Sum(x => x.MontoRecaudado),
                    Placa = vehiculo.First().Vehiculos.Placa
                }
                ).ToList();
            return result;
        }
        [WebMethod]
        public ConsolidadoPorVehiculo GetConsolidado(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            decimal totalCarreras = 0, totalCreditos = 0, totalAbonos = 0;
            totalCarreras = db.Carreras.Where(x => (x.FechaDeCarrera >= fechaInicio && x.FechaDeCarrera <= fechaFin) && !x.CarreraAnulada && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoRecaudado);
            totalCreditos = db.Creditos.Where(x => (x.FechaDeCredito >= fechaInicio && x.FechaDeCredito <= fechaFin) && !x.CreditoAnulado && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoTotal);
            totalAbonos = db.Abonos.Where(x => (x.FechaDeAbono >= fechaInicio && x.FechaDeAbono <= fechaFin) && !x.Estado && x.Creditos.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoDeAbono);
            return new ConsolidadoPorVehiculo { TotalAbonos = totalAbonos, TotalCarreras = totalCarreras, TotalCreditos = totalCreditos };
        }
        [WebMethod]
        public List<AbonosWS> GetAbonosPorVehiculo(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            var abonos = db.Abonos
                            .Include(c => c.Creditos)
                            .Include(v => v.Creditos.Vehiculos)
                            .Where(v =>
                            v.Creditos.VehiculoId.Equals(vehiculoId) &&
                            v.FechaDeAbono.Date >= fechaInicio &&
                            v.FechaDeAbono.Date <= fechaFin
                            )
                            .ToList();
            // Proyeccion para evitar referencias circulares.
            // y solo seleccionar los datos requeridos.
            var result = (from i in abonos
                          orderby i.FechaDeAbono descending
                          select new AbonosWS
                          {
                              Id = i.Id,
                              CreditoId = i.CreditoId,
                              FechaDeAbono = i.FechaDeAbono,
                              CodigoAbono = i.CodigoAbono,
                              MontoDeAbono = i.MontoDeAbono
                          }).ToList();
            return result;
        }
        #endregion
    }

    #region Clases para mostrar resultados
    [Serializable]
    public class WebResult
    {
        public string Mensaje { get; set; }
        public bool Completado { get; set; }
    }
    [Serializable]
    public class GastosPorArticulo
    {
        public string CodigoDeArticulo { get; set; }
        public string DescripcionDeArticulo { get; set; }
        public decimal Gasto { get; set; }
    }
    [Serializable]
    public class MontoRecaudado
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string Placa { get; set; }
    }
    [Serializable]
    public class ConsolidadoPorVehiculo
    {
        public decimal TotalCarreras { get; set; }
        public decimal TotalCreditos { get; set; }
        public decimal TotalAbonos { get; set; }
    }
    #endregion
}
