using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class DesignationImportFaultyLogs
    {
        [Key]
        public Guid DepartmentFaultyId { get; set; }
        public virtual DepartmentImportFaultyLogsGoups DepartmentGroups { get; set; }

        public int DesignationId { get; set; }

        public string DesignationName { get; set; }
        public int DepartmentId { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }
    }
}