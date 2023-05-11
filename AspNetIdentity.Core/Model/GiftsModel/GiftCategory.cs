using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftCategory : DefaultFields
    {
        [Key]
        public int GiftCategoryId { get; set; }

        public int GBCategoryId { get; set; }
        public string GBCategoryName { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Image1 { get; set; }
    }
}