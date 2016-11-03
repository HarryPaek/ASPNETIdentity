using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Users.Infrastructure
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            AppUserManager manager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

            return new MvcHtmlString(manager.FindByIdAsync(id).Result.UserName);
        }

        public static MvcHtmlString GetClaimType(this HtmlHelper html, string claimType)
        {
            FieldInfo[] fields = typeof(ClaimTypes).GetFields();

            foreach (FieldInfo field in fields) {
                if(field.GetValue(null).ToString() == claimType)
                    return new MvcHtmlString(field.Name);
            }

            return new MvcHtmlString(string.Format("{0}", claimType.Split('/', '.').Last()));
        }

        public static MvcHtmlString GetRoleName(this HtmlHelper html, IdentityUserRole userRole)
        {
            return new MvcHtmlString(userRole.GetRoleName());
        }

        public static string GetRoleName(this IdentityUserRole userRole)
        {
            AppRoleManager manager = HttpContext.Current.GetOwinContext().GetUserManager<AppRoleManager>();

            return manager.FindByIdAsync(userRole.RoleId).Result.Name;
        }
    }
}