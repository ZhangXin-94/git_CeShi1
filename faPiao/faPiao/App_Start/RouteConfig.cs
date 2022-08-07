using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace faPiao
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "FaPiao", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "BodyFenXi", action = "BodyIndex", id = UrlParameter.Optional }
            );
        }
    }
}