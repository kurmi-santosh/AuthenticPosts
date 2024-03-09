using System;
namespace AuthenticPosts.Contracts.V1
{
    public class PostRequest
    {
        public required string Title { get; set; }

        public required string Description { get; set; }
    }

    public class UpdateRequest
    {
        public Guid PostId { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

    }
}

