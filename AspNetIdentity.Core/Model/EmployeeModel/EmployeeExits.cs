using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.EmployeeModel
{
    public class EmployeeExits : DefaultFields
    {
        [Key]
        public int EmployeeExitId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsDiscussion { get; set; }
        public string DiscussionSummary { get; set; }
        public int ReasonId { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public ExitInitingType ExitType { get; set; }
        public ExitStatusConstants Status { get; set; }

        public DateTime? TerminateDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public bool IsReHired { get; set; }
        public string ExitFile { get; set; }
        public bool IsAcceptRetainig { get; set; }
        public bool InProgress { get; set; }
        public bool HRApproved { get; set; }
        public bool ITApproved { get; set; }
        public bool Settelment { get; set; }
        public bool Finalsettelment { get; set; }
        public string HRApprovebyName { get; set; }
        public string ITApprovebyName { get; set; }
        public int HRApprovebyId { get; set; }
        public int ITApprovebyId { get; set; }
        public int? LeaveTaken { get; set; }
        public double? Netpay { get; set; }
        public string ITComments { get; set; }
        public string HRComments { get; set; }
        public string TerminationDocument { get; set; }
    }
}