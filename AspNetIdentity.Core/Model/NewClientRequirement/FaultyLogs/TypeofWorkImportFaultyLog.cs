using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs
{
    public class TypeofWorkImportFaultyLog
    {
        [Key]
        public Guid FaultyId { get; set; } = Guid.NewGuid();
        public virtual TypeofWorkImportFaultyLogsGroup Groups { get; set; }
        public int WorktypeId { get; set; } = 0;
        public string WorktypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // public string Code { get; set; } = string.Empty;
        public string FailReason { get; set; } = string.Empty;
    }
}