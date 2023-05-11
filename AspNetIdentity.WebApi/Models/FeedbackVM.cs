using System;

namespace AspNetIdentity.WebApi.Models
{
    public class FeedbackVM
    {
        public int FeedbackId { get; set; }
        public int ReceiverEmployeeId { get; set; }
        public string EmpName { get; set; }
        public int AverageScore { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int RatedByEmpId { get; set; }
        public string RatedBy { get; set; }
        public int RoleId { get; set; }
        public string RoleType { get; set; }
        public int CategoryTypeId { get; set; }
        public string CategoryType { get; set; }
    }
}