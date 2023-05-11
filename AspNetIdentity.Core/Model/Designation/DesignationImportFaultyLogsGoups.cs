using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class DesignationImportFaultyLogsGoups : DefaultFields
    {
        [Key]
        public Guid DesignationGroup { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}