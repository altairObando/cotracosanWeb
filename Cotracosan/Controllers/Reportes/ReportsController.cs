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
    }
}