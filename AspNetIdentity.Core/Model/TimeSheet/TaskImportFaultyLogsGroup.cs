using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    public class TaskImportFaultyLogsGroup : BaseModelClass
    {
        [Key]
        public Guid FaultyLogsGroupId { get; set; } = Guid.NewGuid();
        public long TotalTaskImported { get; set; }
        public long SuccessFullTaskImported { get; set; }
        public long UnSuccessFullTaskImported { get; set; }
    }
}