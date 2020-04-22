using System.Web.Optimization;

namespace BiblosDS.LegalExtension.AdminPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/js/jquery.min.js",
                "~/Scripts/js/kendo.all.min.js",
                "~/Scripts/js/kendo.aspnetmvc.min.js",
                "~/Scripts/js/cultures/kendo.culture.it-IT.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/common.js"));

            bundles.Add(new StyleBundle("~/Content/styles/kendo").Include(
                 "~/Content/styles/kendo.common.min.css",
                 "~/Content/styles/kendo.silver.min.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                "~/Content/fontawesome-all.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/app").Include(
                "~/Content/style.css"));
        }
    }
}
