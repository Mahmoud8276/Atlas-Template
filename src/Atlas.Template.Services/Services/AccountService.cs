using Atlas.Template.Core.Dtos.AccountDtos;
using Atlas.Template.Core.Dtos.AppUserDtos;
using Atlas.Template.Core.Enums;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.Emails;
using Atlas.Template.Services.Helpers;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.Responses;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Services
{

    // TODO: Wrap all the service methods in a DB transactions,
    // so that if any operation fails, the entire transaction
    // is rolled back to maintain data integrity.
    public class AccountService : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountService(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
        }

        private async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        private async Task<string> GenerateConfirmEmailUrlAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = _configuration["Storage:BaseUrl"];
            var confirmEmailUrl = $"{baseUrl}/api/v1/account/confirm-email?userId={user.Id}&token={encodedToken}";
            return confirmEmailUrl;
        }

        private async Task<string> GenerateForgetPasswordUrlAsync(AppUser appUser, string clientUrl)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var forgetPasswordUrl = $"{clientUrl}?userId={appUser.Id}&token={encodedToken}";
            return forgetPasswordUrl;
        }



        public async Task<Response> RegisterUserAsync(RegisterDto dto)
        {
            if (await UserExistsAsync(dto.Email))
                return Response.Fail("User Already Exists",
                    (int)HttpStatusCode.BadRequest);

            var appUser = new AppUser()
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber
            };

            if(dto.UserImage != null) 
            {
                var imageName = await FileHelper.UploadFile(dto.UserImage, "UserImages");
                appUser.Image = imageName;
            }

            var result = await _userManager.CreateAsync(appUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail("User registration failed", 
                    (int)HttpStatusCode.BadRequest, $"{errors}");
            }

            result = await _userManager.AddToRoleAsync(appUser, AppUserRoles.User.ToString());
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail("Failed to assign role to user",
                    (int)HttpStatusCode.InternalServerError, $"{errors}");
            }

            await _emailService.SendAsync(new ConfirmAccountEmail(
                to: appUser.Email,
                recipientName: $"{appUser.FirstName} {appUser.LastName}",
                firstName: appUser.FirstName,
                emailConfirmationLink: await GenerateConfirmEmailUrlAsync(appUser)
                ));

            return Response.Success(message: "registered successfully, please check your email to confirm your account");
        }

        public async Task<Response> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user is null)
                return Response.Fail("Invalid Email Or Password!",
                    (int)HttpStatusCode.Unauthorized);

            if(!user.EmailConfirmed)
                return Response.Fail("Email is not confirmed!",
                    (int)HttpStatusCode.Unauthorized);


            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if(!signInResult.Succeeded)
                return Response.Fail("Invalid Email Or Password!",
                    (int)HttpStatusCode.Unauthorized);

            return Response.Success(new
            {
                user = user.Adapt<AppUserDetailsDto>(),
                token = await _tokenService.GenerateAccessTokenAsync(user)
            });
        }

        public async Task<Response> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response.Fail("User not found", (int)HttpStatusCode.NotFound);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail("Email confirmation failed", (int)HttpStatusCode.BadRequest, $"{errors}");
            }

            return Response.Success(message:"Email confirmed successfully");
        }

        public async Task<Response> ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                // We are returning success here for not leaking the registered emails for unauthorized users.
                return Response.Success(message: "Check your email");
            }

            await _emailService.SendAsync(new ForgetPasswordEmail(
                to: user.Email,
                recipientName: user.FirstName,
                resetLink: await GenerateForgetPasswordUrlAsync(user, dto.ClientUrl)
            ));

            return Response.Success(message: "Plase, check your inbox");
        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return Response.Fail("User not found", (int)HttpStatusCode.NotFound);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail("Password reset failed", (int)HttpStatusCode.BadRequest, $"{errors}");
            }

            return Response.Success(message: "Password has been reset successfully!");
        }
    }
}
