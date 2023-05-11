using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class ShortItemAssignmentHindmt
    {
        [Key]
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string itemNumber { get; set; }
        public string itemname { get; set; }
        public int Orderqty { get; set; }
        public int DamageStockQty { get; set; }
        public int NotinStockQty { get; set; }
        public string DamageComment { get; set; }
        public string NotInStockComment { get; set; }
        public int DeliveryIssuanceId { get; set; }
        public int DboyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
        public string CreatedBy { get; set; }

        public Int32 OrderId { get; set; }
    }
}