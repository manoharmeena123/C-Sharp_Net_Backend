using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemMaster : DefaultFields
    {
        [Key]
        public int ItemId { get; set; }  // primary key

        public string ItemName { get; set; }        // Asset Id
        public int BaseCategoryId { get; set; }
        public string BaseCategoryName { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }        // category name
        public int SubCategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Condition { get; set; }
        public string Itemcode { get; set; } //asset Item id
        public bool IsAsset { get; set; }
        public int Units { get; set; }
        public int UniqueCode { get; set; }
        public string SerialNumber { get; set; }
        public string InvoiceUrl { get; set; }
        public DateTime? PurchasedOn { get; set; }
        //------------------------------------------

        public string AssetDescription { get; set; }
        public string AssetLocation { get; set; }
        public string AssetStatus { get; set; }
        public string AssetType { get; set; }
        public string ReasonifNotAvailable { get; set; }
        public int EmployeeIdifAssigned { get; set; }                   //employee number is Employee Id // assigned to
        public string EmployeeNameifAssigned { get; set; }                   //employee number is Employee name
        public string EmployeeDepartmentifAssigned { get; set; }
        public DateTime? DateofAssetAssignment { get; set; }
        //public int AssetId { get; set; }
        //public int AssetItemId { get; set; }                              //not available in database table sheet
        //public string AssetName { get; set; }              item name
        //public DateTime? PurchasedOn { get; set; }
        //public string AssetCondition { get; set; }

        //public int AssetWarehouseId { get; set; }                           //not available in excel sheet
        //public int AssetUnits { get; set; }                                 //not available in excel sheet
        //public string AssetWarehouseName { get; set; }                      //not available in excel sheet
        //public int AssetUniqueCode { get; set; }                            //not available in excel sheet
        //public string AssetSerialNumber { get; set; }                       //not available in excel sheet
        //public int BaseCategoryId { get; set; }                             //not available in excel sheet
        // public int CategoryId { get; set; }                                 //not available in excel sheet
        // public string BaseCategoryName { get; set; }                        //not available in excel sheet
        //public int AssetSubCategoryId { get; set; }                         //not available in excel sheet
        // public string AssetSubCategoryName { get; set; }                    //not available in excel sheet
        // public int AssetCategoryId { get; set; }                            //not available in excel sheet
        //public string AssetCategory { get; set; }// asset category
        // public string Itemcode { get; set; }

        // public string ItemcmompanyName { get; set; }
        //public string AssetItemId { get; set; }
    }
}