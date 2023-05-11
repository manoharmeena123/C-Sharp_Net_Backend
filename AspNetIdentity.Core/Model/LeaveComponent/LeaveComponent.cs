using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model.Leave
{
    /// <summary>
    /// Created By Harshit Mitra on 19-07-2022
    /// </summary>
    public class LeaveComponent : DefaultFields
    {
        [Key]
        public int ComponentId { get; set; }

        public int LeaveGroupId { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int QuotaCount { get; set; }
        public bool IsQuotaLimit { get; set; }
        public string Quota { get; set; }
        public bool IsCompleted { get; set; }

        [NotMapped]
        public string Description { get; set; }

        [NotMapped]
        public bool IsCheck { get; set; }

        [NotMapped]
        public bool IsDefault { get; set; }

        [NotMapped]
        public bool IsDelatable { get; set; }
    }
}