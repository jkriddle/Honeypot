using System;
using System.Web;
using System.Web.Optimization;

namespace Honeypot.Web
{
    public class BundleConfig
    {
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            /**********************************************************
             * Scripts
             **********************************************************/
            // Core libraries
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/core").Include(
                "~/Content/js/jquery.js",
                "~/Content/js/bootstrap.js",
                "~/Content/js/date.js",
                "~/Content/js/handlebars.js"));

            // Plugins to the core libraries
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/plugins").Include(
                "~/Content/js/plugins/jquery-ui/jquery.ui.custom.js",
                "~/Content/js/plugins/jquery.cookie.js",
                "~/Content/js/plugins/jquery.livequery.js",
                "~/Content/js/plugins/select2/jquery.select2.js",
                "~/Content/js/plugins/jquery.serializeObject.js",
                "~/Content/js/plugins/jquery.ui.custom.js",
                "~/Content/js/plugins/jquery.form.js",
                "~/Content/js/plugins/bootbox.custom.js",
                "~/Content/js/plugins/datepicker/bootstrap-datepicker.js",
                "~/Content/js/plugins/daterangepicker/daterangepicker.js",
                "~/Content/js/plugins/toggle-buttons/jquery.toggle.buttons.js"));

            // Honeypot Application
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/app").Include(
                "~/Content/js/app/core/inheritance.js",
                "~/Content/js/app/core/observable.js",
                "~/Content/js/app/core/app.js",
                "~/Content/js/app/core/config.js",
                "~/Content/js/app/core/template.js",
                "~/Content/js/app/core/api.js",
                "~/Content/js/app/*.js"));

            // Public site
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/public").Include(
                "~/Content/js/init.js",
                "~/Content/js/public.js"));

            // Manage area
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/manage").Include(
                "~/Content/js/init.js",
                "~/Content/js/manage.js"));

            /**********************************************************
             * Styles
             **********************************************************/
            // Core library styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/core").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-responsive.css",
                "~/Content/css/jquery.ui.custom.css"));

            // Plugins styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/plugins").Include(
                "~/Content/js/plugins/jquery-ui/jquery.ui.custom.css",
                "~/Content/js/plugins/select2/select2.css",
                "~/Content/js/plugins/datepicker/datepicker.less",
                "~/Content/js/plugins/daterangepicker/daterangepicker.css",
                "~/Content/js/plugins/toggle-buttons/bootstrap.toggle.buttons.css"
                            ));

            // Share app styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/app").Include(
                "~/Content/css/app/shared.less"
                            ));

            // Public site styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/public").Include(
                "~/Content/css/app/public.less"
                            ));

            // Manage styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/manage").Include(
                "~/Content/css/app/admin-main.css",
                "~/Content/css/app/admin-brand.less",
                "~/Content/css/app/manage.less"));
        }
    }
}