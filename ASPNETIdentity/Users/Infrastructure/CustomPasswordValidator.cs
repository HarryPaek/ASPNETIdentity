using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Users.Infrastructure
{
    public class CustomPasswordValidator : PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string password)
        {
            IdentityResult result = await base.ValidateAsync(password);

            if(password.Contains("12345")) {
                var errors = result.Errors.ToList();
                errors.Add("Passwords cannot contain numeric sequence");
                result = new IdentityResult(errors);
            }

            return result;
        }
    }
}