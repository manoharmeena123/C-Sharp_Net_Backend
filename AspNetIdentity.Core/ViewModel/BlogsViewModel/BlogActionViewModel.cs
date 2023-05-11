using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using System;

namespace AspNetIdentity.Core.ViewModel.TsfViewModel
{
    internal class BaseBlogsActions
    {
    }
    public class ResponseBlogsActions
    {
        public class GetBlogListInReview
        {
            public Guid BlogId { get; set; } = Guid.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Image { get; set; } = string.Empty;
            public DateTimeOffset InReviewDate { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }
        public class GetLiveBlogListResponse : GetBlogListInReview
        {
            public string CreatedByName { get; set; } = string.Empty;
            public DateTimeOffset ActionDate { get; set; } = DateTimeOffset.UtcNow;
            public long LikeCount { get; set; } = 0;
            public bool IsLikedByUser { get; set; } = false;
        }
    }
    public class RequestBlogsActions
    {
        public class BlogChangeStatusRequest
        {
            public Guid BlogId { get; set; } = Guid.Empty;
            public BlogStatusEnumClass StatusType { get; set; } = BlogStatusEnumClass.Still_Pending;
            public string ReasonOfRejection { get; set; }
        }
        public class MoveBlogToTrashRequest
        {
            public Guid[] BlogsIds { get; set; }
        }
        public class ApproveOrRejectMultipleRequest : MoveBlogToTrashRequest
        {
            public BlogStatusEnumClass StatusType { get; set; } = BlogStatusEnumClass.Still_Pending;
        }
        public class BlogLiveViewRequest : RequestShortByWithPagging
        {
            public string SearchString { get; set; } = string.Empty;
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class InReviewRequest
        {
            public string SearchString { get; set; } = string.Empty;
        }
    }
}
