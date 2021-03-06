﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Users.Infrastructure;
using Users.Models;

namespace Users.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View(GetData("Index"));
        }

        [Authorize(Roles = "Users, Administrators")]  // OR Condition
        //[Authorize(Roles = "Administrators")]       // AND Condition
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("Action", actionName);
            dictionary.Add("User", HttpContext.User.Identity.Name);
            dictionary.Add("Authenticated", HttpContext.User.Identity.IsAuthenticated);
            dictionary.Add("Authentication Type", HttpContext.User.Identity.AuthenticationType);
            dictionary.Add("In Users Role", HttpContext.User.IsInRole("Users"));
            dictionary.Add("In Administrators Role", HttpContext.User.IsInRole("Administrators"));
            dictionary.Add("All User Roles", string.Join(", ", UserManager.FindByName(HttpContext.User.Identity.Name).Roles.Select(r => r.GetRoleName())));

            return dictionary;
        }

        [Authorize]
        public ActionResult UserProps()
        {
            return View(CurrentUser);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserProps(Cities city)
        {
            AppUser user = CurrentUser;
            user.City = city;
            user.SetCountryFromCity(city);
            await UserManager.UpdateAsync(user);

            return View(user);
        }


        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        private AppUser CurrentUser
        {
            get { return UserManager.FindByName(HttpContext.User.Identity.Name); }
        }
    }
}