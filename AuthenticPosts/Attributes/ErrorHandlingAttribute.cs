using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthenticPosts.Attributes
{
	public class ErrorHandlingAttribute : ExceptionFilterAttribute
	{
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // .NET Provides us ProblemDetails with contentType problem/json
            var problemDetails = new ProblemDetails()
            {
                Title = "Error occurred",
                Detail = exception.Message,
                Status = 500
            };

            // ObjectResult excepts an object - We can pass ProblemDetails instead of creating our own
            var errorDetails = new ObjectResult(problemDetails);
           
            context.Result = errorDetails;
            context.ExceptionHandled = true;
        }
    }
}

