using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.FaultyImportLog
{
    public class EmployeeImportFaultyLogGroup : DefaultFields
    {
        [Key]
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}