using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Cotracosan.Models;
using System.Web.Services;

namespace Cotracosan.Controllers.WebServices
{
    /// <summary>
    /// Servicio ASMX para la autenticacion de usuarios
    /// que podra ser consumida desde dispositivos moviles desarrollados
    /// usando Xamarin.Android o cualquier uso general
    /// para la Aplicacion Web de CotracosanWeb
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class AccountService : System.Web.Services.WebService
    {
        private ApplicationDbContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountService()
        {
            db = new ApplicationDbContext();
        }
        public AccountService(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = new ApplicationDbContext();
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [WebMethod]
        public bool Login(string Usuario, string Contrasenia)
        {
            var result = SignInManager.PasswordSignIn(Usuario, Contrasenia, true, false);
            if (result == SignInStatus.Success)
                return true;
            else
                return false;
        }
    }
}
