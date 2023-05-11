using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeDocument : DefaultFields
    {
        [Key]
        public int EmployeeDocId { get; set; }

        public int? EmployeeId { get; set; }
        public int? DocTypeId { get; set; }
        public string Branch { get; set; }
        public string Degree { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfCompleation { get; set; }
        public string PerctOrCGPA { get; set; }
        public string UniversityOrCollage { get; set; }
        public string DegreeUpload { get; set; }
        public string PanNumber { get; set; }
        public string NameOnPan { get; set; }
        public DateTime? DateOfBirthDateOnPan { get; set; }
        public string FatherNameOnPan { get; set; }
        public string PanUpload { get; set; }
        public string AadhaarUpload { get; set; }
        public string AadhaarCardNumber { get; set; }
        public DateTime? DateOfBirthOnAadhaar { get; set; }
        public string NameOnAadhaar { get; set; }
        public string FatherHusbandNameOnAadhaar { get; set; }
        public string AddressOnAadhaar { get; set; }
        public string VoterIdNumber { get; set; }
        public DateTime? DateOfBirthOnVoterId { get; set; }
        public string NameOnVoterId { get; set; }
        public string FatherHusbandNameOnVoter { get; set; }
        public string VoterUpload { get; set; }
        public string Licensenumber { get; set; }
        public DateTime? DateOfBirthOnDriving { get; set; }
        public string NameOnDriving { get; set; }
        public string FatherHusbandNameOnDriving { get; set; }
        public DateTime? ExpireOnLicense { get; set; }
        public string DrivingLicenseUpload { get; set; }

        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime? JoiningDateExperience { get; set; }
        public DateTime? RelievoingDateExperience { get; set; }
        public string LocationExperience { get; set; }
        public string DescriptionExperience { get; set; }
        public string ExperienceUpload { get; set; }

        public string PaySlipsUpload { get; set; }
        public string DocumentStatus { get; set; }
        public string Description { get; set; }
        public string location { get; set; }
        public DateTime? DateOfRelieving { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string SignatureUpload { get; set; }

        public string PassportName { get; set; }
        public DateTime? DateOfBirthOnPassport { get; set; }
        public string FullNameOnPassport { get; set; }
        public string FatherNameOnPassport { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime? ExpireOnOnPassport { get; set; }
        public string AddressOnPassport { get; set; }
        public string PassportUpload { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }

        public bool Checked { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string Address { get; set; }
    }
}