using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFoundation.Owin.Security.Naver;

namespace Owin
{
    public static class NaverAuthenticationExtensions
    {
        public static IAppBuilder UseNaverAuthentication(this IAppBuilder app, NaverAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            app.Use(typeof(NaverAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseNaverAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseNaverAuthentication(new NaverAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}
