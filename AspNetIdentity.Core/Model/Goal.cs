using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class Goal : DefaultFields
    {
        [Key]
        public int GoalId { get; set; }

        public string GoalName { get; set; }
        public GoalTypeEnum_PTStateDuration GoalType { get; set; }
        public string Description { get; set; }
        public int AssignToId { get; set; }
        public int AssignById { get; set; }
        public GoalStatusConstants Status { get; set; }
        public string GoalDocuments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; }
        public string ReasonToExtenGoal { get; set; }
        public bool IsRejected { get; set; }
        public bool IsExtended { get; set; }
        public int GoalPercentage { get; set; }
    }
}