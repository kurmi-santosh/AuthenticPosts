using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers
{
	[ApiController]
	public class AccountController: ControllerBase
	{

		[HttpGet("/login")]
		public IActionResult Login()
		{
			var user = new ClaimsPrincipal(
				new ClaimsIdentity(
					new Claim[]
                    {
						new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
						new Claim("my_admin_role", "admin") 
					},
					"cookie",
					nameType: null,
					roleType: "my_admin_role" // This roleType is the actual pointer to the role claim
                )
				
			);

			return SignIn(user, authenticationScheme:"cookie");
		}
	}
}

