using System.Web.Optimization;

namespace ProjectKairos
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
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

            //common css for admin
            bundles.Add(new StyleBundle("~/Content/admin_css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/dataTables.bootstrap4.css",
                "~/Content/main-admin-page.css").Include("~/Content/font-awesome/css/fontawesome-all.min.css", new CssRewriteUrlTransform()));

            //common js for admin
            bundles.Add(new ScriptBundle("~/bundles/admin_script").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/sb-admin.js"));

            //Datatable script for manage account page
            bundles.Add(new ScriptBundle("~/bundles/datatable_account").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb-admin-datatables-account.js"));

            //Datatable script for manage order page
            bundles.Add(new ScriptBundle("~/bundles/datatable_order").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb-admin-datatables-order.js"));

            //Datatable script for manage watch page
            bundles.Add(new ScriptBundle("~/bundles/datatable_watch").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb-admin-datatables-watch.js"));

            //Script for validate add new watch page
            bundles.Add(new ScriptBundle("~/bundles/validate_file").Include(
                "~/Scripts/add-watch-script.js"));

            //CKEditor script 
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                "~/Scripts/ckeditor/ckeditor.js"));

            //common css for user
            bundles.Add(new StyleBundle("~/Content/user_css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/index-animate.css",
                "~/Content/index-hamburgers.min.css",
                "~/Content/index-animsition.min.css",
                "~/Content/index-select2.min.css",
                "~/Content/index-slick.css",
                "~/Content/index-util.min.css",
                "~/Content/index-main.css"
            ).Include("~/Content/font-awesome/css/fontawesome-all.min.css", new CssRewriteUrlTransform()));

            //css for checkbox in product.html
            bundles.Add(new StyleBundle("~/Content/product_checkbox_css").Include(
                "~/Content/magic-check.min.css"));

            //common js for user
            bundles.Add(new ScriptBundle("~/bundles/user_script").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/animsition.min.js",
                "~/Scripts/index-popper.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/index-select2.min.js",
                "~/Scripts/index-slick.min.js",
                "~/Scripts/index-slick-custom.min.js",
                "~/Scripts/index-sweetalert.min.js",
                "~/Scripts/index-main.js"));

            //css for 404
            bundles.Add(new StyleBundle("~/Content/404_css").Include(
                "~/Content/bootstrap.min.css").Include("~/Content/font-awesome/css/fontawesome-all.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/bundles/loadAddress").Include(
                "~/Scripts/checkout-load-address.js"));

            //to disable auto ignore .min file while debug = true in web.config
            bundles.IgnoreList.Clear();

        }
    }
}
