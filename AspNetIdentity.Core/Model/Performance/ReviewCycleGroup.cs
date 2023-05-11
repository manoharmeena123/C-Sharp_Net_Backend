using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class ReviewCycleGroup : BaseModelClass
    {
        [Key]
        public int CycleId { get; set; }
        public Guid? ReviewGroupId { get; set; }
        public Guid? ReviewEmployeeId { get; set; }
        public string ReviewCycleName { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}