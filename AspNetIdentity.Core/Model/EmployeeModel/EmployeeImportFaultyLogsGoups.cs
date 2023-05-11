using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmployeeModel
{
    public class EmployeeImportFaultyLogsGoups : DefaultFields
    {
        [Key]
        public Guid EmployeeGroupId { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}