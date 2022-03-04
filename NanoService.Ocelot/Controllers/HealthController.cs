using Microsoft.AspNetCore.Mvc;

namespace NanoService.Ocelot.Controllers
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
