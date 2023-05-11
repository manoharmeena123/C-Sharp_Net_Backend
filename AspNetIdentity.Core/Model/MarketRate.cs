using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class MarketRate
    {
        [Key]
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string MarketName { get; set; }
        public string MobileNumber { get; set; }
        public double Marketrate { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}