using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs
{
    public class NewclientImportFaultyLog : BaseModelClass
    {
        [Key]
        public Guid FaultyId { get; set; } = Guid.NewGuid();
        public virtual NewclientImportFaultyLogGroup Groups { get; set; }
        public Guid ClientId { get; set; } = Guid.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OfficialEmail { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public string FailReason { get; set; } = String.Empty;
    }
}