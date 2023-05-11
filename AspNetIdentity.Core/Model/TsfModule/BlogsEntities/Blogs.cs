using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule
{
    public class Blogs : BaseModelClass
    {
        [Key]
        public Guid BlogId { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public BlogsTypeEnumClass BlogsStatus { get; set; } = BlogsTypeEnumClass.In_DASH_Review;
        public BlogStatusEnumClass StatusType { get; set; } = BlogStatusEnumClass.Still_Pending;
        public DateTimeOffset ActionDate { get; set; } = DateTimeOffset.UtcNow;
        public int ActionBy { get; set; } = 0;
        public string ReasonOfRejection { get; set; } = string.Empty;
    }
}
