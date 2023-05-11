using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetsHistory : DefaultFields
    {
        [Key]
        public int AssignHistroyId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int AssetsBaseCategoryId { get; set; } = 0;
        public int AssetsCategoryId { get; set; } = 0;
        public int WareHouseId { get; set; } = 0;
        public string ItemCode { get; set; }
        public string SerialNo { get; set; }
        public string Location { get; set; }
        public string InvoiceUrl { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? AssignToId { get; set; }
        public int? RecoverById { get; set; }
        public AssetConditionConstants AssetCondition { get; set; }
        public AssetStatusConstants AssetStatus { get; set; }
        public string AssetsDescription { get; set; }
        public string ReasonNotAvailable { get; set; }
        public bool AvailablityStatus { get; set; }
        public bool Recovered { get; set; }
        public bool Assigned { get; set; }
        public int AssignById { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? RecoverDate { get; set; }
        public string Comment { get; set; }
        public bool IsRefurbish { get; set; }
        public int RefurbishCount { get; set; }
        public string InvoiceNo { get; set; }
        public double Price { get; set; }
        public DateTime? WarentyExpDate { get; set; }
        public string Compliance { get; set; }
        public bool IsCompliance { get; set; }
        public string LicenseKey { get; set; }
        public DateTime? LicenseStartDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public int? LicApplicableCount { get; set; }
        public int? assignCount { get; set; }
        public AssetsItemType AssetsType { get; set; }
        public string UpImg1 { get; set; }
        public string UpImg2 { get; set; }
        public string UpImg3 { get; set; }
        public string UpImg4 { get; set; }
        public string UpImg5 { get; set; }
        public string UpImg6 { get; set; }
        public string UpImg7 { get; set; }
        public string UpImg8 { get; set; }
        public string UpImg9 { get; set; }
        public string UpImg10 { get; set; }
        public string AssignImage1 { get; set; }
        public string AssignImage2 { get; set; }
        public string AssignImage3 { get; set; }
        public string AssignImage4 { get; set; }
        public string AssignImage5 { get; set; }
        public string RecoverImage1 { get; set; }
        public string RecoverImage2 { get; set; }
        public string RecoverImage3 { get; set; }
        public string RecoverImage4 { get; set; }
        public string RecoverImage5 { get; set; }
        public string AssignToName { get; set; }
    }
}