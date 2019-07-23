using Cotracosan.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cotracosan.Controllers.Services
{
    public class ApiAccountController : Controller
    {
        #region Configuración
        private ApplicationDbContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApiAccountController()
        {
            db = new ApplicationDbContext();
        }
        public ApiAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = new ApplicationDbContext();
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion Fin de la configuración

        [HttpPost]
        public async Task<JsonResult> IniciarSesion(string username, string Contrasenia)
        {
            var result = SignInManager.PasswordSignIn(username, Contrasenia, true, false);
            bool sesion = result == SignInStatus.Success;
            ApplicationUser usuario = null;
            if (sesion)
            {
                usuario = db.Users.First(x => x.UserName == username);
            }
            if (!sesion)
                return Json(new
                {
                    login = sesion,
                    message = "Error al inicar sesion",
                    usuario = usuario
                }, JsonRequestBehavior.AllowGet);
            else
            {
                var roles = await UserManager.GetRolesAsync(usuario.Id);
                return Json(new
                {
                    message = "Bienvenido " + usuario.UserName + "!",
                    login = sesion,
                    id = usuario.Id,
                    usuario = usuario.UserName,
                    socioId = usuario.SocioId,
                    email = usuario.Email,
                    rol = roles.FirstOrDefault(),
                    imagen = string.Format("data:image/jpeg;base64, {0}", Convert.ToBase64String(usuario.ImagenPerfil))
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CambiarContraseña(string usuarioId, string viejaContrasenia, string nuevaContrasenia)
        {
            var result = await UserManager.ChangePasswordAsync(usuarioId, viejaContrasenia, nuevaContrasenia);
            return Json(new
            {
                completado = result.Succeeded,
                mensaje = result.Succeeded ? "Se ha cambiado la contraseña" : "Error durante el cambio de contraseña"
            });
        }
        [HttpPost]
        public async Task<JsonResult> CambiarImagen(string usuarioId, string imagenBase64)
        {
            bool completado = false;
            try
            {
                var user = await UserManager.FindByIdAsync(usuarioId);
                byte[] imagenCodificada = Convert.FromBase64String(imagenBase64);
                user.ImagenPerfil = imagenCodificada;
                var taskResult= await UserManager.UpdateAsync(user);
                completado = taskResult.Succeeded;
                return Json(new
                {
                    completado = completado,
                    mensaje = completado ? "Imagen actualizada" : "No se pudo actualizar"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    completado = completado,
                    mensaje = ex.Message
                });
            }
           
        }
    }
}