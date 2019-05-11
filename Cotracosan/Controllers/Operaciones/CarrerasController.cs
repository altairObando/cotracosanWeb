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

namespace Cotracosan.Controllers.Operaciones
{
    public class CarrerasController : Controller
    {
        private Context db = new Context();
        //GET: Carreras/getCarreras
        [HttpPost]
        public async Task<JsonResult> getCarreras()
        {
            var carreras = await db.Carreras.Include(c => c.Conductores).Include(c => c.LugaresFinalesDelosRecorridos).Include(c => c.Penalizaciones).Include(c => c.Turnos).Include(c => c.Vehiculos).ToListAsync();
            var p = from item in carreras
                    where item.Estado
                    select new
                    {
                        Conductor = item.Conductores.NombreCompleto,
                        Vehiculo = item.Vehiculos.Placa,
                        Lugar = item.LugaresFinalesDelosRecorridos.NombreDeLugar,
                        Fecha = item.FechaDeCarrera.Date.ToShortDateString(),
                        Hora = item.HoraRealDeLlegada.ToString(),
                        Monto = item.MontoRecaudado,
                        Multa = item.Multa,
                        Id = item.Id                     
                    };
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
        }
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
        public ActionResult Create()
        {
            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "NombreCompleto");
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "NombreDeLugar");
            ViewBag.PenalizacionId = new SelectList(db.Penalizaciones, "Id", "CodigoPenalizacion");
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "HorasTurno");
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa");
            return View();
        }

        // POST: Carreras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoCarrera,FechaDeCarrera,HoraRealDeLlegada,CarreraAnulada,MontoRecaudado,Multa,VehiculoId,ConductorId,PenalizacionId,TurnoId,LugarFinalDeRecorridoId")] Carreras carreras)
        {
            if (ModelState.IsValid)
            {
                db.Carreras.Add(carreras);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "NombreCompleto", carreras.ConductorId);
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "NombreDeLugar", carreras.LugarFinalDeRecorridoId);
            ViewBag.PenalizacionId = new SelectList(db.Penalizaciones, "Id", "CodigoPenalizacion", carreras.PenalizacionId);
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "HorasTurno", carreras.TurnoId);
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", carreras.VehiculoId);
            return View(carreras);
        }

        // GET: Carreras/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "Licencia", carreras.ConductorId);
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "CodigoDeLugar", carreras.LugarFinalDeRecorridoId);
            ViewBag.PenalizacionId = new SelectList(db.Penalizaciones, "Id", "CodigoPenalizacion", carreras.PenalizacionId);
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "CodigoDeTurno", carreras.TurnoId);
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", carreras.VehiculoId);
            return View(carreras);
        }

        // POST: Carreras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoCarrera,FechaDeCarrera,HoraRealDeLlegada,CarreraAnulada,MontoRecaudado,Multa,VehiculoId,ConductorId,PenalizacionId,TurnoId,LugarFinalDeRecorridoId")] Carreras carreras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carreras).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ConductorId = new SelectList(db.Conductores, "Id", "Licencia", carreras.ConductorId);
            ViewBag.LugarFinalDeRecorridoId = new SelectList(db.LugaresFinalesDelosRecorridos, "Id", "CodigoDeLugar", carreras.LugarFinalDeRecorridoId);
            ViewBag.PenalizacionId = new SelectList(db.Penalizaciones, "Id", "CodigoPenalizacion", carreras.PenalizacionId);
            ViewBag.TurnoId = new SelectList(db.Turnos, "Id", "CodigoDeTurno", carreras.TurnoId);
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", carreras.VehiculoId);
            return View(carreras);
        }

        // GET: Carreras/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
