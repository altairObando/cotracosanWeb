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
                            where !item.CarreraAnulada orderby item.FechaDeCarrera descending
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
            return Json(carreras, JsonRequestBehavior.AllowGet);
        }
    }
}