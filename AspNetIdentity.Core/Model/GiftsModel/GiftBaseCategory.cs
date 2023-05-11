using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftBaseCategory : DefaultFields
    {
        [Key]
        public int GiftBCategoryId { get; set; }

        public String BaseCategoryName { get; set; }
        public string BaseCategoryCode { get; set; }
        public string Description { get; set; }
        public string Image1 { get; set; }
    }
}