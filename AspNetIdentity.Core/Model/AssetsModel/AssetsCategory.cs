using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetsCategory : DefaultFields
    {
        [Key]
        public int AssetsCategoryId { get; set; }

        public int AssetsBCategoryId { get; set; }
        public string AssetsCategoryName { get; set; }

        public string ColorCode { get; set; }
        public int AssetsCategoryIconId { get; set; }
        public bool IsAssetsIcon { get; set; }
        public string AssetIconImgUrl { get; set; }
        public string Description { get; set; }
    }
}