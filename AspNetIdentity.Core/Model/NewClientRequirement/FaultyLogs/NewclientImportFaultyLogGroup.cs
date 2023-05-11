using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs
{
    public class NewclientImportFaultyLogGroup : DefaultFields
    {
        [Key]
        public Guid GroupId { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}