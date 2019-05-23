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
using Newtonsoft.Json;

namespace Cotracosan.Controllers.Operaciones
{
    [Authorize]
    public class CreditosController : Controller
    {
        private Context db = new Context();
        // GET: Creditos/GetCreditos
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
        public ActionResult Create()
        {
            ViewBag.CodigoCredito = GenerarCodigoCredito();
            ViewBag.VehiculoId = new SelectList(db.Vehiculos.Where(x => x.Estado), "Id", "Placa");
            return View();
        }

        private string GenerarCodigoCredito()
        {
            // Ultimo id Credito
            int id = db.Creditos.ToList().OrderByDescending(x => x.Id).First().Id;
            return "CRED-" + (id + 1);
        }

        // POST: Creditos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoCredito,FechaDeCredito,MontoTotal,EstadoDeCredito,CreditoAnulado,VehiculoId")] Creditos creditos, string DetalleCredito)
        {
            bool gCredito = false; // se ha guardado el credito ? 
            bool gDetalle = false; // se ha guardado el detalle ? 
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
                            transact.Commit();
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
            return Json(new { success = gCredito && gDetalle, message = mensaje });
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
