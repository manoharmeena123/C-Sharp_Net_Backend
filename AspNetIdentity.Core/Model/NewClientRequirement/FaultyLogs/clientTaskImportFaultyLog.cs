using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs
{
    public class clientTaskImportFaultyLog : BaseModelClass
    {
        [Key]
        public Guid FaultyId { get; set; } = Guid.NewGuid();
        public virtual TypeofWorkImportFaultyLogsGroup Groups { get; set; }
        public string FailReason { get; set; } = string.Empty;
        public Guid ClientTaskId { get; set; } = Guid.Empty;
        public Guid ClientId { get; set; }
        public string ClientCode { get; set; }
        public DateTimeOffset Taskdate { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public Guid WorkTypeId { get; set; }
        public string Description { get; set; }
    }
}