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

namespace Cotracosan.Controllers.Catalogos
{
    public class TurnosController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;

        [HttpPost]
        public async Task<JsonResult> getTurnos()
        {
            var list = await db.Turnos.ToListAsync();
            // Realizar una proyeccion para evitar referencias circulares
            var result = from item in list
                         select new
                         {
                             Codigo = item.CodigoDeTurno,
                             HoraSalida = item.HoraDeSalida.ToString(),
                             HoraLlegada = item.HoraDeLlegada.ToString(),
                             Id = item.Id
                         };
            return Json( new { data = result}, JsonRequestBehavior.AllowGet);
        }
        // GET: Turnos
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: Turnos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turnos turnos = await db.Turnos.FindAsync(id);
            if (turnos == null)
            {
                return HttpNotFound();
            }
            return View(turnos);
        }

        // GET: Turnos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Turnos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([Bind(Include = "Id,CodigoDeTurno,HoraDeSalida,HoraDeLlegada")] Turnos turnos)
        {
            if (ModelState.IsValid)
            {
                db.Turnos.Add(turnos);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Turnos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turnos turnos = await db.Turnos.FindAsync(id);
            if (turnos == null)
            {
                return HttpNotFound();
            }
            return View(turnos);
        }

        // POST: Turnos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoDeTurno,HoraDeSalida,HoraDeLlegada")] Turnos turnos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(turnos).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Turnos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turnos turnos = await db.Turnos.FindAsync(id);
            if (turnos == null)
            {
                return HttpNotFound();
            }
            return View(turnos);
        }

        // POST: Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Turnos turnos = await db.Turnos.FindAsync(id);
            db.Turnos.Remove(turnos);
            completado = await db.SaveChangesAsync() > 0 ? true : false;
            mensaje = completado ? "Turno Eliminado" : "Error al guardar";
            tipoNotificacion = completado ? "success" : "warning";
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
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
