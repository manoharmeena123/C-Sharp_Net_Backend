using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.FaultyImportLog
{
    public class EmployeeImportFaultyLog
    {
        [Key]
        public Guid LogId { get; set; } = Guid.NewGuid();
        public virtual EmployeeImportFaultyLogGroup Group { get; set; }

        /// <summary>
        /// --------------------------------------  Personal Information
        /// </summary>
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BloodGroup { get; set; }
        public string MaritalStatus { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string MobilePhone { get; set; }
        public string EmergencyNumber { get; set; }
        public string WhatsappNumber { get; set; }
        public string PersonalEmail { get; set; }
        public string PermanentAddress { get; set; }
        public string LocalAddress { get; set; }

        /// <summary>
        /// ------------------------------------------  Official Details
        /// </summary>
        public string LoginType { get; set; }

        public string EmployeeType { get; set; }
        public string DepartmentName { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string DesignationName { get; set; }
        public string BiometricId { get; set; }

        /// <summary>
        /// ------------------------------------------  Bank Info Details
        /// </summary>
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }

        /// <summary>
        /// ------------------------------------------  Credentials
        /// </summary>
        public string OfficeEmail { get; set; }
        public string Password { get; set; }
        public double Salary { get; set; }
        public string EmployeeCode { get; set; }
        public string ShiftGroup { get; set; }
        public string WeekOff { get; set; }
        public string OrganizationName { get; set; }
        public string Reason { get; set; }
        public bool OnUpdate { get; set; } = false;
    }
}