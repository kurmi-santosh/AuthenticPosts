using System;
using Microsoft.EntityFrameworkCore;
using AuthenticPosts.Domain;
using AuthenticPosts.Data;

namespace AuthenticPosts.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<PostEntity>> GetAllPostsAsync()
        {
            return await _dataContext.Posts.ToListAsync();
        }

        public async Task<PostEntity> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(item => item.PostId == postId);
        }

        public async Task<bool> CreatePostAsync(PostEntity post)
        {
            await _dataContext.Posts.AddAsync(post);
            var updatedRows = await _dataContext.SaveChangesAsync();
            return updatedRows > 0;
        }

        public async Task<bool> UpdatePostAsync(PostEntity postToUpdate)
        {
            _dataContext.Posts.Update(postToUpdate);
            var updatedRows = await _dataContext.SaveChangesAsync();
            return updatedRows > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var postToDel = await GetPostByIdAsync(postId);
            _dataContext.Remove(postToDel);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
    }
}

