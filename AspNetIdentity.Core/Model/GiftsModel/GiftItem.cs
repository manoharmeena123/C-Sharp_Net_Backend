using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftItem : DefaultFields
    {
        [Key]
        public int GiftItemId { get; set; }

        public string ItemName { get; set; }
        public int GBCategoryId { get; set; }
        public int GCategoryId { get; set; }
        public int GBBCategoryId { get; set; }
        public int UnitOfStock { get; set; }
        public double MRP { get; set; }
        public double PurchasePrice { get; set; }
        public string ColorPicker { get; set; }
        public string Image { get; set; }
    }
}