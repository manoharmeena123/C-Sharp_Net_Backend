using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class SupplierPaymentDataHindmt
    {
        [Key]
        public int SupplierPaymentDataId { get; set; }

        public int PurchaseOrderId { get; set; }
        public string InVoiceNumber { get; set; }

        public double CreditInVoiceAmount
        {
            get; set;
        }

        public double DebitInvoiceAmount
        {
            get; set;
        }

        public double CreditNoteInvoiceAmount
        {
            get; set;
        }

        public double DebitNoteInvoiceAmount
        {
            get; set;
        }

        public string CreditDebitRemark
        {
            get; set;
        }

        public string Refrence
        {
            get; set;
        }

        public string IssuedOnDate
        {
            get; set;
        }

        public string PaymentMode
        {
            get; set;
        }

        public string PaymentStatusCorD
        {
            get; set;
        }

        public string VoucherType
        {
            get; set;
        }

        public string Perticular
        {
            get; set;
        }

        public double ClosingBalance
        {
            get; set;
        }

        public bool active { get; set; }
        public int CompanyId { get; set; }

        public int WarehouseId
        {
            get; set;
        }

        public string PaymentType
        {
            get; set;
        }

        public DateTime InVoiceDate
        {
            get; set;
        }

        public string IRLogoURL { get; set; }

        public int SupplierId
        {
            get; set;
        }

        public string SupplierName
        {
            get; set;
        }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}