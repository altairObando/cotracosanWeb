using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cotracosan.Models.Cotracosan;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace Cotracosan.Controllers.Operaciones
{
    [Authorize]
    public class CarrerasController : Controller
    {
        private Context db = new Context();
        #region AjaxActions
        //GET: Carreras/getCarreras
        [HttpPost]
        public async Task<JsonResult> getCarreras()
        {
            // Verificar si se desea mostrar todas las carreras
            var _todo = Request["Todo"];
            bool todo = string.IsNullOrWhiteSpace(_todo) ? false : true;

            // Obtener la fecha para las carreras
            string Fechas = Request["Fecha"];
            DateTime fecha = string.IsNullOrWhiteSpace(Fechas) ? DateTime.Now : DateTime.Parse(Fechas);

            List<Carreras> carreras = await db.Carreras
                .Include(c => c.Conductores)
                .Include(c => c.LugaresFinalesDelosRecorridos)
                .Include(c => c.Penalizaciones)
                .Include(c => c.Turnos)
                .Include(c => c.Vehiculos)
                .ToListAsync();

            if (!todo)
                carreras = carreras.Where(c => c.Estado && c.FechaDeCarrera.Equals(fecha.Date)).ToList();
            // Realizar una proyeccion para evitar referencias circulares 

            var p = from item in carreras
                    select new
                    {
                        Codigo = item.CodigoCarrera,
                        Conductor = item.Conductores.NombreCompleto,
                        Vehiculo = item.Vehiculos.Placa,
                        Fecha = item.FechaDeCarrera.Date.ToShortDateString(),
                        Monto = item.MontoRecaudado,
                        Multa = item.Multa,
                        Id = item.Id                     
                    };
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMulta(int id, string horaRealLlegada)
        {
            // Obtener la hora que deberia de haber llegado.
            TimeSpan horaLlegada = db.Turnos.Find(id).HoraDeLlegada;
            TimeSpan horaReal = TimeSpan.Parse(horaRealLlegada);
            // Cantidad de minutos tarde
            decimal difMinutos = Convert.ToDecimal(horaReal.Subtract(horaLlegada).TotalMinutes);
            // Cantidad a descontar por cada minuto tarde
            decimal valor = db.Penalizaciones.FirstOrDefault(x => x.Estado).Cantidad;
            decimal multa = difMinutos > 0 ? valor * difMinutos : 0;

            return Json(new { data = multa}, JsonRequestBehavior.AllowGet);
        }
        #endregion
        // GET: Carreras
        public ActionResult Index()
        {            
            return View();
        }

        // GET: Carreras/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carreras carreras = await db.Carreras.FindAsync(id);
            if (carreras == null)
            {
                return HttpNotFound();
            }
            return View(carreras);
        }

        // GET: Carreras/Create
        [Authorize(Roles ="Cajero")]
        public ActionResult Create()
        {
            // Obtener la penalizacion vigente
            var p = db.Penalizaciones.Where(x => x.Estado).FirstOrDefault();
            ViewBag.PenalizacionId = p.Id;

            ViewBag.CodigoCarrera = GetCodigoCarrera();
            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "NombreCompleto");
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "NombreDeLugar");
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "HorasTurno");
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa");
            return View();
        }
        public string GetCodigoCarrera()
        {
            int ultimaIdCarrera = 0;
            var carrerasList = db.Carreras.ToList();
            if (carrerasList.Count > 0)
                ultimaIdCarrera = carrerasList.Last().Id;

            return "CC" + (ultimaIdCarrera + 1);
        }

        // POST: Carreras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoCarrera,FechaDeCarrera,HoraRealDeLlegada,CarreraAnulada,MontoRecaudado,Multa,VehiculoId,ConductorId,PenalizacionId,TurnoId,LugarFinalDeRecorridoId")] Carreras carreras)
        {
            if (ModelState.IsValid)
            {
                carreras.Estado = true;
                db.Carreras.Add(carreras);
                await db.SaveChangesAsync();
                return RedirectToAction("Resumen", new {id = carreras.Id });
            }
            var codigo = Request["CodigoCarrera"];
            var p = db.Penalizaciones.Where(x => x.Estado).FirstOrDefault();
            ViewBag.PenalizacionId = p.Id;

            ViewBag.CodigoCarrera = GetCodigoCarrera();
            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "NombreCompleto", carreras.ConductorId);
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "NombreDeLugar", carreras.LugarFinalDeRecorridoId);
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "HorasTurno", carreras.TurnoId);
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", carreras.VehiculoId);
            return View(carreras);
        }

        public ActionResult Resumen(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carreras carreras = db.Carreras.Find(id);
            if (carreras == null)
            {
                return HttpNotFound();
            }
            return View(carreras);
        }

        // GET: Carreras/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Administrador"))
                return View("~/Views/Shared/_Error403.cshtml");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carreras carreras = await db.Carreras.FindAsync(id);
            if (carreras == null)
            {
                return HttpNotFound();
            }
            return View(carreras);
        }

        // POST: Carreras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Carreras carreras = await db.Carreras.FindAsync(id);
            db.Carreras.Remove(carreras);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult TicketFactura(int id)
        {
            cotracosanWebDataSet ds = new cotracosanWebDataSet();
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            var adapter = new cotracosanWebDataSetTableAdapters.TicketCarreraTableAdapter();
            adapter.Fill(ds.TicketCarrera, id);
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reportes/TicketCarrera.rdlc";
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables["TicketCarrera"]));
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            ViewBag.reporte = rv;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
