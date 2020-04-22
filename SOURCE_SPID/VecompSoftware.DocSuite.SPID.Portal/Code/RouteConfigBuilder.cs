using Microsoft.AspNetCore.Builder;

namespace VecompSoftware.DocSuite.SPID.Portal.Code
{
    public static class RouteConfigBuilder
    {
        public static IApplicationBuilder UseApplicationRoutes(this IApplicationBuilder app)
        {
            return app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "app-acs-callback",
                    template: "{controller=Home}/{action=AuthenticationCallback}");

                routes.MapRoute(
                    name: "spid-auth",
                    template: "{controller=Saml}/{action=Auth}");

                routes.MapRoute(
                    name: "spid-acs",
                    template: "SpidSaml/{action}",
                    defaults: new { controller = "Saml", action = "ACS" });

                routes.MapRoute(
                    name: "spid-logout",
                    template: "SpidSaml/Logout",
                    defaults: new { controller = "Saml", action = "IdpLogout" });

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
