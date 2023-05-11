using AspNetIdentity.Core.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.ViewModel.TsfViewModel
{
    public class BaseNewsServiceClass
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
    public class RequestNewsServiceClass
    {
        public class CreateNewsRequestClass : BaseNewsServiceClass
        {
            public NewsEnumType NewsType { get; set; }
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class UpdateNewsClassRequest : BaseNewsServiceClass
        {
            public NewsEnumType NewsType { get; set; }
            public Guid NewsId { get; set; }
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class SearchStringRequest
        {
            public string SearchString { get; set; } = string.Empty;
        }
        public class GetNewsAdminViewRequest : SearchStringRequest
        {
            public NewsEnumType NewsType { get; set; }
        }
        public class MoveToTrashOrDeleteRequest
        {
            public Guid[] NewsIds { get; set; }
        }
    }
    public class ResponseNewsServiceClass
    {
        public class GetNewsAdminResponse : BaseNewsServiceClass
        {
            public Guid NewsId { get; set; } = Guid.Empty;
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset? UpdatedOn { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public bool IsFeatured { get; set; } = false;
        }

        public class GetByNewsResponseClass : BaseNewsServiceClass
        {
            public Guid NewsId { get; set; } = Guid.Empty;
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
    }
}
