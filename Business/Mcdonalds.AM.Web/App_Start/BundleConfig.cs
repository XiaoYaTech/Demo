using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Mcdonalds.AM.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection collection)
        {
            Bundle jqBundle = new ScriptBundle("~/Scripts/Jquery").Include(
                "~/Scripts/Libs/JQuery/jquery-1.10.2.js",
                "~/Scripts/Libs/JQuery/jquery.iframe-transport.js",
                "~/Scripts/Libs/JQuery/jquery.ui.widget.js",
                "~/Scripts/Libs/JQuery/chosen.jquery.js",
                "~/Scripts/Libs/JQuery/jquery.easing.min.js",
                "~/Scripts/Libs/JQuery/jquery.mixitup.min.js",
                "~/Scripts/Libs/JQuery/jquery.tagsinput.min.js",
                "~/Scripts/Libs/JQuery/jquery.scrollTo.js",
                "~/Scripts/Libs/JQuery/jquery.aSimpleTour.js");
            jqBundle.Transforms.Add(new JsMinify());

            Bundle angularBundle = new ScriptBundle("~/Scripts/Angular").Include(
                "~/Scripts/Libs/Angular/angular.js",
                "~/Scripts/Libs/Angular/Plugins/angular-route.js",
                "~/Scripts/Libs/Angular/Plugins/angular-animate.js",
                "~/Scripts/Libs/Angular/Plugins/angular-cookies.js",
                "~/Scripts/Libs/Angular/Plugins/angular-resource.js",
                "~/Scripts/Libs/Angular/Plugins/angular-sanitize.js",
                "~/Scripts/Libs/Angular/Plugins/angular-ui-bootstrap.js",
                "~/Scripts/Libs/Angular/Plugins/angular-chosen.js",
                "~/Scripts/Libs/Angular/Plugins/angular-uploadify.js",
                "~/Scripts/Libs/Angular/Plugins/angular-plupload.js",
                "~/Scripts/Modules/Core.js",
                "~/Scripts/Modules/ApprovalDialog.js",
                "~/Scripts/Modules/Inputs.js",
                "~/Scripts/Modules/Validations.js");
            angularBundle.Transforms.Add(new JsMinify());

            Bundle bootstrapBundle = new ScriptBundle("~/Scripts/Bootstrap").Include(
                    "~/Scripts/Libs/Bootstrap/bootstrap.min.js",
                    "~/Scripts/Libs/Bootstrap/bootstrap-switch.min.js"
            );
            bootstrapBundle.Transforms.Add(new JsMinify());

            Bundle compomentsBundle = new ScriptBundle("~/Scripts/Compoments").Include(
                "~/Scripts/Utils/AppRun.js",
                "~/Scripts/Controllers/*js",
                "~/Scripts/Services/*js",
                "~/Scripts/Services/Closure/*js",
                "~/Scripts/Services/TempClosure/*js",
                "~/Scripts/Services/Renewal/*js",
                "~/Scripts/Filter/*js",
                "~/Scripts/Directives/*js"
                );
            compomentsBundle.Transforms.Add(new JsMinify());
            Bundle appBundle = new ScriptBundle("~/Scripts/MainApp").Include(
                "~/Scripts/Apps/MainApp.js");
            appBundle.Transforms.Add(new JsMinify());
            Bundle appClosureBundle = new ScriptBundle("~/Scripts/Closure").Include(
                "~/Scripts/Apps/ClosureApp.js",
                "~/Scripts/Controllers/Closure/*js",
                "~/Scripts/Controllers/Closure/View/*js",
                "~/Scripts/Controllers/Closure/Process/*js",
                "~/Scripts/Modules/Closure.js"
            );
            appClosureBundle.Transforms.Add(new JsMinify());

            Bundle appTempClosureBundle = new ScriptBundle("~/Scripts/TempClosure").Include(
                //Controllers
                "~/Scripts/Apps/TempClosureApp.js",
                "~/Scripts/Controllers/TempClosure/*js",
                "~/Scripts/Modules/TempClosure.js"
            );
            appTempClosureBundle.Transforms.Add(new JsMinify());

            Bundle appRenewalBundle = new ScriptBundle("~/Scripts/Renewal").Include(
                "~/Scripts/Apps/RenewalApp.js",
                "~/Scripts/Controllers/Renewal/*js",
                "~/Scripts/Modules/Renewal.js"
            );
            appRenewalBundle.Transforms.Add(new JsMinify());
            Bundle utilsBundle = new ScriptBundle("~/Scripts/Global").Include(
                "~/Scripts/Libs/modernizr-2.6.2.js",
                "~/Scripts/Libs/moment/moment.js",
                "~/Scripts/Libs/moment/moment-with-langs.js",
                "~/Scripts/Libs/JQuery/plupload/plupload.full.min.js",
                "~/Scripts/Libs/JQuery/plupload/i18n/zh_CN.js",
                "~/Scripts/Utils/ValidatePatterns.js",
                "~/Scripts/Utils/Utils.js",
                "~/Scripts/Libs/AjaxQueue.js",
                "~/Scripts/global.js"
                );
            utilsBundle.Transforms.Add(new JsMinify());

            Bundle koBundle = new ScriptBundle("~/Scripts/Knockout").Include(
                "~/Scripts/Libs/Knockout/knockout-3.2.0.js",
                "~/Scripts/Libs/Knockout/ko.binders.js",
                "~/Scripts/Libs/Knockout/ko.components.js"
            );
            koBundle.Transforms.Add(new JsMinify());

            Bundle cssBundle = new StyleBundle("~/Css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/font-awesome.css",
                "~/Content/chosen.css",
                "~/Scripts/Libs/uploadify/uploadify.css",
                "~/Content/bootstrap-switch.min.css",
                "~/Content/bootstrap-override.css",
                "~/Content/site.css"
            );
            cssBundle.Transforms.Add(new CssMinify());

            Bundle noticesBundle = new ScriptBundle("~/Scripts/Notices").Include(
                //Controllers
                "~/Scripts/Apps/NoticesApp.js"
                //"~/Scripts/Controllers/Notices/*js",
                //"~/Scripts/Services/Notices/*js"
            );
            noticesBundle.Transforms.Add(new JsMinify());

            collection.Add(jqBundle);
            collection.Add(angularBundle);
            collection.Add(bootstrapBundle);
            collection.Add(appBundle);
            collection.Add(appClosureBundle);
            collection.Add(appTempClosureBundle);
            collection.Add(appRenewalBundle);
            collection.Add(noticesBundle);
            collection.Add(compomentsBundle);
            collection.Add(utilsBundle);
            collection.Add(koBundle);
            collection.Add(cssBundle);
        }
    }
}