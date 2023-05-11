using System;

namespace AspNetIdentity.Core.ViewModel.BlogsViewModel
{
    public class BaseBlogCategoryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class ResponseBlogCategory
    {
        public class GetBlogCategoriesResponse : BaseBlogCategoryViewModel
        {
            public Guid CategoryId { get; set; }
        }
        public class GetCategoryWithBlogsCountResponse
        {
            public Guid CategoryId { get; set; }
            public string CategoryName { get; set; }
            public long Count { get; set; }
        }
    }
    public class RequestBlogCategory
    {
        public class CreateBlogCategoryRequest : BaseBlogCategoryViewModel { }
        public class UpdateBlogCategoryRequest : BaseBlogCategoryViewModel
        {
            public Guid CategoryId { get; set; }
        }
    }
}
