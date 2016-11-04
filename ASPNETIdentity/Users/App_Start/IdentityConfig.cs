using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using NetFoundation.Owin.Security.Naver;
using Owin;
using Users.Infrastructure;

namespace Users
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "131234832742-466n1pgmk5fdipjjkkb6s1u0a5lua41s.apps.googleusercontent.com",
                ClientSecret = "wiXJmXDy8mD4ItNXju0Hm060",
                Provider = new GoogleOAuth2AuthenticationProvider()                
            });

            app.UseNaverAuthentication(new NaverAuthenticationOptions
            {
                ClientId = "70KhhCbGhiwAEDmQbNAF",
                ClientSecret = "ttkmUmbnur",
                Provider = new NaverAuthenticationProvider()
            });
        }
    }
}