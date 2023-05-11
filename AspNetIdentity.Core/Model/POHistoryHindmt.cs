using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class POHistoryHindmt
    {
        [Key]
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public string WarehouseName { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string EditBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}