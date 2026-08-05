using Asp.Versioning;
using Atlas.Template.Api.Responses;
using Atlas.Template.Core.Dtos.AccountDtos;
using Atlas.Template.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Atlas.Template.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/v1/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var result = await _accountService.RegisterUserAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }


        // POST api/v1/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _accountService.LoginUserAsync(dto);
            if(result.IsSuccess)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
            
            return StatusCode((int)result.StatusCode, result);
        }


        // GET api/v1/account/confirm-email
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/v1/account/confirm-email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var result = await _accountService.SendConfirmEmailAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/v1/account/forget-password
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            var result = await _accountService.ForgetPasswordAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/v1/account/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _accountService.ResetPasswordAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        // GET api/v1/account/refresh-token
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            //if (string.IsNullOrEmpty(refreshToken))
                return StatusCode(400, ApiResponse.Fail("Refresh token is required."));

            var result = await _accountService.RefreshTokenAsync(refreshToken);
            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
            
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/v1/account/revoke-token
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        {
            var refreshToken = dto.RefreshToken ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return StatusCode(400, ApiResponse.Fail("Refresh token is required."));

            var result = await _accountService.RevokeRefreshTokenAsync(refreshToken);
            return StatusCode((int)result.StatusCode, result);
        }



        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
