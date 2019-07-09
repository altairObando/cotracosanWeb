using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cotracosan.Models.Cotracosan;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Cotracosan.Controllers.Services
{
    public class ApiVehiculosController : Controller
    {
        private Context db = new Context();
        // Todos los buses de un socio.
        public JsonResult getVehiculosPorSocio(int socioId)
        {
            var vehiculos = (from vehiculo in db.Vehiculos
                          where vehiculo.SocioId == socioId
                          select new 
                          {
                              Id = vehiculo.Id,
                              Estado = vehiculo.Estado,
                              Placa = vehiculo.Placa,
                              SocioId = vehiculo.SocioId,
                          }
                          );
            return Json( new { vehiculos }, JsonRequestBehavior.AllowGet);
        }

        // 2. Cuánto dinero hizo un bus en un determinado día.
        public JsonResult getMontoRecaudado(int vehiculoId, DateTime fecha)
        {
            decimal totalRecaudado = 0;
            var bus = db.Vehiculos.Find(vehiculoId);
            if(bus != null)
            {
                totalRecaudado = bus.Carreras.Where(x => !x.CarreraAnulada && x.FechaDeCarrera.Equals(fecha)).Sum(m => m.MontoRecaudado);
            }
            return Json(new { Monto = totalRecaudado }, JsonRequestBehavior.AllowGet);
        }

        // Cuánto dinero hicieron todos los buses de un socio en un intervalo de tiempo
        public JsonResult getMontoTotalRecaudado(int socioId, DateTime fechaInicio, DateTime fechaFin)
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
                select new
                {
                    Id = vehiculo.Key,
                    MontoRecaudado = vehiculo.Sum( x => x.MontoRecaudado),
                    Placa = vehiculo.First().Vehiculos.Placa
                }            
                );
            return Json( new { vehiculos = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getConsolidadoVehiculo(int id)
        {
            string fechaInicio = Request["fechaInicio"];
            string fechaFin = Request["fechaFin"];
            decimal totalCarreras = 0, totalCreditos = 0, totalAbonos = 0;
            if(string.IsNullOrEmpty(fechaInicio)&& string.IsNullOrEmpty(fechaFin))
            {
                totalCarreras = db.Carreras.Where(x => x.FechaDeCarrera.Equals(DateTime.Now.Date) && !x.CarreraAnulada).Sum(a => a.MontoRecaudado);
                totalCreditos = db.Creditos.Where(x => x.FechaDeCredito.Equals(DateTime.Now.Date) && !x.CreditoAnulado).Sum(a => a.MontoTotal);
                totalAbonos = db.Abonos.Where(x => x.FechaDeAbono.Equals(DateTime.Now.Date) && !x.Estado).Sum(a => a.MontoDeAbono);
            }else
            {
                totalCarreras = db.Carreras.Where(x => (x.FechaDeCarrera >= DateTime.Parse(fechaInicio) && x.FechaDeCarrera <= DateTime.Parse(fechaFin)) && !x.CarreraAnulada).Sum(a => a.MontoRecaudado);
                totalCreditos = db.Creditos.Where(x => (x.FechaDeCredito >= DateTime.Parse(fechaInicio) && x.FechaDeCredito <= DateTime.Parse(fechaFin)) && !x.CreditoAnulado).Sum(a => a.MontoTotal);
                totalAbonos = db.Abonos.Where(x => (x.FechaDeAbono >= DateTime.Parse(fechaInicio) && x.FechaDeAbono <= DateTime.Parse(fechaFin)) && !x.Estado).Sum(a => a.MontoDeAbono);
            }
            
            return Json(new { carreras = totalCarreras, creditos = totalCreditos, abonos = totalAbonos}, JsonRequestBehavior.AllowGet);
        }
    }
}