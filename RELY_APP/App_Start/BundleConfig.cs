using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace RELY_APP
{
    public class BundleConfig
    {
        // For more information on Bundling, visit https://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                         "~/Scripts/jquery-{version}.min.js",
                          "~/Scripts/jquery-{version}.min.map"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/StyleVF.css",
                     "~/Content/styles/jqx*"));

            bundles.Add(new StyleBundle("~/Content/VodafoneThemes/css").Include(
                "~/Content/VodafoneThemes/css/Black-theme.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.css.map",
                "~/Content/VodafoneThemes/css/bootstrap-theme.min.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.min.css.map",
                "~/Content/VodafoneThemes/css/bootstrap.css",
                "~/Content/VodafoneThemes/css/bootstrap.css.map",
                "~/Content/VodafoneThemes/css/bootstrap.min.css",
                "~/Content/VodafoneThemes/css/bootstrap.min.css.map",
                "~/Content/VodafoneThemes/css/custom.css",
               "~/Content/VodafoneThemes/css/default.css",
                "~/Content/VodafoneThemes/css/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqxgridbundle").Include(
               "~/Scripts/jqxcore.js",
               "~/Scripts/jqxdata.js",
               "~/Scripts/jqxbuttons.js",
               "~/Scripts/jqxscrollbar.js",
               "~/Scripts/jqxmenu.js",
               "~/Scripts/jqxlistbox.js",
               "~/Scripts/jqxdropdownlist.js",
               "~/Scripts/jqxeditor.js",//added for LEmailTemplate
               "~/Scripts/jqxdropdownbutton.js",
               "~/Scripts/jqxcolorpicker.js",
               "~/Scripts/jqxtooltip.js",
               "~/Scripts/jqxwindow.js",
               "~/Scripts/jqxgrid.js",
               "~/Scripts/jqxgrid.selection.js",
               "~/Scripts/jqxgrid.filter.js",
               "~/Scripts/jqxgrid.sort.js",
               "~/Scripts/jqxgrid.pager.js",
               "~/Scripts/jqxgrid.grouping.js",
               "~/Scripts/jqxgrid.columnsresize.js",
               "~/Scripts/jqxgrid.columnsreorder.js",
               "~/Scripts/jqxgrid.edit.js",
               "~/Scripts/jqxtabs.js",
               "~/Scripts/jqxcheckbox.js",
               "~/Scripts/jqxcalendar.js",
               "~/Scripts/jqxdatetimeinput.js",
               "~/Scripts/globalization/globalize.js",
               "~/Scripts/jqxdatatable.js",
               "~/Scripts/jqxgrid.storage.js",
               "~/Scripts/jqxdata.export.js",
               "~/Scripts/jqxgrid.export.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/jqwidgets").Include(
              "~/Scripts/jqxcore.js",
              "~/Scripts/jqxbuttons.js",
              "~/Scripts/jqxscrollbar.js",
              "~/Scripts/jqxpanel.js",
              "~/Scripts/jqxtree.js",
              "~/Scripts/jqxsplitter.js",
              "~/Scripts/jqxexpander.js"));


            //bundles.Add(new ScriptBundle("~/bundles/jQuery.FileUpload").Include(
            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
     //<!-- The Templates plugin is included to render the upload/download listings -->
     "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
           "~/Scripts/jQuery.FileUpload/tmpl.min.js",
//<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
"~/Scripts/jQuery.FileUpload/load-image.all.min.js",
//<!-- The Canvas to Blob plugin is included for image resizing functionality -->
"~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
//"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
//<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
"~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
//<!-- The basic File Upload plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
//<!-- The File Upload processing plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
//<!-- The File Upload image preview & resize plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
//<!-- The File Upload audio preview plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
//<!-- The File Upload video preview plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
//<!-- The File Upload validation plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
//!-- The File Upload user interface plugin -->
"~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
//Blueimp Gallery 2 
"~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
"~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
"~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
"~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"

));
            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(//Blueimp Gallery 2 
"~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
"~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
"~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
"~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));


            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                 "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                  "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                    "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"
                ));




        }
    }
}