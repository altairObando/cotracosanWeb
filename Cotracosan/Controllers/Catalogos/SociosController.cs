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
    public class SociosController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;
        //POST: Socios/getSocios
        [HttpPost]
        public async Task<JsonResult> getSocios()
        {
            var list = await db.Socios.ToListAsync();
            var p = from item in list
                    where item.Estado
                    select new
                    {
                        Id = item.Id,
                        Codigo = item.CodigoSocio,
                        Nombres = item.Nombres,
                        Apellido1 = item.Apellido1Socio,
                        Apellido2 = item.Apellido2Socio,
                        Vehiculos = item.Vehiculos.Count()
                    };
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
        }
        // GET: Socios
        public ActionResult Index()
        {
            return View();
        }

        // GET: Socios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Socios socios = await db.Socios.FindAsync(id);
            if (socios == null)
            {
                return HttpNotFound();
            }
            return View(socios);
        }

        // GET: Socios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Socios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoSocio,Nombres,Apellido1Socio,Apellido2Socio,Estado")] Socios socios)
        {
            if (ModelState.IsValid)
            {
                db.Socios.Add(socios);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Socios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Socios socios = await db.Socios.FindAsync(id);
            if (socios == null)
            {
                return HttpNotFound();
            }
            return View(socios);
        }

        // POST: Socios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoSocio,Nombres,Apellido1Socio,Apellido2Socio,Estado")] Socios socios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(socios).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Socios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Socios socios = await db.Socios.FindAsync(id);
            if (socios == null)
            {
                return HttpNotFound();
            }
            return View(socios);
        }

        // POST: Socios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Socios socios = await db.Socios.FindAsync(id);
            db.Socios.Remove(socios);
            completado = await db.SaveChangesAsync() > 0 ? true : false;
            mensaje = completado ? "Eliminado Correctamente" : "Error al eliminar";
            tipoNotificacion = completado ? "success" : "warning";
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
