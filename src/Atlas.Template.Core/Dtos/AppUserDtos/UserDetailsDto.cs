using System;
using System.Collections.Generic;

namespace Atlas.Template.Core.Dtos.AppUserDtos
{
    public class UserDetailsDto
    {
        public object Id { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
    }
}
