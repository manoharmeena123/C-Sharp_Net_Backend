using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Updated By Shriya Malvi on 11-06-2022
    /// </summary>
    public class Employee : BaseModelClass
    {
        [Key]
        public int EmployeeId { get; set; }

        /// --------------------------------------  Personal Information
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool HideDOB { get; set; }
        public BloodGroupConstants BloodGroup { get; set; } = BloodGroupConstants.Unknown;
        public string MaritalStatus { get; set; } = "Not Set";
        public string MedicalIssue { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string MobilePhone { get; set; }
        public string EmergencyNumber { get; set; }
        public string WhatsappNumber { get; set; }
        public string PersonalEmail { get; set; }
        public string LocalAddress { get; set; }
        public int? LocalCountryId { get; set; }
        public int? LocalStateId { get; set; }
        public int? LocalCityId { get; set; }
        public int? LocalPinCode { get; set; }
        public string PermanentAddress { get; set; }
        public int? PermanentCountryId { get; set; }
        public int? PermanentStateId { get; set; }
        public int? PermenentCityId { get; set; }
        public int? PermenentPinCode { get; set; }

        /// ------------------------------------------  Official Details
        [NotMapped]
        public LoginRolesConstants LoginType { get; set; }
        public EmployeeTypeConstants EmployeeTypeId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTimeOffset JoiningDate { get; set; }
        public DateTimeOffset? ConfirmationDate { get; set; }
        public DateTimeOffset? ExitDate { get; set; }
        public int DesignationId { get; set; }
        public string BiometricId { get; set; }

        /// ------------------------------------------  Bank Info Details
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }

        /// ------------------------------------------  Credentials
        public string OfficeEmail { get; set; }
        public string Password { get; set; }
        public string ProfessionalSummary { get; set; }
        public string WorkPhone { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileImageExtension { get; set; }
        public string PrimaryContact { get; set; }
        public string SecondaryContact { get; set; }
        public string SpouseName { get; set; }
        public string UploadResume { get; set; }
        public bool IsPhysicallyHandicapped { get; set; }
        public string CurrentAddress { get; set; }
        public string Summary { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Document { get; set; }
        public int RoleId { get; set; }
        public string ResidenceNumber { get; set; }
        public double GrossSalery { get; set; }
        public int? TeamTypeId { get; set; }

        public long? AttendanceNumber { get; set; }
        public DateTimeOffset? ProbationEndDate { get; set; }
        public bool? InProbation { get; set; }
        public string TimeType { get; set; }
        public string WorkerType { get; set; }
        public string ShiftType { get; set; }
        public string WeeklyOffPolicy { get; set; }
        public int? NoticePeriodMonths { get; set; }
        public int? CostCenter { get; set; }
        public string Band { get; set; }
        public string AboutMeRemark { get; set; }
        public string AboutMyJobRemark { get; set; }
        public string InterestAndHobbiesRemark { get; set; }
        public string Country { get; set; }
        public string ResidentPhone { get; set; }
        public string SkypeMail { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string EmployeeCode { get; set; }
        public int ReportingManager { get; set; } = 0;
        public int DotlineManager { get; set; } = 0;
        public int LeaveGroupId { get; set; }
        public bool IsPresident { get; set; } = false;

        //----- For Pay Roll Setup -----//
        public Guid PayGroupId { get; set; } = Guid.Empty;
        public Guid StructureId { get; set; } = Guid.Empty;
        public Guid SalaryBreakDownId { get; set; } = Guid.Empty;
        //----- Holiday, Shift, WeekOffs -----//
        public Guid? ShiftGroupId { get; set; } = Guid.Empty;
        public Guid? WeekOffId { get; set; } = Guid.Empty;

        // ----- Employee Lock ----- //
        public bool IsEmployeeIsLock { get; set; } = false;
        // ----- Holiday Group ----- //
        public Guid HolidayGroupId { get; set; } = Guid.Empty;
    }
}