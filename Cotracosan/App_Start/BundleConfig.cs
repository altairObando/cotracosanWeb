﻿using System.Web;
using System.Web.Optimization;

namespace Cotracosan
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include(
                 "~/Scripts/datatables.min.js",
                 "~/Scripts/dataTables.semanticui.min.js",
                 "~/Scripts/dataTables.jqueryui.min",
                 "~/Scripts/dataTables.foundation.min.js",
                 "~/Scripts/dataTables.bootstrap4.min.js"
                ));
            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/crud").Include(
                      "~/Scripts/cotracosan/Crud.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/datatablecss").Include(
                    "~/Content/datatables.min.css",
                    "~/Content/dataTables.bootstrap4.min.css",
                    "~/Content/dataTables.foundation.min.css",
                    "~/Content/dataTables.jqueryui.min.css",
                    "~/Content/dataTables.semanticui.min.css"
                    ));
        }
    }
}
