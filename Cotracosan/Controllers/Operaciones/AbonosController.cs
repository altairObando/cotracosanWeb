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
    [Authorize]
    public class AbonosController : Controller
    {
        private Context db = new Context();

        // GET: Abonos
        public async Task<ActionResult> Index()
        {
            var abonos = db.Abonos.Include(a => a.Creditos);
            return View(await abonos.ToListAsync());
        }

        // GET: Abonos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonos abonos = await db.Abonos.FindAsync(id);
            if (abonos == null)
            {
                return HttpNotFound();
            }
            return View(abonos);
        }

        // GET: Abonos/Create
        public ActionResult Create()
        {
            ViewBag.CreditoId = new SelectList(db.Creditos, "Id", "CodigoCredito");
            return View();
        }

        // POST: Abonos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FechaDeAbono,CodigoAbono,MontoDeAbono,CreditoId")] Abonos abonos)
        {
            if (ModelState.IsValid)
            {
                db.Abonos.Add(abonos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CreditoId = new SelectList(db.Creditos, "Id", "CodigoCredito", abonos.CreditoId);
            return View(abonos);
        }

        // GET: Abonos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonos abonos = await db.Abonos.FindAsync(id);
            if (abonos == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreditoId = new SelectList(db.Creditos, "Id", "CodigoCredito", abonos.CreditoId);
            return View(abonos);
        }

        // POST: Abonos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FechaDeAbono,CodigoAbono,MontoDeAbono,CreditoId")] Abonos abonos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(abonos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CreditoId = new SelectList(db.Creditos, "Id", "CodigoCredito", abonos.CreditoId);
            return View(abonos);
        }

        // GET: Abonos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonos abonos = await db.Abonos.FindAsync(id);
            if (abonos == null)
            {
                return HttpNotFound();
            }
            return View(abonos);
        }

        // POST: Abonos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Abonos abonos = await db.Abonos.FindAsync(id);
            db.Abonos.Remove(abonos);
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
