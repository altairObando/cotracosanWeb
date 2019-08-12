using System;
using Microsoft.Owin;
using Owin;
using Cotracosan.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(Cotracosan.Startup))]
namespace Cotracosan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            crearRoles();
        }

        private void crearRoles()
        {
            // Instancia del modelo de base de datos
            // Puerto original 63572
            using (var db = new ApplicationDbContext())
            {
                // Transaccion para garantizar la insercion o manejo de error del contenido.
                using (var transact = db.Database.BeginTransaction())
                {
                    try
                    {
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                        //Verificar si existen roles creados al momento de lanzar la aplicacion por primera vez.
                        if (!roleManager.RoleExists("Administrador"))
                        {
                            //Creamos el rol de administrador.
                            var role = new IdentityRole();
                            role.Name = "Administrador";
                            roleManager.Create(role);
                        }
                        if (!roleManager.RoleExists("Contador"))
                        {
                            //Creamos el rol de administrador.
                            var role = new IdentityRole();
                            role.Name = "Contador";
                            roleManager.Create(role);
                        }
                        if (!roleManager.RoleExists("Cajero"))
                        {
                            //Creamos el rol de administrador.
                            var role = new IdentityRole();
                            role.Name = "Cajero";
                            roleManager.Create(role);
                        }
                        if (!roleManager.RoleExists("Gerente"))
                        {
                            //Creamos el rol de administrador.
                            var role = new IdentityRole();
                            role.Name = "Gerente";
                            roleManager.Create(role);
                        }
                        bool existe = userManager.FindByName("admin") != null ? true : false;
                        if(!existe)
                        {
                            // Creamos un usuario por default
                            var user = new ApplicationUser();
                            user.UserName = "admin";
                            user.Email = "admin@cotracosan.com";
                            string password = "c0ntraseñA123*";
                            //Crear usuario
                            var checkUser = userManager.Create(user, password);
                            //Verificar y asignar rol de admin.
                            if (checkUser.Succeeded)
                                userManager.AddToRole(user.Id, "Administrador");
                        }

                        db.SaveChanges();
                        transact.Commit();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
        }
    }
}
