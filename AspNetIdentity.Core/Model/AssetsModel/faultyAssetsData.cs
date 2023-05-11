using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class FaultyAssetsData : DefaultFields
    {
        [Key]
        public int FaultyAssetId { get; set; }

        public int WarehouseId { get; set; }
        public int ItemId { get; set; }
        public string Assetskey { get; set; }
        public string Condition { get; set; }
        public string Comments { get; set; }
        public int? AssetRecoveredById { get; set; }
        public string AssetRecoveredBy { get; set; }
    }
}