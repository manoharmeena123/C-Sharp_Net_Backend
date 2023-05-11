using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskLog : BaseModelClass
    {
        [Key]
        public Guid TaskLogId { get; set; } = Guid.NewGuid();
        public int? ProjectId { get; set; } = 0;
        public Guid TaskId { get; set; } = Guid.Empty;
        public TaskStatusConstants Status { get; set; }
        public int LogEmployeeId { get; set; } = 0;
        public int TaskIdNumber { get; set; } = 0;
        public DateTimeOffset DueDate { get; set; }
        public int Percentage { get; set; } = 0;
        //public TimeSpan SpentTime { get; set; }
        public TimeSpan SpentTime { get; set; } = TimeSpan.Zero;
        public string Image1 { get; set; } = String.Empty;
        public string Image2 { get; set; } = String.Empty;
        public string Image3 { get; set; } = String.Empty;
        public string Image4 { get; set; } = String.Empty;
        public string Image5 { get; set; } = String.Empty;
        public string Comment { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public string TaskImg { get; set; } = String.Empty;
        public bool IsApproved { get; set; }
    }
}