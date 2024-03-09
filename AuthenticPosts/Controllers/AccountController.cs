using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers
{
	[ApiController]
	public class AccountController: ControllerBase
	{

		[HttpGet("/login")]
		public async Task<IActionResult> Login(SignInManager<IdentityUser> signInManager)
		{
			await signInManager.PasswordSignInAsync("test@gmail.com", "test", false, false);
			return Ok();
		}
	}
}

