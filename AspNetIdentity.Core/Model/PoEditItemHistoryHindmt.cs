using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class PoEditItemHistoryHindmt
    {
        [Key]
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public string ItemName { get; set; }
        public string SupplierName { get; set; }
        public double NOOfPieces { get; set; }
        public int CurrentStock { get; set; }
        public double oldprice { get; set; }
        public double newprice { get; set; }
        public string ItemNumber { get; set; }
        public string status { get; set; }
        public int? DepoId { get; set; }
        public string DepoName { get; set; }
    }
}