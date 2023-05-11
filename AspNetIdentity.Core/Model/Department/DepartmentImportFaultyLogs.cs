using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class DepartmentImportFaultyLogs
    {
        [Key]
        public Guid DepartmentFaultyId { get; set; }
        public virtual DepartmentImportFaultyLogsGoups DepartmentGroups { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string Description { get; set; }
        public bool UsedForLogin { get; set; }
        public string FailReason { get; set; }
    }
}