using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftBaseBaseCategory : DefaultFields
    {
        [Key]
        public int GiftBBCategoryId { get; set; }

        public int BaseCategoryId { get; set; }
        public string BaseCategoryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BBCategoryName { get; set; }
        public string Description { get; set; }
        public string Image1 { get; set; }
    }
}