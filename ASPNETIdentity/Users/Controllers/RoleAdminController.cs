using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Authorize(Roles = "Administrators")]
    public class RoleAdminController : Controller
    {
        // GET: RoleAdmin
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new AppRole(name));

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }

            return View(name);
        }

        public async Task<ActionResult> Edit(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIDs = role.Users.Select(u => u.UserId).ToArray();
            IEnumerable<AppUser> menbers = UserManager.Users.Where(user => memberIDs.Any(userId => userId == user.Id));
            IEnumerable<AppUser> nonMembers = UserManager.Users.Except(menbers);

            return View(new RoleEditViewModel { Role = role, Members = menbers, NonMembers = nonMembers });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleModificationViewModel model)
        {
            IdentityResult result;

            if(ModelState.IsValid) {
                foreach (string userId in model.IdsToAdd ?? new string[] { }) {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);

                    if (!result.Succeeded)
                        return View("Error", result.Errors);
                }

                foreach (string userId in model.IdsToDelete ?? new string[] {}) {
                    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);

                    if (!result.Succeeded)
                        return View("Error", result.Errors);
                }

                return RedirectToAction("Index");
            }

            return View("Error", new string[] { "Role Not Found" });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    return View("Error", result.Errors);
            }
            else
                return View("Error", new string[] { "Role Not Found." });
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }
 
        private AppRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>(); }
        }
    }
}