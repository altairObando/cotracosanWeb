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
        [AllowAnonymous]
        public JsonResult GetAbonos(string fechaAbono)
        {
            List<Abonos> abonos = db.Abonos.Where(x => x.Estado).ToList();
            DateTime FechaAbono = DateTime.Now;
            if(Request.Params.AllKeys.Contains("fechaAbono"))
            {
                if (!string.IsNullOrWhiteSpace(fechaAbono))
                {
                    FechaAbono = DateTime.Parse(fechaAbono);
                }
                abonos = abonos.Where(f => f.FechaDeAbono == FechaAbono).ToList();
            }
            // proyeccion para evitar referencias circulares
            var result = from item in abonos
                         select new
                         {
                             CodigoAbono = item.CodigoAbono,
                             FechaAbono = item.FechaDeAbono.ToShortDateString(),
                             Credito = item.Creditos.InfoCredito,
                             MontoAbono = string.Format("{0:C2}",item.MontoDeAbono),
                             Id = item.Id
                         };
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
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
        public string GenerarCodigoAbono()
        {
            int ultimoAbono = db.Abonos.ToList().Count > 0  ? db.Abonos.ToList().OrderByDescending(x=> x.Id).First().Id : 0;
            return "ABO-" + (ultimoAbono + 1);
        }
        public JsonResult GetSaldo(string CreditoId)
        {
            int id = int.Parse(CreditoId);
            //Buscar el valor total del credito.            
            return Json(new { data = _obtenerSaldo(id) }, JsonRequestBehavior.AllowGet);
        }
        public decimal _obtenerSaldo(int creditoId)
        {
            var credito = db.Creditos.Find(creditoId);
            decimal montoCredito = credito.MontoTotal;
            // La suma de los abonos
            decimal abonos = credito.Abonos.Count > 0 ? credito.Abonos.Where(y => y.Estado).Sum(x => x.MontoDeAbono) : 0;
            decimal dif = montoCredito - abonos;
            return dif;
        }
        // GET: Abonos/Create
        public ActionResult Agregar()
        {
            // Si proviene de una redireccion
            var creditoId = Request["CreditoId"];
            if (creditoId == null)
                ViewBag.CreditoId = new SelectList(db.Creditos.Where(x => x.EstadoDeCredito), "Id", "InfoCredito");
            else
            {
                ViewBag.CreditoId = new SelectList(db.Creditos.Where(x=> x.EstadoDeCredito), "Id", "InfoCredito", int.Parse(creditoId));
                ViewBag.IdSelect2 = int.Parse(creditoId);
            }
            ViewBag.CodigoAbono = GenerarCodigoAbono();
            return View();
        }

        // POST: Abonos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Agregar([Bind(Include = "Id,FechaDeAbono,CodigoAbono,MontoDeAbono,CreditoId")] Abonos abonos)
        {
            abonos.Estado = true;
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
            bool success = false;
            string mensaje = "Error";
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    Abonos abonos = await db.Abonos.FindAsync(id);
                    db.Entry(abonos).State = EntityState.Modified;
                    success = await db.SaveChangesAsync() > 0 ? true : false;
                    mensaje = success ? "Registro Eliminado" : "Error durante la eliminacion del abono";
                }
                catch (Exception ex)
                {
                    transact.Rollback();
                }
            }
            return Json(new {success = success, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
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
