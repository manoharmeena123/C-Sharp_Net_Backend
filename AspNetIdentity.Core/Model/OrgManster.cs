using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class OrgMaster
    {
        [Key]
        public int OrgId { get; set; }
        public Guid OrgGuidId { get; set; } = Guid.NewGuid();
        public int CompanyId { get; set; } = 0;
        public Guid CompnayGuidId { get; set; } = Guid.Empty;
        public string OrgName { get; set; } = String.Empty;
        public string OrgAddress { get; set; } = String.Empty;
        public string TimeZoneId { get; set; } = "India Standard Time";
        public int CountryId { get; set; } = 0;
        public int CreatedBy { get; set; } = 0;
        public int? UpdatedBy { get; set; } = 0;
        public int? DeletedBy { get; set; } = 0;
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool IsOrgIsLock { get; set; } = false;
    }
}