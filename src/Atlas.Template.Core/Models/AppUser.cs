using Microsoft.AspNetCore.Identity;
using System;

namespace Atlas.Template.Core.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Image { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
    }
}
