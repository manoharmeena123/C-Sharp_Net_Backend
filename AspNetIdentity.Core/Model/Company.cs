using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public Guid CompanyGuidId { get; set; } = Guid.NewGuid();
        public string RegisterCompanyName { get; set; } = String.Empty;
        public string CompanyGst { get; set; } = String.Empty;
        public string CIN { get; set; } = String.Empty;
        public string RegisterAddress { get; set; } = String.Empty;
        public string RegisterEmail { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;
        public DateTimeOffset IncorporationDate { get; set; } = DateTimeOffset.Now;
        public string IncorporationCertificate { get; set; } = String.Empty;
        public int CreatedBy { get; set; } = 0;
        public string UpdateJson { get; set; } = String.Empty;
        public string CompanyWebSiteURL { get; set; } = String.Empty;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string AppAdminLogo { get; set; } = String.Empty;
        public string NavigationLogo { get; set; } = String.Empty;
        public Guid DefaultShiftId { get; set; } = Guid.Empty;
        public Guid DefaultWeekOff { get; set; } = Guid.Empty;
        public Guid DefaultRole { get; set; } = Guid.Empty;
        public string CompanyDomain { get; set; } = String.Empty;
        public int CountryId { get; set; } = 0;
        public string CompanyDefaultTimeZone { get; set; } = "India Standard Time";
        public bool IsCompanyIsLock { get; set; } = false;
        public bool IsSmtpProvided { get; set; } = false;
        public bool IsTsfCompany { get; set; } = false;
        public string ImgKitApiKey { get;set; } = String.Empty;
    }
}