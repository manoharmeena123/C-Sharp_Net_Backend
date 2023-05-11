using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class MarketRateHistory
    {
        [Key]
        public int Id { get; set; }

        public int MarketRateId { get; set; }
        public double MarketRate { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}