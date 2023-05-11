using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    public class TaskImportFaultyLogs : BaseModelClass
    {
        [Key]
        public Guid TaskFaultyLogId { get; set; } = Guid.NewGuid();
        public virtual TaskImportFaultyLogsGroup Group { get; set; }
        public string ProjectName { get; set; } = String.Empty;
        public string TaskTitle { get; set; } = String.Empty;
        public string Discription { get; set; } = String.Empty;
        public string Priority { get; set; } = String.Empty;
        public string TaskType { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public string BillingStatus { get; set; } = String.Empty;
        public int TaskIdNumber { get; set; } = 0;
        public string EmployeeName { get; set; } = String.Empty;
        public string Attechment { get; set; } = String.Empty;
        public long EstimateTime { get; set; }
        public TimeSpan SpentTime { get; set; } = TimeSpan.Zero;
        //public double SpentTime { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string FailReason { get; set; } = String.Empty;
        public string OfficalEmail { get; set; } = String.Empty;
        public string SprintName { get; set; } = string.Empty;
    }
}