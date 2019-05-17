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
    public class PenalizacionesController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;
        // POST: getPenalizaciones
        [HttpPost]
        public async Task<JsonResult> getPenalizaciones()
        {
            var lista = await db.Penalizaciones.ToListAsync();
            // proyecion de penalizaciones para evitar referencias circulares
            var p = from item in lista
                    where item.Estado
                    select new
                    {
                        Id = item.Id,
                        Codigo = item.CodigoPenalizacion,
                        Valor = item.Cantidad
                    };
            return Json(new { data = p}, JsonRequestBehavior.AllowGet);
        }
        // GET: Penalizaciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: Penalizaciones/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Penalizaciones penalizaciones = await db.Penalizaciones.FindAsync(id);
            if (penalizaciones == null)
            {
                return HttpNotFound();
            }
            return View(penalizaciones);
        }

        // GET: Penalizaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Penalizaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoPenalizacion,Cantidad,Estado")] Penalizaciones penalizaciones)
        {
            if (ModelState.IsValid)
            {
                db.Penalizaciones.Add(penalizaciones);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
                // Actualizar todas las penalizaciones en false.
                db.Database.ExecuteSqlCommand("UPDATE dbo.Penalizaciones SET Estado='0' WHERE Id !=" + penalizaciones.Id);
            }
            return Json(new
            {
                success = completado,
                mensaje = mensaje,
                type = tipoNotificacion
            });
        }

        // GET: Penalizaciones/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Penalizaciones penalizaciones = await db.Penalizaciones.FindAsync(id);
            if (penalizaciones == null)
            {
                return HttpNotFound();
            }
            return View(penalizaciones);
        }

        // POST: Penalizaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoPenalizacion,Cantidad,Estado")] Penalizaciones penalizaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(penalizaciones).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Actualizado Correctamente" : "Error al Actualizar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new
            {
                success = completado,
                mensaje = mensaje,
                type = tipoNotificacion
            });
        }

        // GET: Penalizaciones/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Penalizaciones penalizaciones = await db.Penalizaciones.FindAsync(id);
            if (penalizaciones == null)
            {
                return HttpNotFound();
            }
            return View(penalizaciones);
        }

        // POST: Penalizaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Penalizaciones penalizaciones = await db.Penalizaciones.FindAsync(id);
            penalizaciones.Estado = false;
            db.Entry(penalizaciones).State = EntityState.Modified;
            completado = await db.SaveChangesAsync() > 0 ? true : false;
            mensaje = completado ? "Eliminado Correctamente" : "Error al eliminar";
            tipoNotificacion = completado ? "success" : "danger";
            return Json(new
            {
                success = completado,
                mensaje = mensaje,
                type = tipoNotificacion
            });
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
