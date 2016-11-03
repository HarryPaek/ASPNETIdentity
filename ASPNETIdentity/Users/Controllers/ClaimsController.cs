using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Users.Infrastructure;

namespace Users.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
                return View("Error", new string[] { "No claims available" });
            else
                return View(identity.Claims);
        }

        //[Authorize(Roles="DCStaff")]
        [ClaimsAccess(Issuer="RemoteClaims", ClaimType=ClaimTypes.PostalCode, Value="DC 20500")]
        public string OtherAction()
        {
            return "This is the protected action";
        }
    }
}