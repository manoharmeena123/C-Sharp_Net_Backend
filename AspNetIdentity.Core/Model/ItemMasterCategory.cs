using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemMasterCategory : DefaultFields
    {
        [Key]
        public int Categoryid { get; set; }

        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryCode { get; set; }
        public int BaseCategoryId { get; set; }
    }
}