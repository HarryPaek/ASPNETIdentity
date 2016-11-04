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
           if (HttpContext.User.Identity.IsAuthenticated) {
               return View("Error", new string[] { "Access Denied" });
           }

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
                   identity.AddClaims(LocationClaimsProvider.GetClaims(identity));
                   identity.AddClaims(ClaimsRoles.CreateRolesFromClaims(identity));
                   AuthManager.SignOut();
                   AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                   return Redirect(returnUrl ?? "/");
               }
           }

           ViewBag.returnUrl = returnUrl;
           return View(details);
       }


       #region External Login

       [HttpPost]
       [AllowAnonymous]
       [ValidateAntiForgeryToken]
       public ActionResult GoogleLogin(string returnUrl)
       {
           // Request a redirect to the external login provider
           return new ChallengeResult("Google", Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
       }

       [HttpPost]
       [AllowAnonymous]
       [ValidateAntiForgeryToken]
       public ActionResult NaverLogin(string returnUrl)
       {
           // Request a redirect to the external login provider
           return new ChallengeResult("Naver", Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
       }

       [AllowAnonymous]
       public ActionResult ExternalLoginCallbackRedirect(string returnUrl)
       {
           return RedirectPermanent("/Account/ExternalLoginCallback");
       }

       [AllowAnonymous]
       public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
       {
           ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();

           if (loginInfo == null)
               return RedirectToAction("Login");

           AppUser user = await UserManager.FindAsync(loginInfo.Login);

           if (user == null) {
               user = new AppUser
               {
                   Email = loginInfo.Email,
                   UserName = loginInfo.DefaultUserName,
                   City = Cities.SEOUL,
                   Country = Countries.KOREA
               };

               IdentityResult result = await UserManager.CreateAsync(user);
               if (!result.Succeeded)
                   return View("Error", result.Errors);
               else {
                   result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                   if (!result.Succeeded)
                       return View("Error", result.Errors);
               }
           }

           ClaimsIdentity identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
           identity.AddClaims(loginInfo.ExternalIdentity.Claims);
           AuthManager.SignOut();
           AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

           return Redirect(returnUrl ?? "/");
       }

       #endregion


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

   internal class ChallengeResult : HttpUnauthorizedResult
   {
       private const string XsrfKey = "XsrfId";

       public ChallengeResult(string provider, string redirectUri): this(provider, redirectUri, null)
       {
       }

       public ChallengeResult(string provider, string redirectUri, string userId)
       {
           LoginProvider = provider;
           RedirectUri = redirectUri;
           UserId = userId;
       }

       public string LoginProvider { get; set; }
       public string RedirectUri { get; set; }
       public string UserId { get; set; }

       public override void ExecuteResult(ControllerContext context)
       {
           var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
           if (UserId != null)
               properties.Dictionary[XsrfKey] = UserId;

           context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
       }
   }
}