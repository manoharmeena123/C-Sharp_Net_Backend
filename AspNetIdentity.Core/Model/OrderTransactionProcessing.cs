using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class OrderTransactionProcessing
    {
        [Key]
        public int OTPId { get; set; }

        public int OrderId { get; set; }
        public bool IsDispatched { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsOrderReturned { get; set; }
        public bool IsSettled { get; set; }
        public bool IsAccountSettled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}