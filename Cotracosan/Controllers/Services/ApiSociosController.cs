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
    public class ApiSociosController : Controller
    {
        private Context db = new Context();
        // Lista de abonos realizados de los buses de un socio. 
        public async Task<JsonResult> GetAbonosPorSocio(int socioId)
        {
            string fechaInicio = Request["fechaInicio"];
            string fechaFin = Request["fechaFin"];

            // Filtrar todos los abonos por socioId
            var abonos = await db.Abonos
                .Include(v => v.Creditos)
                .Include(w => w.Creditos.Vehiculos)
                .Include(x => x.Creditos.Vehiculos.Socios)
                .Where( y => y.Creditos.Vehiculos.SocioId.Equals(socioId))
                .ToListAsync();
            // Aplicar el filtro de las fecha.
            if(!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                abonos = abonos
                    .Where(  x => 
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
            var result = ( from i in abonos 
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