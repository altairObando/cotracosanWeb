using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cotracosan.Models.Cotracosan;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Cotracosan.Controllers.Services
{
    public class ApiCreditosController: Controller
    {
        private Context db = new Context();
        // Todos los creditos efectuados a un bus o a todos
        public async Task<JsonResult> GetCreditos(int? idBus)
        {
            List<Creditos> creditos = idBus == null ?
                await db.Creditos.ToListAsync() :
                await db.Creditos.Where(y => y.VehiculoId.Equals((int)idBus)).ToListAsync();

            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new
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
                            x => new {
                                Id = x.Id,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Cantidad = x.Cantidad,
                                Precio = x.Articulos.Precio
                            })
                    }
                );

            return Json(new { Creditos = result }, JsonRequestBehavior.AllowGet);
        }
        // Créditos pendiente por bus ambas consultas.
        public async Task<JsonResult> GetCreditosPorBus(int idBus)
        {
            string fechaInicio = Request["fechaInicio"];
            string fechaFin = Request["fechaFin"];

            List<Creditos> creditos = await db.Creditos
                .Where(
                x => x.VehiculoId.Equals(idBus) &&
                x.MontoTotal > x.Abonos.Where(y => y.Estado).Sum( a => a.MontoDeAbono))
                .ToListAsync();

            if(!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                DateTime fechaI = DateTime.Parse(fechaInicio);
                DateTime fechaF = DateTime.Parse(fechaFin);
                creditos = creditos.Where(filtro => filtro.FechaDeCredito >= fechaI && filtro.FechaDeCredito <= fechaF).ToList();
            }else
            {
                if (!string.IsNullOrEmpty(fechaInicio))
                {
                    DateTime filtrarDia = DateTime.Parse(fechaInicio);
                    creditos = creditos.Where(filtro => filtro.FechaDeCredito.Equals(filtrarDia)).ToList();
                }
            }

            // Proyeccion para eliminar referencias circulares.

            var result = 
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select  new
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
                            x => new {
                                Id = x.Id,
                                Cantidad = x.Cantidad,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Precio = x.Articulos.Precio                       
                            })
                    }
                );

            return Json(new {Creditos = result }, JsonRequestBehavior.AllowGet);
        }
        // Créditos pendientes total de un socio. Todos descendentes.**
        public async Task<JsonResult> GetCreditosPendientePorSocio(int id)
        {
            var creditos = await db.Creditos
                .Include(x => x.Vehiculos)
                .Where(c => c.Vehiculos.SocioId == id)
                .Where(c => c.MontoTotal > c.Abonos.Where(x => x.Estado).Sum( a => a.MontoDeAbono ))
                .ToListAsync();

            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new
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
                            x => new {
                                Id = x.Id,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Cantidad = x.Cantidad,
                                Precio = x.Articulos.Precio
                            })
                    }
                );
            decimal totalCreditos = result.Sum(x => x.MontoTotal);
            return Json(new { Creditos = result, total = totalCreditos }, JsonRequestBehavior.AllowGet);
        }
        // Lista de los últimos créditos realizados. De forma descendente.
        public async Task<JsonResult> GetUltimosCreditos()
        {
            string maximo = Request["max"];

            var creditos = await db.Creditos
                .OrderByDescending(x => x.FechaDeCredito)
                .ToListAsync();
            try
            {
                if (!string.IsNullOrEmpty(maximo))
                    creditos = creditos.Take(int.Parse(maximo)).ToList();
            }
            catch (Exception)
            {
            }
            var result =
                (
                    from item in creditos
                    orderby item.FechaDeCredito descending
                    select new
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
                            x => new {
                                Id = x.Id,
                                Cantidad = x.Cantidad,
                                ArticuloId = x.ArticuloId,
                                CodigoArticulo = x.Articulos.CodigoDeArticulo,
                                Articulo = x.Articulos.DescripcionDeArticulo,
                                Precio = x.Articulos.Precio
                            })
                    }
                );

            return Json(new { Creditos = result }, JsonRequestBehavior.AllowGet);
        }
        // Eliminar un credito y sus abonos
        public async Task<JsonResult> DeleteCredito(int creditoId)
        {
            // Buscar el credito.
            var credito = await db.Creditos.FindAsync(creditoId);
            if(credito == null)
                return Json(new { eliminado = false, mensaje = "No se ha encontrado el credito con el id: " + creditoId }, JsonRequestBehavior.AllowGet);
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    credito.CreditoAnulado = true;
                    db.Entry(credito).State = EntityState.Modified;
                    bool creditoAnulado = await db.SaveChangesAsync() > 0;
                    bool abonosAnulados = false;
                    foreach(var abono in credito.Abonos)
                    {
                        abono.Estado = false;
                        db.Entry(abono).State = EntityState.Modified;
                        abonosAnulados = await db.SaveChangesAsync() > 0;
                    }
                    if (creditoAnulado && abonosAnulados)
                        transact.Commit();
                    return Json(
                        new {
                            eliminado = creditoAnulado && abonosAnulados,
                            mensaje = creditoAnulado && abonosAnulados ?   "Se ha eliminado el credito solicitado"
                                      : creditoAnulado && !abonosAnulados ? "No se ha logrado eliminar los abonos del credito"
                                      : !creditoAnulado && abonosAnulados ? "No se ha logrado eliminar el credito"
                                      : "No se ha eliminado ningun registro del sistema remoto"
                        }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    transact.Rollback();
                    return Json(new { eliminado = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }            
        }
        
        public async Task<JsonResult> AddCredito([Bind(Include = "Id,CodigoCredito,FechaDeCredito,MontoTotal,EstadoDeCredito,CreditoAnulado,VehiculoId")] Creditos creditos, string DetalleCredito)
        {
            bool gCredito = false; // se ha guardado el credito ? 
            bool gDetalle = false; // se ha guardado el detalle ? 
            List<DetallesDeCreditos> detalle = JsonConvert.DeserializeObject<List<DetallesDeCreditos>>(DetalleCredito);

            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    creditos.EstadoDeCredito = true;
                    db.Creditos.Add(creditos);
                    gCredito = await db.SaveChangesAsync() > 0;
                    if (gCredito)
                    {
                        detalle.ForEach(x => x.CreditoId = creditos.Id);
                        db.DetallesDeCreditos.AddRange(detalle);
                        gDetalle = await db.SaveChangesAsync() > 0;
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
            return Json(new { success = gCredito && gDetalle, message = mensaje });
        }
    }
}