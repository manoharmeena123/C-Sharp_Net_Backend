using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskModel : BaseModelClass
    {
        [Key]

        public Guid TaskId { get; set; } = Guid.NewGuid();
        public string TaskCode { get; set; } = String.Empty;
        public int ProjectId { get; set; } = 0;
        // public int SubProjectId { get; set; } = 0;
        public string TaskTitle { get; set; } = String.Empty;
        public string ProjectTaskId { get; set; } = String.Empty;
        public string Discription { get; set; } = String.Empty;
        public TaskPriorityConstants Priority { get; set; } = TaskPriorityConstants.Medium;
        public TaskTypeConstants TaskType { get; set; } = TaskTypeConstants.BackLog;
        public TaskStatusConstants Status { get; set; } = TaskStatusConstants.Pending;
        public Guid TaskTypeId { get; set; } = Guid.Empty;
        public Guid SprintId { get; set; } = Guid.Empty;
        public TaskBillingConstants TaskBilling { get; set; } = TaskBillingConstants.Non_Billable;
        public int TaskIdNumber { get; set; } = 0;
        public int? AssignEmployeeId { get; set; } = 0;
        public int Percentage { get; set; } = 0;
        public string Attechment { get; set; } = String.Empty;
        public string Image1 { get; set; } = String.Empty;
        public string Image2 { get; set; } = String.Empty;
        public string Image3 { get; set; } = String.Empty;
        public string Image4 { get; set; } = String.Empty;
        public long EstimateTime { get; set; } = 0;
        public DateTimeOffset? StartDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsMail { get; set; } = true;
        public string TaskURL { get; set; } = string.Empty;
        //public string TId { get; set; }
    }
    public enum TaskBillingConstants
    {
        Billable = 1,
        Non_Billable = 2,
    }
    /// <summary>
    /// Created By Ravi Vyas On 08-12-2022
    /// </summary>
    public enum TaskStatusConstants
    {
        Pending = 1,
        In_Progress = 2,
        Resolved = 3,
        Re_Open = 4,
        Closed = 5,
    }
    /// <summary>
    /// Created By Ravi Vyas On 08-12-2022
    /// </summary>
    public enum TaskTypeConstants
    {
        BackLog = 0,
        Task = 1,
        Issue = 2,
        Bug = 3,
        Leave = 4,
        Break = 5,
        Deployment = 6,


    }
    /// <summary>
    /// Created By Ravi Vyas On 08-12-2022
    /// </summary>
    public enum TaskPriorityConstants
    {
        Urgent = 1,
        High = 2,
        Medium = 3,
        Low = 4,
    }
}