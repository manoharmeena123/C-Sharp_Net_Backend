using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 10-02-2022
    /// </summary>
    public class CandidateDocuments
    {
        [Key]
        public int CandidateDocId { get; set; }

        public int CandidateId { get; set; }

        [NotMapped]
        public int DocTypeId { get; set; }

        /// <summary>
        /// For Degree
        /// </summary>
        public string Branch { get; set; }

        public string Degreee { get; set; }
        public DateTime? YearOfJoining { get; set; }
        public DateTime? YearOfCompleation { get; set; }
        public string PerctOrCGPA { get; set; }
        public string UniversityOrCollage { get; set; }
        public string DegreeeUpload { get; set; }

        /// <summary>
        /// For Pan Card
        /// </summary>
        public string PanNumber { get; set; }

        public string NameOnPan { get; set; }
        public DateTime? DateOfBirthDateOnPan { get; set; }
        public string FatherNameOnPan { get; set; }
        public string PanUpload { get; set; }

        /// <summary>
        /// For Aadhaar Card
        /// </summary>
        public string AadhaarCardNumber { get; set; }

        public DateTime? DateOfBirthOnAadhaar { get; set; }
        public string NameOnAadhaar { get; set; }
        public string FatherHusbandNameOnAadhaar { get; set; }
        public string GenderOnAadhaar { get; set; }
        public string AddressOnAadhaar { get; set; }
        public string FrontAadhaarUpload { get; set; }
        public string BackAadhaarUpload { get; set; }

        /// <summary>
        /// For Voter Id
        /// </summary>
        public string VoterIdNumber { get; set; }

        public DateTime? DateOfBirthOnVoterId { get; set; }
        public string NameOnVoterId { get; set; }
        public string FatherHusbandNameOnVoter { get; set; }
        public string AddressOnVoterId { get; set; }

        /// <summary>
        /// For Driving License
        /// </summary>
        public string Licensenumber { get; set; }

        public DateTime? DateOfBirthOnDriving { get; set; }
        public string NameOnDriving { get; set; }
        public string FatherHusbandNameOnDriving { get; set; }
        public DateTime? ExpireOnLicense { get; set; }
        public string DrivingLicenseUpload { get; set; }

        /// <summary>
        /// For Passport
        /// </summary>
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

        /// <summary>
        /// For Previous Experience
        /// </summary>
        public string CompanyName { get; set; }

        public string JobTitle { get; set; }
        public DateTime? JoiningDateExperience { get; set; }
        public DateTime? RelievoingDateExperience { get; set; }
        public string LocationExperience { get; set; }
        public string DescriptionExperience { get; set; }
        public string ExperienceUpload { get; set; }

        /// <summary>
        /// For Pay Slips
        /// </summary>
        public string PaySlipsUpload { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}