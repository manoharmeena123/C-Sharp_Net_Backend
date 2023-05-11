using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetImportFaultyLogs
    {
        [Key]
        public Guid FaultyId { get; set; }
        public virtual AssetImportFaultyLogsGoups Groups { get; set; }
        public string ItemName { get; set; } = String.Empty;
        public string AssetsBaseCategoryName { get; set; } = String.Empty;
        public string AssetsCategoryName { get; set; } = String.Empty;
        public string WareHouseName { get; set; } = String.Empty;
        public string ItemCode { get; set; } = String.Empty;
        public string SerialNo { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public DateTime? PurchaseDate { get; set; }
        public string AssetCondition { get; set; } = String.Empty;
        public string AssetsDescription { get; set; } = String.Empty;
        public string AssetType { get; set; } = String.Empty;
        public string Comment { get; set; } = String.Empty;
        public string InvoiceNo { get; set; } = String.Empty;
        public double Price { get; set; } = 0;
        public DateTime? WarentyExpDate { get; set; }
        public string LicenseKey { get; set; } = String.Empty;
        public DateTime? LicenseStartDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public int? LicApplicableCount { get; set; } = 0;
        public DateTime? AssignDate { get; set; }
        public string Compliance { get; set; } = String.Empty;
        public string Officeemail { get; set; } = String.Empty;
        public string FailReason { get; set; } = String.Empty;
    }
}