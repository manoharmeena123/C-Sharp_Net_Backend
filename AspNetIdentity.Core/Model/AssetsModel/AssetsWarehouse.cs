using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetsWarehouse : DefaultFields
    {
        [Key]
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseDescription { get; set; }
        public int? TotalItems { get; set; }
    }
}