using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Users.Models
{
    public class AppUser : IdentityUser
    {
        public Cities City { get; set; }
    }

    public enum Cities
    {
        LONDON,
        PARIS,
        CHICAGO,
        SEOUL
    }
}