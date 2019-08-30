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
        public WebLoginResult IniciarSesion(string username, string Contrasenia)
        {
            var result = SignInManager.PasswordSignIn(username, Contrasenia, true, false);
            bool sesion = result == SignInStatus.Success;
            ApplicationUser usuario = null;
            if (sesion)
            {
                usuario = db.Users.First(x => x.UserName == username);
            }
            if (!sesion)
                return new WebLoginResult
                {
                    IsLogged = sesion,
                    Mensaje = "Error al inicar sesion",
                    UserName = usuario.UserName
                };
            else
            {
                var roles = UserManager.GetRolesAsync(usuario.Id).Result;
                return new
                WebLoginResult
                {
                    Mensaje = "Bienvenido " + usuario.UserName + "!",
                    IsLogged = sesion,
                    UserId = usuario.Id,
                    UserName = usuario.UserName,
                    SocioId = usuario.SocioId,
                    Email = usuario.Email,
                    Rol = roles.FirstOrDefault(),
                    Imagen = string.Format("data:image/jpeg;base64, {0}", Convert.ToBase64String(usuario.ImagenPerfil))
                };
            }
        }

        [WebMethod]
        public WebResult CambiarContraseña(string usuarioId, string viejaContrasenia, string nuevaContrasenia)
        {
            var result = UserManager.ChangePasswordAsync(usuarioId, viejaContrasenia, nuevaContrasenia).Result;
            return (new WebResult
            {
                Completado = result.Succeeded,
                Mensaje = result.Succeeded ? "Se ha cambiado la contraseña" : "Error durante el cambio de contraseña"
            });
        }
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

    public class WebLoginResult
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SocioId { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Imagen { get; set; }
        public string Mensaje { get; set; }
        public bool IsLogged { get; set; }
    }
}
