﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NanoService.Service.Product.Controllers
{
    [Route("api/product")]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Hello()
        {
            return HelloProduct();
        }

        [HttpGet]
        [Route("hello")]
        public IActionResult HelloProduct()
        {
            var connection = Request.HttpContext.Connection;
            var user = Request.HttpContext.User.Identity?.Name ?? "未知用户";
            var address = connection.LocalIpAddress?.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort;

            return Content($"{user}访问的是{address}");
        }
    }
}
