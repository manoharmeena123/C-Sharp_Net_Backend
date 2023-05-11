using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class GoalManagement : BaseModelClass
    {
        [Key]
        public Guid GoalId { get; set; } = Guid.NewGuid();
        public int GoalOwnerId { get; set; } = 0;
        public int DepartmentId { get; set; } = 0;
        public GoalTypeConstants GoalType { get; set; } = GoalTypeConstants.Individual_Goal;
        public GoalCycleConstants GoalCycle { get; set; } = GoalCycleConstants.Monthly;
        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;
        public GoalReviewerType GoalReviewerType { get; set; } = GoalReviewerType.Self;
        public string GoalTitle { get; set; } = string.Empty;
        public string Description { get; set; } = String.Empty;
        public GoalStatusConstantsClass Status { get; set; } = GoalStatusConstantsClass.Pending;
        public GoalResponseConstantsClass GoalResponse { get; set; } = GoalResponseConstantsClass.Approved;
        public string Reason { get; set; } = string.Empty;
        public string GoalUrl { get; set; } = string.Empty;
        public int GoalPercentage { get; set; } = 0;
    }

    /// <summary>
    /// Created By Ankit on 23-01-2023
    /// Its For Identify Goal Cycle and In PT State Duration
    /// </summary>
    public enum GoalCycleConstants
    {
        Monthly = 1,
        Quarterly = 3,
        Half_Yearly = 6,
        Yearly = 12,
        Custom = 0,
    }

    /// <summary>
    /// Created By Ankit on 23-01-2023
    /// Its For Identify Goal Types and In PT State Duration
    /// </summary>
    public enum GoalTypeConstants
    {
        Individual_Goal = 1,
        Departmental_Goal = 2,
        Company_Goal = 3,
    }

    /// <summary>
    /// Created By Ankit on 23-01-2023
    /// Its For Identify Goal Reviewer Type and In PT State Duration
    /// </summary>
    public enum GoalReviewerType
    {
        Self = 1,
        ReportingManager = 2,
        Both = 3,
        Custom = 4,
    }

    /// <summary>
    /// Created by Ankit
    /// Its For Identify GoalStatus of Goal
    /// </summary>
    public enum GoalStatusConstantsClass
    {
        Pending = 1,
        InProgress = 2,
        InReview = 3,
        Completed = 4,
        Reject = 5,
    }

    /// <summary>
    /// Created by Ankit
    /// Its For Identify GoalResponse of Goal
    /// </summary>
    public enum GoalResponseConstantsClass
    {
        Approved = 1,
        Rejected = 2,
    }
}