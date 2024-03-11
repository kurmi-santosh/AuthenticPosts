using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers
{
	[ApiController]
	[Route("home")]
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

		[HttpGet("/exception")]
		public int Exception()
		{
			throw new NotImplementedException();
		}
	}
}

