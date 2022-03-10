using System.Net;
using Microsoft.AspNetCore.Mvc;
using NanoService.Consumer.Models;

namespace NanoService.Consumer.Controllers
{
    [Route("api/consumer")]
    public class ConsumerController : Controller
    {
        private readonly SampleDbContext _dbContext;

        public ConsumerController(SampleDbContext sampleDbContext)
        {
            _dbContext = sampleDbContext;
        }

        [HttpGet]
        public IActionResult Hello()
        {
            _dbContext.Applications.Add(new Application()
            {
                Id = new Random().Next(1, 1000),
                Name = new Random().Next(1, 1000).ToString()
            });

            return HelloConsumer();
        }

        [HttpGet]
        [Route("hello")]
        public IActionResult HelloConsumer()
        {
            var connection = Request.HttpContext.Connection;
            var user = Request.HttpContext.User.Identity?.Name ?? "未知用户";
            var address = connection.LocalIpAddress?.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort;

            return Content($"{user}访问的是{address}");
        }

        [HttpGet]
        [Route("hello-trace")]
        public IActionResult HelloTrace()
        {
            var httpClient = new HttpClient();

            var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://vm.local.cn:5511/api/product"),
                Headers = { }
            };
            httpClient.Send(req);

            req = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://vm.local.cn:5510/api/customer"),
                Headers = { }
            };
            httpClient.Send(req);

            return Ok();
        }
    }
}
