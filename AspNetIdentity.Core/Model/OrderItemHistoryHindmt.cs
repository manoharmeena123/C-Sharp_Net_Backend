using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class OrderItemHistoryHindmt
    {
        [Key]
        public int id { get; set; }

        public int orderid { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int oldqty { get; set; }
        public int qty { get; set; }
        public string Reasoncancel { get; set; }
    }
}