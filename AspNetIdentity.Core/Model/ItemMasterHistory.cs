using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class ItemMasterHistory
    {
        [Key]
        public int id { get; set; }

        public int CompanyId { get; set; }
        public int ItemId { get; set; }
        public int Cityid { get; set; }
        public string CityName { get; set; }
        public int Categoryid { get; set; }
        public int SubCategoryId { get; set; }
        public int SubsubCategoryid { get; set; }
        public int WarehouseId { get; set; }
        public int SupplierId { get; set; }
        public string SUPPLIERCODES { get; set; }
        public int userid { get; set; }

        //public int? Id { get; set; }
        public string CategoryName { get; set; }

        public int BaseCategoryid { get; set; }
        public string BaseCategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string SubsubcategoryName { get; set; }
        public string SupplierName { get; set; }
        public string itemcode { get; set; }
        public string SellingUnitName { get; set; }  //Selling Unit
        public string PurchaseUnitName { get; set; }  //Purchase unit name
        public double price { get; set; }
        public double VATTax { get; set; }
        public bool active { get; set; }
        public string LogoUrl { get; set; }
        public string CatLogoUrl { get; set; }
        public int MinOrderQty { get; set; }
        public int PurchaseMinOrderQty { get; set; }
        public int GruopID { get; set; }
        public string TGrpName { get; set; }

        //Code For Cess group
        public int? CessGrpID { get; set; }

        public string CessGrpName { get; set; }
        public double TotalCessPercentage { get; set; }

        public double Discount { get; set; }
        public double UnitPrice { get; set; }
        public string Number { get; set; }
        public string PurchaseSku { get; set; }
        public string SellingSku { get; set; }
        public double? PurchasePrice { get; set; }
        public double? SellingPrice { get; set; }
        public double? GeneralPrice { get; set; }
        public string title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double PramotionalDiscount { get; set; }
        public double TotalTaxPercentage { get; set; }
        public string WarehouseName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
        public bool IsDailyEssential { get; set; }
        public double DisplaySellingPrice { get; set; }
        public string StoringItemName { get; set; }
        public double SizePerUnit { get; set; }
        public double itemarea { get; set; }
        public double NoOfUnit { get; set; }
        public string HindiName { get; set; }
        public string Barcode { get; set; }
        public int? marginPoint { get; set; }
        public int? promoPoint { get; set; }
        public string UserName { get; set; }

        //By Anu
        public double DefaultBaseMargin { get; set; }

        public bool ShowMrp { get; set; }
        public bool ShowUnit { get; set; }//Min order qty
        public bool ShowUOM { get; set; }//

        public bool ShowType { get; set; }
        public string ShowTypes { get; set; } // fast slow non movinng
        public string Reason { get; set; } // MRP Issue Stock Unavailable  Price Issue Other

        [NotMapped]
        public int CurrentStock { get; set; }

        public string itemUnit { get; set; }

        //ItemMultiMRPId 20/03/2019 Harry
        public string itemname { get; set; }

        public string itemBaseName { get; set; }
        public int ItemMultiMRPId { get; set; }
        public double MRP { get; set; }
        public string UpdateBy { get; set; }
        public string UnitofQuantity { get; set; }
        public string UOM { get; set; }//Unit of masurement like GM Kg

        //Anu (02/05/2019)
        public bool IsSensitive { get; set; }//Is Sensitive Yes/No
    }
}