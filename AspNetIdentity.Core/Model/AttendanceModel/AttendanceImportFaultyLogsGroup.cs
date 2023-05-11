using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AttendanceModel
{
    public class AttendanceImportFaultyLogsGroup : DefaultFields
    {
        [Key]
        public Guid AttendanceGroupId { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}