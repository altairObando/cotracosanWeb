using Cotracosan.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Cotracosan
{
    /// <summary>
    /// Descripción breve de Manage
    /// </summary>
    [WebService(Namespace = "http://cotracosan.tk/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Manage : WebService
    {
        #region Configuración
        private ApplicationDbContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public Manage()
        {
            db = new ApplicationDbContext();
        }
        public Manage(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        #endregion Fin de la configuración        

        [WebMethod]
        public bool CambiarImagen(byte[] imagen, string idUsuario)
        {
            bool actualizado = false;
            // Buscar el usuario
            var user = db.Users.Find(idUsuario);
            if (user != null)
            {
                user.ImagenPerfil = imagen;
                actualizado = db.SaveChanges() > 0;
            }
            return actualizado;
        }
    }
}
