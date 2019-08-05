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
    public class RecorridosController : Controller
    {
        private Context db = new Context();
        private string mensaje = "Error";
        private string tipoNotificacion = "danger";
        private bool completado = false;
        // POST: Recorridos/getRecorridos
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> getRecorridos()
        {
            var lista = await db.LugaresFinalesDelosRecorridos.ToListAsync();
            var p = from item in lista
                    where item.Estado
                    select new
                    {
                        Id = item.Id,
                        Codigo = item.CodigoDeLugar,
                        Nombre = item.NombreDeLugar
                    };
            return Json(new { data = p }, JsonRequestBehavior.AllowGet);
        }
        // GET:  Recorridos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Recorridos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos = await db.LugaresFinalesDelosRecorridos.FindAsync(id);
            if (lugaresFinalesDelosRecorridos == null)
            {
                return HttpNotFound();
            }
            return View(lugaresFinalesDelosRecorridos);
        }

        // GET: Recorridos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Recorridos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodigoDeLugar,NombreDeLugar,Estado")] LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos)
        {
            lugaresFinalesDelosRecorridos.Estado = true;
            if (ModelState.IsValid)
            {
                db.LugaresFinalesDelosRecorridos.Add(lugaresFinalesDelosRecorridos);
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Guardado Correctamente" : "Error al guardar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Recorridos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos = await db.LugaresFinalesDelosRecorridos.FindAsync(id);
            if (lugaresFinalesDelosRecorridos == null)
            {
                return HttpNotFound();
            }
            return View(lugaresFinalesDelosRecorridos);
        }

        // POST: Recorridos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodigoDeLugar,NombreDeLugar,Estado")] LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos)
        {
            lugaresFinalesDelosRecorridos.Estado = true;
            if (ModelState.IsValid)
            {
                db.Entry(lugaresFinalesDelosRecorridos).State = EntityState.Modified;
                completado = await db.SaveChangesAsync() > 0 ? true : false;
                mensaje = completado ? "Actualizado Correctamente" : "Error al actualizar";
                tipoNotificacion = completado ? "success" : "warning";
            }
            return Json(new { success = completado, mensaje = mensaje, type = tipoNotificacion });
        }

        // GET: Recorridos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Administrador"))
                return View("~/Views/Shared/_Error403.cshtml");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos = await db.LugaresFinalesDelosRecorridos.FindAsync(id);
            if (lugaresFinalesDelosRecorridos == null)
            {
                return HttpNotFound();
            }
            return View(lugaresFinalesDelosRecorridos);
        }

        // POST: Recorridos/Delete/5
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LugaresFinalesDelosRecorridos lugaresFinalesDelosRecorridos = await db.LugaresFinalesDelosRecorridos.FindAsync(id);
            lugaresFinalesDelosRecorridos.Estado = false;
            db.Entry(lugaresFinalesDelosRecorridos).State = EntityState.Modified;
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
