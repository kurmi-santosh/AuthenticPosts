using System;
namespace AuthenticPosts.Contracts.V1
{
    public static class APIRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/posts";

            public const string GetPostById = Base + "/posts/{postId}";

            public const string CreatePost = Base + "/posts/CreatePost";

            public const string UpdatePost = Base + "/posts/{postId}";

            public const string DeletePost = Base + "/posts/{postId}";

        }
    }
}

