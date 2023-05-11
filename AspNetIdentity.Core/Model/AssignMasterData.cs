using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class AssignMasterData : DefaultFields
    {
        [Key]
        public long AssignId { get; set; }

        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int WarehouseId { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedTo { get; set; }
        public string Condition { get; set; }
        public string AssetsKey { get; set; }
        public int? AssetRecoveredById { get; set; }
        public string AssetRecoveredBy { get; set; }
        public string Comment { get; set; }
        public bool AvailablityStatus { get; set; }
        public string ReasonfornotAvailable { get; set; }
        public string Note { get; set; }
        public string IconUrl { get; set; }       // change by suraj bundel
        public int IconId { get; set; }       // change by suraj bundel
    }
}