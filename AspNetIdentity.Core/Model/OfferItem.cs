using System;
using System.ComponentModel.DataAnnotations;

namespace GenricEcommers.Models
{
    public class OfferItem
    {
        [Key]
        public int OfferId { get; set; }

        public int CompanyId { get; set; }
        public int WarehouseId { get; set; }
        public int itemId { get; set; }
        public string itemname { get; set; }
        public int MinOrderQuantity { get; set; }
        public int NoOffreeQuantity { get; set; }
        public int FreeItemId { get; set; }
        public string FreeItemName { get; set; }

        public double FreeItemMRP
        {
            get; set;
        }

        public int CustomerId
        {
            get; set;
        }

        public int OrderId
        {
            get; set;
        }

        public int WallentPoint
        {
            get; set;
        }

        public string OfferType
        {
            get; set;
        }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public int ReferOfferId
        {
            get; set;
        }
    }
}