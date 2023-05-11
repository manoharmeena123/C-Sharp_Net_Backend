using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SupplierRateHistory
    {
        [Key]
        public int Id { get; set; }

        public int SupplierRateId { get; set; }
        public int ItemId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public double Supplierrate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}