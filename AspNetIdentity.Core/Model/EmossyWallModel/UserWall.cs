using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmossyWallModel
{
    public class UserWall : BaseModelClass
    {
        [Key]
        public Guid WallPostId { get; set; } = Guid.NewGuid();
        public WallPostType PostType { get; set; } = WallPostType.Post;
        public string Details { get; set; } = String.Empty;
        public string ImageURl { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public bool NotifyToAllEmployees { get; set; } = false;
    }
    public enum WallPostType
    {
        Post = 1,
        Annoucement = 2,
    }
}