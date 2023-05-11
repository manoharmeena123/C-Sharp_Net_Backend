using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.CloudSofwareLicenes
{
    public class CloudSoftwareLicense
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public DateTimeOffset? USerCreatedDate { get; set; }
        public bool IsLicensed { get; set; }
        public string LicensePlanWithEnabledService { get; set; }
        public string FriendlyNameOfLicensePlanAndEnabledService { get; set; }
        public DateTime? CreateOn { get; set; }
        public int? CreatedBY { get; set; }
    }
}