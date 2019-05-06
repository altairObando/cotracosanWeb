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
    public class CreditosController : Controller
    {
        private Context db = new Context();

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
        public ActionResult Create()
        {
            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa");
            return View();
        }

        // POST: Creditos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoCredito,FechaDeCredito,MontoTotal,EstadoDeCredito,CreditoAnulado,VehiculoId")] Creditos creditos)
        {
            if (ModelState.IsValid)
            {
                db.Creditos.Add(creditos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Placa", creditos.VehiculoId);
            return View(creditos);
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
