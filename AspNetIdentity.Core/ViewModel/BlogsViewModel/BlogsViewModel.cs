using AspNetIdentity.Core.Enum;
using System;

namespace AspNetIdentity.Core.ViewModel.TsfViewModel
{
    public class BlogBaseClass
    {

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public BlogsTypeEnumClass BlogsStatus { get; set; } = BlogsTypeEnumClass.In_DASH_Review;
    }
    public class BlogRequestClass
    {

        public class CreateBlogRequestclass : BlogBaseClass
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class UpdateBlogRequestclass : CreateBlogRequestclass
        {
            public Guid BlogId { get; set; }
        }
        public class GetRequestClass
        {
            public string SearchString { get; set; } = string.Empty;
            public BlogsTypeEnumClass? BlogType { get; set; }
            public ShortByEnumClass? ShortType { get; set; }
        }
        public class DeleteBlogRequest
        {
            public Guid[] BlogsIds { get; set; }
        }
        public class ResquestBlogsPreview : BlogBaseClass
        {
            public string CategoryId { get; set; }
        }
    }
    public class BlogResponseClass
    {
        public class GetBlogResponseByIdClass : BlogBaseClass
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
            public Guid BlogId { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
        }
        public class GetBlogListPagingResponse : GetBlogResponseByIdClass
        {
            public string BlogStatusName { get; set; }
            public string CategoryName { get; set; }
            public DateTimeOffset? UpdatedDate { get; set; }
        }

        public class GetBlogHeaderResponse : BlogsResponseEnumData
        {
            public long Count { get; set; }
        }
        public class BlogsResponseEnumData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class PreviewBlogResponse : BlogBaseClass
        {
            public Guid BlogId { get; set; }
            public string CategoryName { get; set; }
            public DateTimeOffset CreateOn { get; set; }
            public string CreatedByName { get; set; }
        }
        public class GetMyBlogListResponse : BlogBaseClass
        {
            public Guid BlogId { get; set; }
            public string CategoryName { get; set; }
            public DateTimeOffset CreateOn { get; set; }
        }
    }
}
