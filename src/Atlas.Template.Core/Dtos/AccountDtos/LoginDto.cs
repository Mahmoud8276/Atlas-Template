using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage = "Invalid Email!")]
        [Required(ErrorMessage = "Email Is Required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
