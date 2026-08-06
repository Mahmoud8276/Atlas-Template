using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AppUserDtos
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
       
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number!")]
        public string? PhoneNumber { get; set; }
        
        public IFormFile? UserImage { get; set; }
        public bool RemoveImage { get; set; } = false;
    }
}
