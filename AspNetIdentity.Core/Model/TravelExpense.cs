using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class TravelExpense : DefaultFields
    {
        [Key]
        public int TravelExpenseId { get; set; }

        public int? EmployeeId { get; set; }
        public string AppliedBy { get; set; }
        public string TravelFrom { get; set; }
        public string TravelTo { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int TravelerCount { get; set; }
        public TravelViaConstants TravelVia { get; set; }
        public string UploadTravelExpenseDoc { get; set; }
        public string Comment { get; set; }
        public string ExpenseStatus { get; set; }
        public string TransactoionNo { get; set; }
        public ModeofPaymentConstants ModeofPayment { get; set; }
        public string Reason { get; set; }
        public double FinalApproveAmount { get; set; }
        public int? ApprovedRejectBy { get; set; }
    }
}