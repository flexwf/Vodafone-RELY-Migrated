using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace RELY_APP
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Set("Server", "Cloud");
            //Response.Headers.Set("X-Powered-By", "Megacube Pty Ltd");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-SourceFiles");
            //Response.Headers.Remove("X-Frame-Options");
            //ShivaniG added as part for Security implementations 
            Response.Headers.Add("X-Frame-Options", "DENY");
            Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-XSS-Protection", "1;mode=block");
            //CSP updated, allowed img-src 
            Response.Headers.Add("Content-Security-Policy",
                "default-src 'none';script-src 'self' 'unsafe-inline' 'unsafe-eval';style-src 'self' 'unsafe-inline' ;" +
                "img-src 'self'  data:;font-src 'self';connect-src 'self';form-action 'self'");

        }

    }
}