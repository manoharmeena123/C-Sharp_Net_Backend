using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class ImportEmployee : DefaultFields
    {
        public List<ImportEmployee> EmpList { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Role { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string Bank_Name { get; set; }

        public string Whatsapp_Number { get; set; }

        public int Emergency_Number { get; set; }

        public string Email { get; set; }

        public string Moreyeahs_Mail_Id { get; set; }

        public string Technology { get; set; }

        public string Skills { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime Joining_Date { get; set; }
        public DateTime Confirmation_Date { get; set; }

        public string Primary_Contact { get; set; }

        public string Company_Name { get; set; }
        public string Profile { get; set; }
        public string Salary { get; set; }

        public string Employee_Type { get; set; }

        public string Permanent_Address { get; set; }
        public string Local_Address { get; set; }

        public string Blood_Group { get; set; }

        // public int OrgId { get; set; }

        public string Password { get; set; }
        public string Secondary_Contact { get; set; }
        public string Marital_Status { get; set; }
        public string Spouse_Name { get; set; }

        public string Aadhar_Number { get; set; }
        public string Pan_Number { get; set; }

        public string Bank_Account_Number { get; set; }
        public string IFSC { get; set; }
        public string Account_Holder_Name { get; set; }

        public string Medical_Issue { get; set; }
        //public int CompanyId { get; set; }
        //public int OrgId { get; set; }

        //---New Attributes (Roopesh Mandloi - 31 Dec 2021)
        public string DisplayName { get; set; }

        public string SecondaryJobTitle { get; set; }
        public string ReportingManager { get; set; }
        public long BiometricID { get; set; }
        public long AttendanceNumber { get; set; }
        public Nullable<System.DateTime> ProbationEndDate { get; set; }
        public bool InProbation { get; set; }
        public string TimeType { get; set; }
        public string WorkerType { get; set; }
        public string ShiftType { get; set; }
        public string WeeklyOffPolicy { get; set; }
        public int NoticePeriodMonths { get; set; }
        public string PayGroup { get; set; }
        public int CostCenter { get; set; }
        public string WorkNumber { get; set; }
        public string ResidenceNumber { get; set; }
        public string SkypeId { get; set; }
        public string Band { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }

        //---New Attributes (Roopesh Mandloi - 31 Dec 2021) Ends
    }
}