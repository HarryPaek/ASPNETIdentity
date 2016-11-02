﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Users.Infrastructure;
using Users.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Users.Controllers
{
   [Authorize]
    public class AccountController : Controller
    {
       [AllowAnonymous]
       public ActionResult Login(string returnUrl)
       {
           ViewBag.returnUrl = returnUrl;
           return View();
       }

       [HttpPost]
       [AllowAnonymous]
       [ValidateAntiForgeryToken]
       public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
       {
           if (ModelState.IsValid) {
               AppUser user = await UserManager.FindAsync(details.Name, details.Password);

               if (user == null)
                   ModelState.AddModelError("", "Invalid name or password.");
               else {
                   ClaimsIdentity identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                   AuthManager.SignOut();
                   AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                   return Redirect(returnUrl);
               }
           }

           ViewBag.returnUrl = returnUrl;
           return View(details);
       }

       public ActionResult Logout()
       {
           AuthManager.SignOut();
           return RedirectToAction("Index", "Home");
       }

       private AppUserManager UserManager {
           get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
       }

       private IAuthenticationManager AuthManager {
           get { return HttpContext.GetOwinContext().Authentication; }
       }
    }
}