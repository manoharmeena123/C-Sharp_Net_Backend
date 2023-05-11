using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemHistory : DefaultFields
    {
        [Key]
        public int HistoryId { get; set; }

        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int BaseCategoryId { get; set; }
        public string BaseCategoryName { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Condition { get; set; }
        public string Itemcode { get; set; }
        public bool IsAsset { get; set; }
        public int Units { get; set; }
        public int UniqueCode { get; set; }
        public string SerialNumber { get; set; }
        public string InvoiceUrl { get; set; }
    }
}