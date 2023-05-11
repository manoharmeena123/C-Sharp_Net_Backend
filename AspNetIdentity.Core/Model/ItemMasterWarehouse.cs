using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ItemMasterWarehouse : DefaultFields
    {
        [Key]
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseDescription { get; set; }
        public int? TotalItems { get; set; }
    }
}