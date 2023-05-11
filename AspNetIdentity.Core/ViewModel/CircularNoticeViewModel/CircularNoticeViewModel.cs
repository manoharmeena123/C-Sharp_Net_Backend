using AspNetIdentity.Core.Common;
using System;

namespace AspNetIdentity.Core.ViewModel.CircularNoticeViewModel
{
    public class BaseCircularNoticeViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
    }
    public class ResponseCircularNoticeViewModel
    {
        public class UserViewResponse : BaseCircularNoticeViewModel
        {
            public Guid Id { get; set; } = Guid.Empty;
        }
        public class AdminResponse : UserViewResponse
        {
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset? UpdatedOn { get; set; }
        }
        public class GetCountOnUser
        {
            public int TypeId { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
            public long Count { get; set; } = 0;
        }
    }
    public class RequestCircularNoticeViewModel
    {
        public class CreateRequest : BaseCircularNoticeViewModel { }
        public class UpdateRequest : BaseCircularNoticeViewModel
        {
            public Guid Id { get; set; } = Guid.Empty;
        }
        public class AdminRequest
        {
            public string SearchString { get; set; } = string.Empty;
        }
        public class UserRequest : RequestShortByWithPagging
        {
            public int TypeId { get; set; } = 0;
            public string SearchString { get; set; } = string.Empty;
        }
        public class MultiSelectSelectRequest
        {
            public Guid[] Ids { get; set; }
        }
        public class GetByIdRequest
        {
            public Guid Id { get; set; } = Guid.Empty;
        }
    }
}
