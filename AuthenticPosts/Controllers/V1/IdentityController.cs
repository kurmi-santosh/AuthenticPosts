using System;
using AuthenticPosts.Contracts.V1;
using AuthenticPosts.Contracts.V1.Requests;
using AuthenticPosts.Contracts.V1.Responses;
using AuthenticPosts.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticPosts.Controllers.V1
{
	public class IdentityController: Controller
	{
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
		{
            _identityService = identityService;
        }

        [HttpPost(APIRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                var failedResponse = new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                };
                return BadRequest(failedResponse);
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpPost(APIRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserRegistrationRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                var failedResponse = new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                };
                return BadRequest(failedResponse);
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}

