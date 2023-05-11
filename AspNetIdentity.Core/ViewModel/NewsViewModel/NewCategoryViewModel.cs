using System;

namespace AspNetIdentity.Core.ViewModel.NewsViewModel
{
    public class BaseNewsCategoryModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class ResponseNewsCategory
    {
        public class GetNewsCategoryResponse : GetNewsCategoryByIdResponse
        {
            public string CreatedByName { get; set; } = string.Empty;
        }
        public class GetNewsCategoryByIdResponse : BaseNewsCategoryModel
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
            public DateTimeOffset CreatedOn { get; set; }
        }
    }
    public class RequestNewsCategory
    {
        public class CreateNewCateoryRequest : BaseNewsCategoryModel { }
        public class UpdateNewCateoryRequest : BaseNewsCategoryModel
        {
            public Guid CategoryId { get; set; } = Guid.Empty;
        }
        public class MoveToTrashDeleteReQuest
        {
            public Guid[] CategoriesIds { get; set; } = new Guid[0];
        }
    }
}
