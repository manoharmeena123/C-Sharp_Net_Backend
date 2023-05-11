using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class PoApprovalHindmt
    {
        [Key]
        public int Poapprovelid { get; set; }

        public string Level { get; set; }
        public int Warehouseid { get; set; }
        public string WarehouseName { get; set; }
        public int? Approval1 { get; set; }
        public int? Reviewer1 { get; set; }
        public int? Approval2 { get; set; }
        public int? Reviewer2 { get; set; }
        public int? Approval3 { get; set; }
        public int? Reviewer3 { get; set; }
        public int? Approval4 { get; set; }
        public int? Reviewer4 { get; set; }
        public int? Approval5 { get; set; }
        public int? Reviewer5 { get; set; }
        public string ApprovalName1 { get; set; }
        public string ApprovalName2 { get; set; }
        public string ApprovalName3 { get; set; }
        public string ApprovalName4 { get; set; }
        public string ApprovalName5 { get; set; }
        public string ReviewerName1 { get; set; }
        public string ReviewerName2 { get; set; }
        public string ReviewerName3 { get; set; }
        public string ReviewerName4 { get; set; }
        public string ReviewerName5 { get; set; }
        public double AmountlmtMin { get; set; }
        public double AmountlmtMax { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedTime { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public int userid { get; set; }
    }
}