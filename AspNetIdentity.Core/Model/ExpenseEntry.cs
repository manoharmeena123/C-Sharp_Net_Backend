using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class ExpenseEntry : DefaultFields
    {
        [Key]
        public int ExpenseId { get; set; }

        public int CategoryId { get; set; }
        public int EmployeeId { get; set; }
        public string AppliedBy { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ExpenseTitle { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string ISOCurrencyCode { get; set; }
        public double ExpenseAmount { get; set; }
        public string MerchantName { get; set; }
        public string BillNumber { get; set; }
        public string Comment { get; set; }
        public string ImageUrl { get; set; }
        public bool IsApprove { get; set; }
        public string ExpenseStatus { get; set; }
        public string Reason { get; set; }
        public string IconImageUrl { get; set; }
        public ExpenseTypeConstants ExpenseCategoryType { get; set; }
        public ModeofPaymentConstants ModeofPayment { get; set; }
        public double FinalApproveAmount { get; set; }
        public int? ApprovedRejectBy { get; set; }
        public string TravelFrom { get; set; } //shriya
        public string TravelTo { get; set; }//shriya
        public DateTime? DepartureDate { get; set; } //shriya
        public DateTime? ReturnDate { get; set; }//shriya
        public int TravelerCount { get; set; }//shriya
        public TravelViaConstants TravelVia { get; set; }//shriya
    }
}