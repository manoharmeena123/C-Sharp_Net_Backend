using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskApprovel : BaseModelClass
    {
        [Key]

        public Guid TaskApprovelId { get; set; } = Guid.NewGuid();
        public int? ProjectId { get; set; }
        public Guid TaskId { get; set; } = Guid.Empty;
        public int LogEmployeeId { get; set; } = 0;
        public string ReEvaluteDiscription { get; set; }
        public string TaskName { get; set; }
        public bool IsSFA { get; set; }
        public string ProjectName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public TimeSpan SpentTime { get; set; }
        //public double SpentTime { get; set; }
        public long EstimateTime { get; set; }
        public TaskRequestConstants TaskRequest { get; set; } = TaskRequestConstants.Not_Selected;
        public bool IsApproved { get; set; }
        public bool IsRe_Evaluate { get; set; }
        public int ProjectManagerId { get; set; } = 0;
        public string DataJson { get; set; }
        public string TotalWorkingTime { get; set; }
    }
}