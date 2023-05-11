using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemMasterBaseCategory : DefaultFields
    {
        [Key]
        public int BaseCategoryId { get; set; }

        public string BaseCategoryName { get; set; }
        public string BaseCategoryDescription { get; set; }
        public string BaseCategoryCode { get; set; }
    }
}