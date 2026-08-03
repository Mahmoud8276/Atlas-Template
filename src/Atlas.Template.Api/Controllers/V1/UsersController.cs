using Asp.Versioning;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Atlas.Template.Api.Controllers.V1
{
    //[Authorize(Roles = "Admin")]
    [ApiVersion(1.0)]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        // GET api/v1/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserSpecParams specParams)
        {
            var result = await _userService.GetAllAsync(specParams);
            return StatusCode((int)result.StatusCode, result);
        }

        // GET api/v1/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
