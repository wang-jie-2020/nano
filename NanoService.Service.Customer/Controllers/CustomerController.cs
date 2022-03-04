using Microsoft.AspNetCore.Mvc;

namespace NanoService.Service.Customer.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        [HttpGet]
        [Route("hello")]
        public IActionResult HelloCustomer()
        {
            var connection = Request.HttpContext.Connection;

            var ip = connection.LocalIpAddress?.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort;
            return Content(ip);
        }
    }
}
