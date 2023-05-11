using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmossyWallModel
{
    public class PostMention
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WallPostId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
    }
}