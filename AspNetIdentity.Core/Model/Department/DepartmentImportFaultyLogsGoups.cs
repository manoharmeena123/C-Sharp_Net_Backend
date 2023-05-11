using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class DepartmentImportFaultyLogsGoups : DefaultFields
    {
        [Key]
        public Guid DepartmentGroup { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }

    }
}