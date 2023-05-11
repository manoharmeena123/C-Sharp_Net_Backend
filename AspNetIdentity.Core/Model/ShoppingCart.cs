using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class IDetail
    {
        public int ItemId { get; set; }
        public int qty { get; set; }
        public int CompanyId { get; set; }
        public int WarehouseId { get; set; }
        public int FreeItemId { get; set; }
        public int FreeItemqty { get; set; }

        public bool IsOffer
        {
            get; set;
        }

        public bool IsFlashDeal
        {
            get; set;
        }

        public int OfferId
        {
            get; set;
        }

        public double OfferWalletPoint
        {
            get; set;
        }

        public int OfferCategory
        {
            get; set;
        }

        [NotMapped]
        public double UnitPrice { get; set; }

        [NotMapped]
        public double NewUnitPrice { get; set; }

        [NotMapped]
        public bool IsSuccess { get; set; }

        [NotMapped]
        public string Message { get; set; }
    }

    public class ShoppingCart
    {
        public string Customerphonenum { get; set; }
        public List<IDetail> itemDetails { get; set; }
        public int? SalesPersonId { get; set; }
        public string CustomerName { get; set; }
        public string Trupay { get; set; }
        public string ShopName { get; set; }
        public string CustomerType { get; set; }
        public string ShippingAddress { get; set; }
        public string LScode { get; set; }
        public double deliveryCharge { get; set; }
        public double WalletAmount { get; set; }
        public double walletPointUsed { get; set; }
        public int DialEarnigPoint { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string paymentThrough { get; set; }
        public string TrupayTransactionId { get; set; }
        public string paymentMode { get; set; }
        public string DeliveredSlot { get; set; }
        public double Savingamount { get; set; }
        public double OnlineServiceTax { get; set; }

        [NotMapped]
        public int OrderId { get; set; }

        [NotMapped]
        public string status { get; set; }

        public string BillDiscountOfferId { get; set; } // Offer Id of Bill Discount (change to string from int)
        public double? BillDiscountAmount { get; set; } // Bill Discount Amount
    }

    public class ShoppingCartForInactiveOrder
    {
        public string Customerphonenum { get; set; }
        public List<IDetail> itemDetails { get; set; }
        public int? SalesPersonId { get; set; }
        public string CustomerName { get; set; }
        public string Trupay { get; set; }
        public string ShopName { get; set; }
        public string CustomerType { get; set; }
        public string ShippingAddress { get; set; }
        public string LScode { get; set; }
        public double deliveryCharge { get; set; }
        public double WalletAmount { get; set; }
        public double walletPointUsed { get; set; }
        public int DialEarnigPoint { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string paymentThrough { get; set; }
        public string TrupayTransactionId { get; set; }
        public string paymentMode { get; set; }
        public double Savingamount { get; set; }
        public double OnlineServiceTax { get; set; }

        [NotMapped]
        public int OrderId { get; set; }

        [NotMapped]
        public string status { get; set; }

        public int? BillDiscountOfferId { get; set; } // Offer Id of Bill Discount
        public double? BillDiscountAmount { get; set; } // Bill Discount Amount

        [NotMapped]
        public bool IsNewOrder { get; set; }
    }

    public class MyOrders
    {
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime Deliverydate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? ReadytoDispatchedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public double GrossAmount { get; set; }
        public double RemainingAmount { get; set; }
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string status { get; set; }
        public double? walletPointUsed { get; set; }
        public double? RewardPoint { get; set; }
        public bool EnablePayNowButton { get; set; }
        public string PayNowOption { get; set; }
        public string PaymentMode { get; set; }
        public double? DeliveryCharge { get; set; }
        public double DisCountAmount { get; set; }
        public List<OrderPayments> OrderPayments { get; set; }
        public List<MyOrderDetail> itemDetails { get; set; }
    }

    public class MyOrderDetail
    {
        public int ItemId { get; set; }
        public int qty { get; set; }
        public int CompanyId { get; set; }
        public int WarehouseId { get; set; }

        [NotMapped]
        public double UnitPrice { get; set; }

        [NotMapped]
        public string ItemName { get; set; }

        [NotMapped]
        public string LogoUrl { get; set; }

        public int OrderId { get; set; }
        public string ItemImage { get; set; }
    }

    public class OrderPayments
    {
        public int OrderId { get; set; }
        public string PaymentFrom { get; set; }
        public double Amount { get; set; }
        public string TransactionNumber { get; set; }
        public string DeliveryCharges { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class CompanyItemDetail
    {
        public List<IDetail> item_Detail { get; set; }
    }
}