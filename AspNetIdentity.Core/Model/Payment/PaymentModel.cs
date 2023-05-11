using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Payment
{
    public class PaymentModel : BaseModelClass
    {
        [Key]
        public Guid ProjectPaymentId { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectId { get; set; } = 0;
        public int ChequeNo { get; set; } = 0;
        public int TransactionNo { get; set; } = 0;
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public double Amount { get; set; }
        public TransactionTypeConstants TransactionType { get; set; }
        public string TransactionName { get; set; }
        // public string ClientName { get; set; }
        public PaymentModeConstants PaymentMode { get; set; }
        public string PaymentModeName { get; set; }
    }
}