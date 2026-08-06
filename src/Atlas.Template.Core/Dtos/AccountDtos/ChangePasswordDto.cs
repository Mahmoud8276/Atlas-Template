using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "New password is required!")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Password Confirmation is required!")]
        [Compare("NewPassword")]
        public string NewPasswordConfirmation { get; set; }
    }
}
