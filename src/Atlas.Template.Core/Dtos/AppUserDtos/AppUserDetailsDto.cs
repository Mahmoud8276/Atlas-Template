using System;

namespace Atlas.Template.Core.Dtos.AppUserDtos
{
    public class AppUserDetailsDto
    {
        public object Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
    }
}
