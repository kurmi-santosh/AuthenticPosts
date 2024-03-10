using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticPosts.Contracts.V1;
using AuthenticPosts.Domain;
using AuthenticPosts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthenticPosts.Attributes;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticPosts.Controllers.V1
{
    
    public class PostsController : Controller
    {
        readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [Route(APIRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAllPosts()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route(APIRoutes.Posts.GetPostById)]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post != null) return Ok(post);
            else return NotFound();
        }

        [HttpPost]
        [Route(APIRoutes.Posts.CreatePost)]
        public async Task<IActionResult> CreatePost([FromBody] PostRequest request)
        {
            var post = new PostEntity(Guid.NewGuid(), request.Title, request.Description);
            await _postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + APIRoutes.Posts.GetPostById.Replace("{postId}", post.PostId.ToString());
            return Created(locationUrl, post);
        }

        [HttpPut]
        [Route(APIRoutes.Posts.UpdatePost)]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateRequest request)
        {
            var postEntity = new PostEntity { PostId = request.PostId, Title = request.Title, Description = request.Description };
            var updated = await _postService.UpdatePostAsync(postEntity);
            if (updated) return Ok(postEntity);
            else return NotFound();
        }

        [HttpDelete]
        [Route(APIRoutes.Posts.DeletePost)]
        public async Task<IActionResult> DeletPost(Guid postId)
        {
            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted) return NoContent();
            else return NotFound();
        }
    }
}

