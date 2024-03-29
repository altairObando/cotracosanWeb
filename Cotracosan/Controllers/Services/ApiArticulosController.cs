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
    public class ApiArticulosController : Controller
    {
        private Context db = new Context();
        // AgregarAbono
        public async Task<JsonResult> getGastosPorArticulos()
        {
            string sql = @"SELECT A.CodigoDeArticulo, A.DescripcionDeArticulo, SUM(D.Cantidad * A.Precio ) as Gasto
                           FROM Articulos A INNER JOIN
                           DetallesDeCreditos D ON A.Id = D.ArticuloId INNER JOIN
	                       Creditos C on C.Id = D.CreditoId
                           ";
            if(!string.IsNullOrEmpty(Request["fechaInicio"]) && !string.IsNullOrEmpty(Request["fechaFin"]) )
            {
                sql += string.Format(" WHERE C.FechaDeCredito between '{0}' AND '{1}' ", Request["fechaInicio"], Request["fechaFin"]);                
            }
            try
            {
                var result = await db.Database.SqlQuery<GastosPorArticulo>(
                    sql + @" GROUP BY A.DescripcionDeArticulo, A.CodigoDeArticulo
                             ORDER BY Gasto desc").ToListAsync();
                return Json(new { gastoPorRubro = result, mensaje = "Consulta efectuada correctamente." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new {  mensaje = ex.Message  }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public async Task<JsonResult> getArticulos()
        {
            List<Articulos> articulos;
            string parametro = Request["parametro"];
            if(!string.IsNullOrEmpty(parametro))
                articulos = await db.Articulos.Where(x => x.Estado && 
                x.DescripcionDeArticulo.ToUpper().Contains(parametro.ToUpper())
                ).ToListAsync();
            else
                articulos = await db.Articulos.Where(x => x.Estado).ToListAsync();
            var result = from i in articulos
                         orderby i.DescripcionDeArticulo
                         select new
                         {
                             Id = i.Id,
                             Codigo = i.CodigoDeArticulo,
                             Descripcion = i.DescripcionDeArticulo,
                             Precio = i.Precio
                         };
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        public class GastosPorArticulo
        {
            public string CodigoDeArticulo { get; set; }
            public string DescripcionDeArticulo { get; set; }
            public decimal Gasto { get; set; }
        }
    }
}