using Microsoft.AspNetCore.Mvc;

namespace NanoService.Service.Customer.Controllers
{
    [Route("CustomerService/api/customer")]
    public class CustomerController : Controller
    {
        [HttpGet]
        [Route("hello")]
        public IActionResult HelloCustomer()
        {
            var connection = Request.HttpContext.Connection;
            var user = Request.HttpContext.User.Identity?.Name ?? "未知用户";
            var address = connection.LocalIpAddress?.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort;

            return Content($"{user}访问的是{address}");
        }
    }
}
