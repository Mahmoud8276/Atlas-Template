using System.ComponentModel.DataAnnotations;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Client URL is required!")]
        [Url(ErrorMessage = "Invalid URL!")]
        public string ClientUrl { get; set; }
    }
}
