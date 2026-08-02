using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class RegisterDto
    {
        [EmailAddress(ErrorMessage = "Invalid Email Address!")]
        [Required(ErrorMessage = "Email Address Is Required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password Confirmation Is Required!")]
        [Compare("Password", ErrorMessage = "Password and Password Confirmation Does not Match!")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "First Name Is Required!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required!")]
        [Phone(ErrorMessage = "Invalid Phone Number!")]
        public string PhoneNumber { get; set; }

        public IFormFile? UserImage { get; set; }
    }
}
