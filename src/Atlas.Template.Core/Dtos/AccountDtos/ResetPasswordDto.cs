using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Reset password token is required!")]
        public string Token { get; set; }

        [Required(ErrorMessage = "User ID is required!")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "New password is required!")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Password Confirmation is required!")]
        [Compare("NewPassword")]
        public string NewPasswordConfirmation { get; set; }
    }
}
