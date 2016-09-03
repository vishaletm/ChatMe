﻿using System.Web.Mvc;
using System.Web.Routing;

namespace ChatMe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "MainApp",
                url: "{action}",
                defaults: new { controller = "Users", action = "Index" }
            );

            routes.MapRoute(
                name: "UserProfile",
                url: "users/{userName}",
                defaults: new { controller = "Users", action = "UserProfile" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
