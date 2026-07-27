using Microsoft.AspNetCore.Mvc;

namespace Atlas.Template.Api.Controllers
{
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}
