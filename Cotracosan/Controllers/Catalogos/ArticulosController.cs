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
    [Authorize(Roles = "Contador, Administrador")]
    public class ArticulosController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> getArticulos()
        {
            var lista = await db.Articulos.ToListAsync();
            var proyeccion = from item in lista
                             where item.Estado
                       select new
                       {
                           Id = item.Id,
                           Codigo = item.CodigoDeArticulo,
                           Descripcion = item.DescripcionDeArticulo,
                           Precio = item.Precio
                       };
            return Json( new { data = proyeccion }, JsonRequestBehavior.AllowGet);
        }
        // GET: Articulos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Articulos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = await db.Articulos.FindAsync(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            return View(articulos);
        }

        // GET: Articulos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articulos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoDeArticulo,DescripcionDeArticulo,Precio,Estado")] Articulos articulos)
        {
            articulos.Estado = true;
            if (ModelState.IsValid)
            {
                db.Articulos.Add(articulos);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion});
        }

        // GET: Articulos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = await db.Articulos.FindAsync(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            return View(articulos);
        }

        // POST: Articulos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoDeArticulo,DescripcionDeArticulo,Precio,Estado")] Articulos articulos)
        {
            articulos.Estado = true;
            if (ModelState.IsValid)
            {
                db.Entry(articulos).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Actualizado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Articulos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = await db.Articulos.FindAsync(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            return View(articulos);
        }

        // POST: Articulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Articulos articulos = await db.Articulos.FindAsync(id);
            articulos.Estado = false;
            db.Entry(articulos).State = EntityState.Modified;
            completado = await db.SaveChangesAsync() > 0 ? true : false;
            mensaje = completado ? "Articulo Eliminado" : "Error al guardar";
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
