using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cotracosan.cotracosanWebDataSetTableAdapters;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace Cotracosan.Controllers.Reportes
{
    [Authorize]
    public class ReportsController : Controller
    {
        private Cotracosan.Models.Cotracosan.Context db = new Models.Cotracosan.Context();
        // GET: Reports/
        public ActionResult Index()
        {
            return View();
        }
        // Instancia del dataset
        cotracosanWebDataSet ds = new cotracosanWebDataSet();
        public ActionResult GetReport(string Catalogo)
        {
            Catalogo = "Articulos";
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            var adapter = new ArticulosTableAdapter();
            adapter.Fill(ds.Articulos);
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reportes/"+ Catalogo +".rdlc";
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables[Catalogo]));
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            ViewBag.reporte = rv;
            return View();
        }
        [HttpGet]
        public ActionResult GastosDeArticulosPorVehiculos()
        {
            ViewBag.Articulos = new SelectList(db.Articulos, "DescripcionDeArticulo", "DescripcionDeArticulo");
            ViewBag.reporte = null;
            return View();
        }
        [HttpPost]
        public ActionResult GastosDeArticulosPorVehiculos(string Articulos, DateTime fechainicio, DateTime fechafin)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            var adapter = new Reporte1TableAdapter();
            adapter.FillByArticuloYFechas(ds.Reporte1, Articulos, fechainicio, fechafin);
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reportes/Report1.rdlc";
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables["Reporte1"]));
            rv.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("articulo", Articulos)});
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            ViewBag.reporte = rv;
            ViewBag.Articulos = new SelectList(db.Articulos, "DescripcionDeArticulo", "DescripcionDeArticulo");
            return View();

        }

        [HttpGet]
        public ActionResult GastosDeVehiculosPorArticulos()
        {
            ViewBag.Bus = new SelectList(db.Vehiculos, "Placa", "Placa");
            ViewBag.reporte = null;
            return View();
        }

        [HttpPost]
        public ActionResult GastosDeVehiculosPorArticulos(string Bus, DateTime fechaInicio, DateTime fechaFin)
        {
            
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            var adapter = new Reportes2TableAdapter();
            adapter.Fill(ds.Reportes2, Bus, fechaInicio, fechaFin);
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reportes/Report2.rdlc";
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables["Reportes2"]));
            rv.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("placa", Bus) });
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            ViewBag.reporte = rv;
            ViewBag.Bus = new SelectList(db.Vehiculos, "Placa", "Placa");
            return View();
        }
    }
}