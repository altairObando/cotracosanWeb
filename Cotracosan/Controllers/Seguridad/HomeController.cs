using Cotracosan.Models.Cotracosan;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotracosan.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Context db = new Context();
        private static string[] meses = new string[] {
            "ENERO","FEBRERO","MARZO",
            "ABRIL","MAYO","JUNIO",
            "JULIO", "AGOSTO","SEPTIEMBRE",
            "OCTUBRE","NOVIEMBRE","DICIEMBRE"
        };
        private static int[] mesesId = new int[]
        {
            1,2,3,4,5,6,7,8,9,10,11,12
        };
        public ActionResult Index()
        {
            
            if (User.IsInRole("Cajero"))
                return RedirectToAction("Create", "Carreras");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [AllowAnonymous]
        public JsonResult GetDataSets()
        {
            // Obtener las carreras
            var list = db.Carreras.Where(x => x.Estado).ToList();
            // Todos los creditos
            var list2 = db.Creditos.Where(x => x.EstadoDeCredito).ToList();
            // Todos los abonos
            var list3 = db.Abonos.ToList();
            //total de carreras en los meses
            var query1 = (from carrera in list
                         where carrera.FechaDeCarrera.Year == DateTime.Now.Year
                         orderby carrera.FechaDeCarrera.Month ascending
                         group carrera by carrera.FechaDeCarrera.Month into c
                         select new QueryMes{
                             IdMes = c.FirstOrDefault().FechaDeCarrera.Month,
                             Mes = c.FirstOrDefault().NombreMes, 
                             Valor = c.Sum(x => x.MontoRecaudado) / 1000 // Obtenemos el monto recaudado de cada mes
                         }
                         ).ToList();

            var query2 = (from credito in list2
                          where credito.FechaDeCredito.Year == DateTime.Now.Year
                          orderby credito.FechaDeCredito.Month ascending
                          group credito by credito.FechaDeCredito.Month into c
                          select new QueryMes
                          {
                              IdMes = c.FirstOrDefault().FechaDeCredito.Month,
                              Mes = c.FirstOrDefault().NombreMes,
                              Valor = c.Sum(y => y.MontoTotal) / 1000 // Obtenemos el monto recaudado de cada mes
                          }
                         ).ToList();

            var query3 = (from abono in list3
                          where abono.FechaDeAbono.Year == DateTime.Now.Year
                          orderby abono.FechaDeAbono.Month ascending
                          group abono by abono.FechaDeAbono.Month into a
                          select new QueryMes
                          {
                              IdMes = a.FirstOrDefault().FechaDeAbono.Month,
                              Mes = a.FirstOrDefault().NombreMes,
                              Valor = a.Sum(z => z.MontoDeAbono) / 1000
                          }
                          ).ToList();

            fillWithZeros(query1);
            query1 = query1.OrderBy(x => x.IdMes).ToList();

            fillWithZeros(query2);
            query2 = query2.OrderBy(x => x.IdMes).ToList();

            fillWithZeros(query3);
            query3 = query3.OrderBy(x => x.IdMes).ToList();

            // Obtener el valor mas alto alcanzado en ventas
            decimal maximo = query1.Max(x => x.Valor) / 1000;
            return Json(
                new {
                    labels = meses,
                    Carreras = query1,
                    Creditos = query2,
                    Abonos = query3,
                    maximo = maximo
                }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDashboardData()
        {
            #region primer linea del dashboard
            // Datos para la busqueda
            var list = db.Vehiculos.Include(x => x.Carreras).ToList();
            // Vehiculo con mas carreras
            Vehiculos vc = list.OrderByDescending(y => y.Carreras.Count).FirstOrDefault();
            // Todas las carreras.
            var list2 = db.Carreras.Include(v => v.Vehiculos).ToList();
            //Agrupar las carreras por vehiculo y obtener el mas alto
            var group = (from carrera in db.Carreras.Include(v => v.Vehiculos).ToList()
                        where carrera.Estado && carrera.FechaDeCarrera.Year == DateTime.Now.Year
                        group carrera by carrera.VehiculoId into g
                        select new QueryCarrera {
                            Placa = g.FirstOrDefault().Vehiculos.Placa,
                            Monto = g.Sum(x => x.MontoRecaudado)
                        }).ToList();
            // Obtener el mas alto.
            var vd = group.OrderByDescending(d => d.Monto).FirstOrDefault();
            #endregion
            #region segunda parte del dashboard
            // Datos para la busqueda
            var dcreditos = db.DetallesDeCreditos.Where(x => x.Creditos.EstadoDeCredito &&x.Creditos.FechaDeCredito.Year == DateTime.Now.Year).ToList();
            // Agrupar los articulos por credito
            var gArticulos = (from detalle in dcreditos
                         group detalle by detalle.ArticuloId into g
                         select new QueryArticulos {
                             ArticuloId = g.FirstOrDefault().ArticuloId,
                             Descripcion = g.FirstOrDefault().Articulos.DescripcionDeArticulo,
                             Cantidad = g.Sum(i => i.Cantidad),
                             Total = g.Sum(t => t.Cantidad * t.Articulos.Precio)
                         }) .ToList();

            var masSolicitado = gArticulos.OrderByDescending(x => x.Cantidad).First();
            var menosSolicitado = gArticulos.OrderBy(y => y.Cantidad).First();

            #endregion
            return Json(new {
                vehiculo = new { placa1 = vc.Placa, carreras = vc.Carreras.Count, placa2 = vd.Placa, monto = string.Format("{0:C2}", vd.Monto ) },
                credito = new { nombre1 = masSolicitado.Descripcion, total1 = string.Format("{0:C2}", masSolicitado.Total), nombre2 = menosSolicitado.Descripcion, total2 = string.Format("{0:C2}", menosSolicitado.Total) }
            }, JsonRequestBehavior.AllowGet);
        }
        private void fillWithZeros(List<QueryMes> resultado)
        {
            foreach (int item in mesesId)
            {
                if(resultado.Find(x=> x.IdMes == item) == null)
                {
                    resultado.Add(new QueryMes {IdMes = item, Mes = meses[item-1].ToLower(), Valor = 0 });
                }
            }
        }
        /// <summary>
        /// Obtiene las ultimas 30 fechas de ventas
        /// </summary>
        /// <returns>Json Result</returns>
        [AllowAnonymous]        
        public JsonResult GetTimeSeries()
        {
            // Agrupar las carreras por fecha y seleccionar las ultimas 30
            var carreras = db.Carreras.Where(y => y.Estado && y.FechaDeCarrera.Year == DateTime.Now.Year).ToList();
            // Agrupando
            var group = (from c in carreras
                         group c by c.FechaDeCarrera.Date into g
                         select new QuerySerieTiempo
                         {
                             Key = g.Key,
                             FechaDia = g.FirstOrDefault().NombreMes + " " + g.Key.Day,
                             TotalDia = g.Sum(x => x.MontoRecaudado)
                         })
                         .OrderByDescending(y => y.Key)
                         .Take(30)
                         .OrderBy( z => z.Key)
                         .ToList();
            return Json(new
            {   data = group,
                fechaInicio = group.FirstOrDefault().Key.ToShortDateString(),
                fechaFin = group.LastOrDefault().Key.ToShortDateString()
            }, JsonRequestBehavior.AllowGet);
        }
    }

    public class QueryMes
    {
        public int IdMes { get; set; }
        public string Mes { get; set; }
        public decimal Valor { get; set; }
    }
    public class QueryCarrera
    {
        public string Placa { get; set; }
        public decimal Monto { get; set; }
    }
    public class QueryArticulos
    {
        public int ArticuloId { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
    public class QuerySerieTiempo
    {
        public DateTime Key { get; set; }
        public string FechaDia { get; set; }
        public decimal TotalDia { get; set; }
    }
}