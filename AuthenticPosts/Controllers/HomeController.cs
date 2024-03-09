using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers
{
	[ApiController]
	[Route("homes")]
	public class HomeController : ControllerBase
	{
		[HttpGet("/")]
		public String Index()
		{
			return "Index Route";
		}

        [HttpGet("{id}")]
        public String Test(string id)
        {
			return $"Test - {id}";
        }

        [HttpGet("/secret")]
		[Authorize(Roles ="admin")]
		public String Secret()
		{
			return "Secret route";
		}
	}
}

