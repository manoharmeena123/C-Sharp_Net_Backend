using System;
using System.ComponentModel.DataAnnotations;

namespace GenricEcommers.Models
{
    public class RewardItemsHindmt
    {
        [Key]
        public int rItemId { get; set; }

        public int? CompanyId { get; set; }
        public int? WarehouseId { get; set; }
        public string rName { get; set; }
        public int rPoint { get; set; }
        public string rItem { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}