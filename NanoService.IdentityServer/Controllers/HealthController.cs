using Microsoft.AspNetCore.Mvc;

namespace NanoService.IdentityServer.Controllers
{
    [Route("api/health")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
