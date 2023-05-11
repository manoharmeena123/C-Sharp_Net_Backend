using AspNetIdentity.Core.Enum;
using System.Collections.Generic;

namespace AspNetIdentity.Core.ViewModel.TicketViewModel
{
    public class BaseTicketCategoryClass
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    #region Request View Model
    public class RequestTicketCategory
    {
        public class CreateTicketCategoryRequest : BaseTicketCategoryClass
        {
            public List<int> Employees { get; set; } = new List<int>();
            public List<RequestTicketProprities> Priorities { get; set; } = new List<RequestTicketProprities>();
        }
        public class RequestTicketProprities
        {
            public PriorityTypeEnum PriorityType { get; set; }
            public bool IsRequired { get; set; } = false;
        }
    }
    public class ResponseTicketCategory
    {

    }
    #endregion
}
