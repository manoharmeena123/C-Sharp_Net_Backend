using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class ReviewsGroup : BaseModelClass
    {
        [Key]
        public Guid ReviewGroupId { get; set; } = Guid.NewGuid();
        public string ReviewGroupName { get; set; }
        public int? EmployeeId { get; set; }
        public string Description { get; set; }
        public ReviewCycleConstants ReviewCycle { get; set; }
        public FrequenceConstants FrequenceReview { get; set; }
        public DateTime? StartDate { get; set; }
        public string ManageGroup { get; set; }
    }
}