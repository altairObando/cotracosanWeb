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
    public class ApiCarrerasController : Controller
    {
        private Context db = new Context();
        public JsonResult getCarreras()
        {
            var c = db.Carreras.ToList();
            // Proyectar para evitar referencias circulares
            var carreras = (from item in c
                            where item.Estado orderby item.FechaDeCarrera descending
                            select new
                            {
                                Id = item.Id,
                                CodigoCarrera = item.CodigoCarrera,
                                FechaDeCarrera = item.FechaDeCarrera.ToShortDateString(),
                                MontoRecaudado = item.MontoRecaudado,
                                Multa = item.Multa,
                                Conductor = item.Conductores.NombreCompleto,
                                ConductorId = item.ConductorId,
                                Vehiculo = item.Vehiculos.Placa,
                                VehiculoId = item.VehiculoId,
                                LugarFinalRecorrido = item.LugaresFinalesDelosRecorridos.NombreDeLugar,
                                HoraDeLlegada = item.HoraRealDeLlegada.ToString(),
                                Turno = item.Turnos.HorasTurno,
                                CarreraAnulada = item.CarreraAnulada

                            });
            return Json(new { carreras = carreras}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> getCarrerasPorVehiculo(int vehiculoId)
        {
            List<Carreras> carreras = null;
            // Obteniendo extras
            string _fechaInicial = Request["fechaInicial"];
            string _fechaFinal = Request["fechaFinal"];
            string _max = Request["max"];
            // Parametros opcionales
            int Maximo = 0;
            DateTime FechaInicial = new DateTime();
            DateTime FechaFinal = new DateTime();

            if (!string.IsNullOrEmpty(_max))
                Maximo = int.Parse(_max);
            if (!string.IsNullOrEmpty(_fechaInicial))
                FechaInicial = DateTime.Parse(_fechaInicial);
            if (!string.IsNullOrEmpty(_fechaFinal))
                FechaFinal = DateTime.Parse(_fechaFinal);
            // Consultar por fechas
            if(!string.IsNullOrEmpty(_fechaInicial) && !string.IsNullOrEmpty(_fechaFinal))
            {
                // Consultar entre fechas
                carreras = await db.Carreras.Where(
                    x => x.VehiculoId.Equals(vehiculoId) &&
                    x.FechaDeCarrera.Date >= FechaInicial && 
                    x.FechaDeCarrera.Date <= FechaFinal  &&
                    x.Estado
                    ).OrderByDescending(z => z.FechaDeCarrera).ToListAsync();
            }
            else
            {
                // Consultar todas las carreras.
                carreras = await db.Carreras.Where(
                    y => y.Estado &&
                    y.VehiculoId.Equals(vehiculoId)
                    ).OrderByDescending(z => z.FechaDeCarrera).ToListAsync();
            }
            // Reducir el limite con el valor maximo
            if (Maximo > 0)
                carreras = carreras.Take(Maximo).ToList();
            // Realizar proyeccion para evitar referencias circulares
            var result = (from item in carreras
                          select new
                          {
                              Id = item.Id,
                              CodigoCarrera = item.CodigoCarrera,
                              FechaDeCarrera = item.FechaDeCarrera.ToShortDateString(),
                              MontoRecaudado = item.MontoRecaudado,
                              Multa = item.Multa,
                              Conductor = item.Conductores.NombreCompleto,
                              ConductorId = item.ConductorId,
                              Vehiculo = item.Vehiculos.Placa,
                              VehiculoId = item.VehiculoId,
                              LugarFinalRecorrido = item.LugaresFinalesDelosRecorridos.NombreDeLugar,
                              HoraDeLlegada = item.HoraRealDeLlegada.ToString(),
                              Turno = item.Turnos.HorasTurno,
                              CarreraAnulada = item.CarreraAnulada
                          });
            return Json(new { carreras = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCarrera( int carreraId)
        {
            // Buscar la carrera
            var carrera = db.Carreras.Find(carreraId);
            if (carrera == null)
                return Json(new { eliminado = false, mensaje = "No existe ninguna carrera con el id: " + carreraId }, JsonRequestBehavior.AllowGet);
            carrera.CarreraAnulada = true;
            db.Entry(carrera).State = EntityState.Modified;
            bool eliminado = db.SaveChanges() > 0;
            return Json(new {
                eliminado = eliminado,
                mensaje = eliminado ? "Se ha eliminado la carrera" : "No se ha eliminado la carrera"
                });
        }
    }
}