using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetsBaseCategory : DefaultFields
    {
        [Key]
        public int AssetsBCategoryId { get; set; }

        public string AssetsBCategoryName { get; set; }
        public string Description { get; set; }
        public AssetsItemType AssetsType { get; set; }
    }
}