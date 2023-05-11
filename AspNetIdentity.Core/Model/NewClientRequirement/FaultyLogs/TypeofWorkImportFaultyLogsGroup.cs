using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs
{
    public class TypeofWorkImportFaultyLogsGroup : BaseModelClass
    {
        [Key]
        public Guid GroupId { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}