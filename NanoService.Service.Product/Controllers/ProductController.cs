using Microsoft.AspNetCore.Mvc;

namespace NanoService.Service.Product.Controllers
{
    [Route("api/product")]
    public class ProductController : Controller
    {
        [HttpGet]
        [Route("hello")]
        public IActionResult HelloProduct()
        {
            var connection = Request.HttpContext.Connection;

            var ip = connection.LocalIpAddress?.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort;
            return Content(ip);
        }
    }
}
