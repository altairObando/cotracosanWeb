using Cotracosan.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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


        public JsonResult IniciarSesion(string Usuario, string Contrasenia)
        {
            var result = SignInManager.PasswordSignIn(Usuario, Contrasenia, true, false);
            bool sesion = result == SignInStatus.Success;
            string socioId = "";
            if (sesion)
            {
                socioId = db.Users.First(x => x.UserName == Usuario).SocioId;
            }
            return Json( new { login = sesion, SocioId = socioId }, JsonRequestBehavior.AllowGet);
        }
    }
}