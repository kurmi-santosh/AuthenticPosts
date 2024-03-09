using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers
{
	[ApiController]
	public class HomeController : ControllerBase
	{
		[HttpGet("/")]
		public String Index()
		{
			return "Index Route";
		}

		[HttpGet("/secret")]
		[Authorize(Roles ="admin")]
		public String Secret()
		{
			return "Secret route";
		}
	}
}

