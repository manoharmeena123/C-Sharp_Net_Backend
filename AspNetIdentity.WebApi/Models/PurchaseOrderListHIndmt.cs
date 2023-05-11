using System;

namespace AngularJSAuthentication.Model
{
    public class PurchaseOrderListHindmt
    {
        public DateTime OrderDate { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int CompanyId { get; set; }
        public int? Cityid { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int OrderId { get; set; }
        public int OrderDetailsId { get; set; }
        public string PurchaseSku { get; set; }

        //public string SellingSku { get; set; }
        public string PurchaseUnitName { get; set; }   //new added

        public int PurchaseUnitId { get; set; }        //new added
        public double Conversionfactor { get; set; }   // new added

        public double NetPurchasePrice { get; set; } // new Added By raj
        public string StoringItemName { get; set; }
        public int? ItemId { get; set; }
        public string SKUCode { get; set; }
        public string ItemName { get; set; }
        public int PurchaseMinOrderQty { get; set; }
        public string Unit { get; set; }
        public string Discription { get; set; }
        public int qty { get; set; }
        public int requiredqty { get; set; }
        public int CurrentInventory { get; set; }
        public int NetPurchaseQty { get; set; }  //TotalPurchaseQty=CurrentInventory-qty;
        public double Price { get; set; }
        public double NetAmmount { get; set; }
        public double TaxPercentage { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmountIncTax { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Deleted { get; set; }

        //multimrp
        public int ItemMultiMRPId { get; set; }

        public int DepoId { get; set; }
        public string DepoName { get; set; }
    }
}