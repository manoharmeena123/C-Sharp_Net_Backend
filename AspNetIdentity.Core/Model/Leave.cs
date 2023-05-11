using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class LeaveConsumed : DefaultFields
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public DateTime MonthYear { get; set; }
        public int SickLeave { get; set; }
        public int PaidLeave { get; set; }
        public int UnpaidLeave { get; set; }
        public int FloaterLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int WeddingLeave { get; set; }
        public int LoseOfPayLeave { get; set; }
    }

    public class LeaveBalance : DefaultFields
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public DateTime MonthYear { get; set; }
        public int SickLeave { get; set; }
        public int PaidLeave { get; set; }
        public int UnpaidLeave { get; set; }
        public int FloaterLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int WeddingLeave { get; set; }
        public int LoseOfPayLeave { get; set; }
    }

    public class LeaveCarryOver : DefaultFields
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public DateTime MonthYear { get; set; }
        public int SickLeave { get; set; }
        public int PaidLeave { get; set; }
        public int UnpaidLeave { get; set; }
        public int FloaterLeave { get; set; }
        public int SpecialLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int BereavementLeave { get; set; }
        public int CasualLeave { get; set; }
        public int CompOffs { get; set; }
    }

    public class CurrentSalaryInformation
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public float FixedAnnualGross { get; set; }
        public float CTCExludingBonus { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string Message { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class OrganizationLocation
    {
        [Key]
        public long LocationId { get; set; }

        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string Message { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class EmployeeBonus
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string BunusType { get; set; }
        public float BonusAmount { get; set; }
        public DateTime PayoutDate { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string Message { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class EmployeeSalary
    {
        [Key]
        public long Id { get; set; }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public float MonthlyGross { get; set; }
        public float NumberOfPayDays { get; set; }
        public float Basic { get; set; }
        public float HRA { get; set; }
        public float MedicalAllowance { get; set; }
        public float ConveyanceAllowance { get; set; }
        public float SpecialAllowance { get; set; }
        public float TravelReimbursement { get; set; }
        public float Reimbursements { get; set; }
        public float Arrears { get; set; }
        public float ProvidentFund { get; set; }
        public float IncomeTax { get; set; }
        public float ProfessionalTax { get; set; }
        public float SalaryAdvance { get; set; }
        public float NetPay { get; set; }
        public DateTime SalaryDate { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class HolidayDay
    {
        [Key]
        public long HolidayId { get; set; }

        public string HolidayName { get; set; }
        public string Description { get; set; }
        public bool IsFloaterOptional { get; set; }
        public string ImageUrl { get; set; }
        public string TextColor { get; set; }
        public DateTime HolidayDate { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string MonthName { get; set; }
        [NotMapped]
        public string StartDate { get; set; }
        [NotMapped]
        public string DayName { get; set; }

        public int CompanyId { get; set; }
        public bool IsCompleted { get; set; }
    }

}