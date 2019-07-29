using Cotracosan.Models;
using Cotracosan.Models.Cotracosan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cotracosan.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        Models.Cotracosan.Context db = new Models.Cotracosan.Context();
        // POST: Datatable Roles
        public JsonResult GetRoles()
        {
            var data = context.Roles.ToList();
            var jsonResult = from item in
                             data
                             select
                        new
                        {
                            id = item.Id,
                            descripcion = item.Name,
                            usuarios = item.Users.Count
                        };
            return Json(new { data = jsonResult}, JsonRequestBehavior.AllowGet);
        }
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create()
        {
            try
            {
                string idRol = Request["idRol"];
                string descripcion = Request["Descripcion"];
                string msj = "Sin cambios";
                bool guardo = false;
                if (string.IsNullOrWhiteSpace(idRol))
                {
                    // Agregar nuevo rol.
                    context.Roles.Add( new Microsoft.AspNet.Identity.EntityFramework.IdentityRole {Name = descripcion });
                    guardo = context.SaveChanges() > 0 ? true : false;
                    msj = "Rol agregado.";
                }else
                {
                    var rol = context.Roles.Find(idRol);
                    rol.Name = descripcion;
                    context.Entry(rol).State = System.Data.Entity.EntityState.Modified;
                    guardo = context.SaveChanges() > 0 ? true : false;
                    msj = "Rol actualizado.";
                }
                return Json( new { success = guardo, mensaje = msj }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            var rol = context.Roles.Find(id);
            var rolView = new RolViewModel { Id = rol.Id, Name = rol.Name };
            foreach (var item in rol.Users)
            {
                var u = context.Users.Find(item.UserId);
                rolView.Usuarios.Add(new AccountViewModel {UserName = u.UserName, Email = u.Email });
            }
            return View(rolView);
        }
        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(string id)
        {
            string mensaje = "Sin Cambios";
            bool guardado = false;
            try
            {
                var rol = context.Roles.Find(id);
                if(rol != null)
                {
                    using (var transact = context.Database.BeginTransaction())
                    {
                        try
                        {
                            // Establecer un rol basico para los usuarios que tenian el rol.
                            var idBasico = context.Roles.First(x => x.Name == "Cajero");
                            if (idBasico == null)
                                throw new NullReferenceException("No existe un rol basico para actualizar los usuarios existentes");

                            var sqlUpdate = string.Format("UPDATE AspNetUserRoles SET RoleId = '{0}' WHERE RoleId = '{1}'",
                                idBasico.Id, rol.Id);
                            var result = context.Database.ExecuteSqlCommand(sqlUpdate);
                            // Eliminamos el rol
                            context.Entry(idBasico).State = System.Data.Entity.EntityState.Detached;
                            context.Roles.Remove(rol);
                            if(guardado = context.SaveChanges() > 0)
                            {
                                transact.Commit();
                                mensaje = "Se ha eliminado el rol.";
                            }
                            else
                            {
                                mensaje = "Error al eliminar las referencias al rol.";
                            }
                        }
                        catch (Exception e)
                        {
                            mensaje = e.Message;
                            if(context.Database.CurrentTransaction != null)
                                transact.Rollback();
                        }
                    }                    
                }

            }
            catch(Exception ex)
            {
                mensaje = ex.Message;
            }
            return Json(new {success = guardado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AsignarUsuarioSocio()
        {
            // Lista de usuarios.
            ViewBag.UserId = new SelectList(context.Users, "Id", "Username");
            ViewBag.RolesId = new SelectList(context.Roles, "Id", "Name");
            return View() ;
        }
        [HttpPost]
        public ActionResult AsignarUsuarioSocio(SocioUsuarioRol socioUsuario)
        {
            return View();
        }
        public ActionResult ListaDeSocios(string userId)
        {
            // buscamos el socio asociado actual, si existe.
            
            string s = context.Users.Find(userId).SocioId;
            if (!string.IsNullOrEmpty(s))
                ViewBag.SocioId = new SelectList(db.Socios, "Id", "SocioNombre", int.Parse(s));
            else
                ViewBag.SocioId = new SelectList(db.Socios, "Id", "SocioNombre");
            return View();
        }
    }
}
