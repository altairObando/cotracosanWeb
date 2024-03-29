﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cotracosan.Models.Cotracosan;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace Cotracosan.Controllers.Operaciones
{
    [Authorize]
    public class CreditosController : Controller
    {
        private Context db = new Context();
        // GET: Creditos/GetCreditos
        [AllowAnonymous]
        public JsonResult GetCreditos()
        {
            var creditos = db.Creditos.ToList();
            // realizar una proyeccion de los creditos y abonos.
            var p = (from item in creditos
                     where item.EstadoDeCredito
                     select new
                     {
                         IdCredito = item.Id,
                         CodigoCredito = item.CodigoCredito,
                         FechaDeCredito = item.FechaDeCredito.ToShortDateString(),
                         MontoTotal = string.Format("{0:C}", item.MontoTotal),
                         CreditoAnulado = item.CreditoAnulado,
                         Vehiculo = item.Vehiculos.Placa
                     });
            
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
            
        }

        // GET Abonos/5
        public ActionResult Abonos(int id)
        {
            ViewBag.MontoTotal = db.Creditos.Find(id).MontoTotal;
            List<Abonos> abonos = db.Abonos.Where(x => x.CreditoId == id).ToList();
            return View("DetalleAbonos",abonos);
        }
        // GET: Creditos
        public async Task<ActionResult> Index()
        {
            var creditos = db.Creditos.Include(c => c.Vehiculos);
            return View(await creditos.ToListAsync());
        }

        // GET: Creditos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Creditos creditos = await db.Creditos.FindAsync(id);
            if (creditos == null)
            {
                return HttpNotFound();
            }
            return View(creditos);
        }

        // GET: Creditos/Create
        [Authorize(Roles ="Cajero")]
        public ActionResult Create()
        {
            ViewBag.CodigoCredito = GenerarCodigoCredito();
            ViewBag.VehiculoId = new SelectList(db.Vehiculos.Where(x => x.Estado), "Id", "Placa");
            ViewBag.id = Request["VehiculoId"];
            return View();
        }

        private string GenerarCodigoCredito()
        {
            // Ultimo id Credito
            var list = db.Creditos.ToList();
            int id = list.Count > 0 ? list.OrderByDescending(x => x.Id).First().Id : 0;
            return "CRED-" + (id + 1);
        }

        // POST: Creditos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoCredito,FechaDeCredito,MontoTotal,EstadoDeCredito,CreditoAnulado,VehiculoId")] Creditos creditos, string DetalleCredito)
        {
            bool gCredito = false; // se ha guardado el credito ? 
            bool gDetalle = false; // se ha guardado el detalle ? 
            int idCredito = 0;
            List<DetallesDeCreditos> detalle = JsonConvert.DeserializeObject<List<DetallesDeCreditos>>(DetalleCredito);
            if (detalle.Count < 1)
                ModelState.AddModelError("Detalle de Credito", "No se ha agregado ningun detalle al credito actual ó no se ha logrado deserializar el contenido");

            if (ModelState.IsValid)
            {
                using (var transact = db.Database.BeginTransaction())
                {
                    try
                    {
                        creditos.EstadoDeCredito = true;
                        db.Creditos.Add(creditos);
                        gCredito  = await db.SaveChangesAsync() > 0;
                        if (gCredito)
                        {
                            detalle.ForEach(x => x.CreditoId = creditos.Id);
                            db.DetallesDeCreditos.AddRange(detalle);
                            gDetalle = await db.SaveChangesAsync() > 0;
                            // Guardamos la transaccion solo si se guardo el detalle
                            if(gDetalle)
                            {
                                idCredito = creditos.Id;
                                transact.Commit();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        transact.Rollback();
                    }
                }
            }
            string mensaje = gCredito && gDetalle ? "Credito Guardado Correctamente" :
                gCredito && !gDetalle ? "No se ha logrado guardar los articulos del credito" :
                !gCredito && gDetalle ? "No se ha podido guardar el credito" : "Error en la validacion del modelo de datos";
            return Json(new { success = gCredito && gDetalle, message = mensaje, idCredito = idCredito });
        }

        // GET: Creditos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Creditos creditos = await db.Creditos.FindAsync(id);
            if (creditos == null)
            {
                return HttpNotFound();
            }
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", creditos.VehiculoId);
            return View(creditos);
        }

        // POST: Creditos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoCredito,FechaDeCredito,MontoTotal,EstadoDeCredito,CreditoAnulado,VehiculoId")] Creditos creditos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(creditos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", creditos.VehiculoId);
            return View(creditos);
        }

        // GET: Creditos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Administrador"))
                return View("~/Views/Shared/_Error403.cshtml");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Creditos creditos = await db.Creditos.FindAsync(id);
            if (creditos == null)
            {
                return HttpNotFound();
            }
            return View(creditos);
        }

        // POST: Creditos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Creditos creditos = await db.Creditos.FindAsync(id);
            db.Creditos.Remove(creditos);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult FacturaCredito(int id)
        {
            var credito = db.Creditos.Find(id);
            if (credito == null)
                return HttpNotFound();

            cotracosanWebDataSet ds = new cotracosanWebDataSet();
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            var adapter = new cotracosanWebDataSetTableAdapters.FacturaDetalleCreditoTableAdapter();
            adapter.FillByCreditoId(ds.FacturaDetalleCredito, id);
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reportes/FacturaCredito.rdlc";
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables["FacturaDetalleCredito"]));
            rv.SizeToReportContent = false;
            rv.ZoomMode = ZoomMode.FullPage;
            rv.Width = Unit.Pixel(1600);
            rv.Height = Unit.Pixel(1200);
            rv.AsyncRendering = false;
            rv.LocalReport.SetParameters(new ReportParameter[] {
                new ReportParameter("Socio", credito.Vehiculos.Socios.ToString()),
                new ReportParameter("Placa", credito.Vehiculos.Placa),
                new ReportParameter("Fecha", credito.FechaDeCredito.ToShortDateString())
            });
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
