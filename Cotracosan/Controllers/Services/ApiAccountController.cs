using Cotracosan.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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


        public JsonResult IniciarSesion(string username, string Contrasenia)
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
               
                return Json(new
                {
                    message = "Bienvenido " + usuario.UserName + "!",
                    login = sesion,
                    id = usuario.Id,
                    usuario = usuario.UserName,
                    socioId = usuario.SocioId,
                    email = usuario.Email,
                    imagen = string.Format("data:image/jpeg;base64, {0}", Convert.ToBase64String(usuario.ImagenPerfil))
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}