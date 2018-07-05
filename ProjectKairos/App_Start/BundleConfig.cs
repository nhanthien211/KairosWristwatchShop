using System.Web.Optimization;

namespace ProjectKairos
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryeasing").Include(
            //            "~/Scripts/jquery.easing*"));

            //Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.


            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.bundle.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/datatablejquery").Include(
            //    "~/Scripts/dataTables.bootstrap4.js", "~/Scripts/jquery.dataTables.js",
            //    "~/Scripts/sb-admin-datatables.min.js"));

            //css for login
            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/login-form-elements.css",
                "~/Content/login-style.css").Include("~/Content/font-awesome/css/fontawesome-all.min.css", new CssRewriteUrlTransform()));


            //js for login
            bundles.Add(new ScriptBundle("~/bundles/loginscript").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/jquery.backstretch.min.js",
                "~/Scripts/scripts-login-register.js"));

            //css for admin
            bundles.Add(new StyleBundle("~/Content/admin_info_css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/dataTables.bootstrap4.css",
                "~/Content/main-admin-page.css").Include("~/Content/font-awesome/css/fontawesome-all.min.css", new CssRewriteUrlTransform()));

            //js for admin
            bundles.Add(new ScriptBundle("~/bundles/admin_info_script").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/sb-admin.js",
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb-admin-datatables.min.js"));
            bundles.IgnoreList.Clear();
        }
    }
}
