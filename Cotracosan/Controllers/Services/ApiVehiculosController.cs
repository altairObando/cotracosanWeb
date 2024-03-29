﻿using System;
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
        // Todos los buses
        public JsonResult getVehiculos()
        {
            var vehiculos = (from v in db.Vehiculos
                             where v.Estado
                             select new
                             {
                                 Id = v.Id,
                                 SocioId = v.SocioId,
                                 Placa = v.Placa
                             }
                          );
            return Json(new { vehiculos }, JsonRequestBehavior.AllowGet);
        }
        // Todos los buses de un socio.
        public JsonResult getVehiculosPorSocio(int socioId)
        {
            var vehiculos = (from vehiculo in db.Vehiculos
                          where vehiculo.SocioId == socioId && vehiculo.Estado
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

        public JsonResult getConsolidadoVehiculo(int vehiculoId)
        {
            string fechaInicio = Request["fechaInicio"];
            string fechaFin = Request["fechaFin"];
            decimal totalCarreras = 0, totalCreditos = 0, totalAbonos = 0;
            if(string.IsNullOrEmpty(fechaInicio)&& string.IsNullOrEmpty(fechaFin))
            {
                totalCarreras = db.Carreras.Where(x => x.FechaDeCarrera.Equals(DateTime.Now) && !x.CarreraAnulada && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(x => x.MontoRecaudado);
                // totalCarreras = tmpCarreras != null ? tmpCarreras.Sum(a => a.MontoRecaudado) : 0;
                totalCreditos = db.Creditos.Where(x => x.FechaDeCredito.Equals(DateTime.Now) && !x.CreditoAnulado && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoTotal);
                totalAbonos = db.Abonos.Where(x => x.FechaDeAbono.Equals(DateTime.Now) && x.Estado && x.Creditos.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoDeAbono);
            }
            else
            {
                DateTime f1 = DateTime.Parse(fechaInicio);
                DateTime f2 = DateTime.Parse(fechaFin);
                totalCarreras = db.Carreras.Where(x => (x.FechaDeCarrera >= f1 && x.FechaDeCarrera <= f2 ) && !x.CarreraAnulada && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoRecaudado);
                totalCreditos = db.Creditos.Where(x => (x.FechaDeCredito >= f1 && x.FechaDeCredito <= f2 ) && !x.CreditoAnulado && x.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoTotal);
                totalAbonos = db.Abonos.Where(x => (x.FechaDeAbono >= f1 && x.FechaDeAbono <= f2 ) && x.Estado && x.Creditos.VehiculoId.Equals(vehiculoId)).ToList().Sum(a => a.MontoDeAbono);
            }
            
            return Json(new { carreras = totalCarreras, creditos = totalCreditos, abonos = totalAbonos}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> getAbonosPorVehiculo(int vehiculoId)
        {
            var abonos = await db.Abonos
                            .Include(c => c.Creditos)
                            .Include(v => v.Creditos.Vehiculos)
                            .Where(v => v.Creditos.VehiculoId.Equals(vehiculoId))
                            .ToListAsync();
            string fechaInicio = Request["fechaInicio"];
            string fechaFin = Request["fechaFin"];

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                abonos = abonos
                    .Where(x =>
                  x.FechaDeAbono.Date >= DateTime.Parse(fechaInicio) &&
                  x.FechaDeAbono.Date <= DateTime.Parse(fechaFin)
                    ).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(fechaInicio))
                    abonos = abonos.Where(x => x.FechaDeAbono.Equals(DateTime.Parse(fechaInicio))).ToList();
            }
            // Proyeccion para evitar referencias circulares.
            // y solo seleccionar los datos requeridos.
            var result = (from i in abonos
                          orderby i.FechaDeAbono descending
                          select new
                          {
                              IdAbono = i.Id,
                              CreditoId = i.CreditoId,
                              VehiculoId = i.Creditos.VehiculoId,

                              Placa = i.Creditos.Vehiculos.Placa,
                              FechaDeAbono = i.FechaDeAbono.ToShortDateString(),
                              CodigoAbono = i.CodigoAbono,
                              MontoDeAbono = i.MontoDeAbono,
                              AbonoAnulado = i.Estado
                          });
            return Json(new { abonos = result }, JsonRequestBehavior.AllowGet);
        }

    }
}