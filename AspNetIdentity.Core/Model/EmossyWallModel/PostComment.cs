using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmossyWallModel
{
    public class PostComment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WallPostId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
        public string Comment { get; set; } = String.Empty;
        public DateTimeOffset CommentOn { get; set; } = DateTimeOffset.UtcNow;
    }
}