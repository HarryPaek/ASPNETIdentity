using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Users.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View(GetData("Index"));
        }

        [Authorize(Roles="Users")]
        [Authorize(Roles = "Administrators")]
        public ActionResult OtherAction()
        {
            return View(GetData("OtherAction"));
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("Action", actionName);
            dictionary.Add("User", HttpContext.User.Identity.Name);
            dictionary.Add("Authenticated", HttpContext.User.Identity.IsAuthenticated);
            dictionary.Add("Authentication Type", HttpContext.User.Identity.AuthenticationType);
            dictionary.Add("In Users Role", HttpContext.User.IsInRole("Users"));

            return dictionary;
        }
    }
}