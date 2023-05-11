using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement
{
    public class NewClientHistory : BaseModelClass
    {
        [Key]
        public Guid ClientHistoryId { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; } = Guid.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Worktype { get; set; } = string.Empty;
        public string OfficialEmail { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
    }
}