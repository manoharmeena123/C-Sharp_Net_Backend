using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class SupplierWarehouseHindmt
    {
        [Key]
        public int SupplierWarehouseId { get; set; }

        public int SupplierId { get; set; }
        public int? CompanyId { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //by Anushka
        public int? DepoId { get; set; }

        public string DepoName { get; set; }

        [NotMapped]
        public bool check { get; set; }

        public bool Deleted { get; set; }
        public bool Active { get; set; }
    }
}