using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticPosts.Domain
{
    [Table("Posts")]
    public class PostEntity
    {

        public PostEntity()
        {

        }
        public PostEntity(Guid postId, string title, string description)
        {
            PostId = postId;
            Title = title;
            Description = description;
        }

        [Key]
        public Guid PostId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}

