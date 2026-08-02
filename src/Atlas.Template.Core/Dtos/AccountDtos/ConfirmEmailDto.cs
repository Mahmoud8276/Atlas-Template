using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User Id is required!")]
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "Confirm email token is required!")]
        public string Token { get; set; }
    }
}
