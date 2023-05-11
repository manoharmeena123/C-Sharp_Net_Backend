using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet.History
{
    public class TaskModelHistory : BaseModelClass
    {
        [Key]
        public Guid TaskHistoryId { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; } = Guid.Empty;
        public string TaskCode { get; set; } = String.Empty;
        public int ProjectId { get; set; }
        public string TaskTitle { get; set; }
        public string Discription { get; set; }
        public TaskPriorityConstants Priority { get; set; } = TaskPriorityConstants.Medium;
        public TaskTypeConstants TaskType { get; set; } = TaskTypeConstants.BackLog;
        public TaskStatusConstants Status { get; set; } = TaskStatusConstants.Pending;
        public Guid TaskTypeId { get; set; } = Guid.Empty;
        public TaskBillingConstants TaskBilling { get; set; } = TaskBillingConstants.Non_Billable;
        public int TaskIdNumber { get; set; } = 0;
        public int? AssignEmployeeId { get; set; } = 0;
        public int Percentage { get; set; } = 0;
        public string Attechment { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public long EstimateTime { get; set; } = 0;
        public DateTimeOffset? StartDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsMail { get; set; } = true;
        public string TaskURL { get; set; } = string.Empty;
    }
}