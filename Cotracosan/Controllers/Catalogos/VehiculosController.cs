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
    [Authorize(Roles = "Contador,Administrador")]
    public class VehiculosController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;
        // POST Vehiculos/getVehiculos
        [AllowAnonymous]
        public async Task<JsonResult> getVehiculos()
        {
            var vehiculos = await db.Vehiculos.Include(v => v.Socios).ToListAsync();
            // Proyeccion para evitar referencias circulares.
            var p = from item in vehiculos
                    where item.Estado
                    select new
                    {
                        Id = item.Id,
                        Placa = item.Placa,
                        Socio = item.Socios.SocioNombre
                    };
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
        }
        // GET: Vehiculos
        public ActionResult Index()
        {
            
            return View();
        }

        // GET: Vehiculos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculos vehiculos = await db.Vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            return View(vehiculos);
        }

        // GET: Vehiculos/Create
        public ActionResult Create()
        {
            ViewBag.SocioId = new SelectList(db.Socios, "Id", "SocioNombre");
            return View();
        }

        // POST: Vehiculos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Placa,SocioId,Estado")] Vehiculos vehiculos)
        {
            vehiculos.Estado = true;
            if (ModelState.IsValid)
            {
                db.Vehiculos.Add(vehiculos);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Vehiculos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculos vehiculos = await db.Vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            ViewBag.SocioId = new SelectList(db.Socios, "Id", "SocioNombre", vehiculos.SocioId);
            return View(vehiculos);
        }

        // POST: Vehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Placa,SocioId,Estado")] Vehiculos vehiculos)
        {
            vehiculos.Estado = true;
            if (ModelState.IsValid)
            {
                db.Entry(vehiculos).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Actualizado Correctamente" : "Error al actualizar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Vehiculos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculos vehiculos = await db.Vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            return View(vehiculos);
        }

        // POST: Vehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Vehiculos vehiculos = await db.Vehiculos.FindAsync(id);
            vehiculos.Estado = false;
            db.Entry(vehiculos).State = EntityState.Modified;
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
