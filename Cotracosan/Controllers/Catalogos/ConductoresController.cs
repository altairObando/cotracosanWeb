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
    [Authorize]
    public class ConductoresController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;

        // POST: Json List
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> getConductores()
        {
            var list = await db.Conductores.ToListAsync();
            // Proyeccion de conductores
            var result = from item in list
                         where item.Estado
                         select new
                         {
                             Id        = item.Id,
                             Nombres   = item.Nombres,
                             Apellido1 = item.Apellido1Conductor,
                             Apellido2 = item.Apellido2Conductor,
                             Licencia  = item.Licencia
                             //,Carreras  = item.Carreras.Count()
                         };
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Conductores
        [Authorize(Roles ="Contador,Administrador")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Conductores/Details/5
        [Authorize(Roles = "Contador,Administrador")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = await db.Conductores.FindAsync(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // GET: Conductores/Create
        [Authorize(Roles = "Contador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Conductores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Licencia,Nombres,Apellido1Conductor,Apellido2Conductor,Estado")] Conductores conductores)
        {
            if (ModelState.IsValid)
            {
                db.Conductores.Add(conductores);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Conductores/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = await db.Conductores.FindAsync(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // POST: Conductores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Licencia,Nombres,Apellido1Conductor,Apellido2Conductor,Estado")] Conductores conductores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conductores).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Actualizado Correctamente" : "Error al Actulizar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Conductores/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = await db.Conductores.FindAsync(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // POST: Conductores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Conductores conductores = await db.Conductores.FindAsync(id);
            conductores.Estado = false;
            db.Entry(conductores).State = EntityState.Modified;
            completado = await db.SaveChangesAsync() > 0 ? true : false;
            mensaje = completado ? "Eliminado Correctamente" : "Error al eliminar";
            tipoNotificacion = completado ? "success" : "warning";
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion});
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
