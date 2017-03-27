using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WRC_CMS.Controllers;

namespace WRC_CMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_EndRequest()
        {
            //if (Context.Response.StatusCode == 404 || Context.Response.StatusCode == 500)
            //{
            //    Response.Clear();
            //    var rd = new RouteData();
            //    rd.Values["controller"] = "ErrorInfo";
            //    rd.Values["action"] = "Index";
            //    IController DC = new ErrorInfoController();
            //    DC.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            //}
        }
    }
}
