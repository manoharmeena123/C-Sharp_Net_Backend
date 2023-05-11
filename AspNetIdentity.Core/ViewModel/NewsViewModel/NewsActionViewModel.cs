using AspNetIdentity.Core.Common;
using System;

namespace AspNetIdentity.Core.ViewModel.NewsViewModel
{
    public class BaseNewsActionClass
    {
        public Guid NewsId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
    public class NewsActionViewRequest
    {
        public class PublishNewsFilterRequest : RequestShortByWithPagging
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
            public string SearchString { get; set; } = string.Empty;
        }
        public class GetTopNewsRequest
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class ByIdClassRequest
        {
            public Guid NewsId { get; set; } = Guid.Empty;
        }
    }
    public class NewsActionViewResponse
    {
        public class GetNewsCategoriesCountResponse
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public long Count { get; set; } = 0;
        }
        public class GetPublishNewsResponse : BaseNewsActionClass
        {
            public string CategoryName { get; set; } = string.Empty;
            public DateTimeOffset CreatedOn { get; set; }
            public long LikeCount { get; set; } = 0;
            public bool IsLikedByUser { get; set; } = false;
        }
    }
}
