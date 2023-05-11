using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftItemCategory : DefaultFields
    {
        [Key]
        public int GItemCategoryId { get; set; }

        public string GItemCategoryName { get; set; }
        public string CategoryCods { get; set; }
    }
}