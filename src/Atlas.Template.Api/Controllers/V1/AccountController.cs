using Asp.Versioning;
using Atlas.Template.Core.Dtos.AccountDtos;
using Atlas.Template.Services.IServices;
using Microsoft.AspNetCore.Mvc;
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
            return StatusCode((int)result.StatusCode, result);
        }


        // POST api/v1/account/confirm-email
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
