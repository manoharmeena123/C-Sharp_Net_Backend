using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemMasterSubCategory : DefaultFields
    {
        [Key]
        public int SubCategoryId { get; set; }

        public int Categoryid { get; set; }
        public string SubcategoryName { get; set; }
        public string SubCategoryDescription { get; set; }
        public string SubCategoryCode { get; set; }
    }
}