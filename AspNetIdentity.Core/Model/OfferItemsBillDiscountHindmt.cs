using System.ComponentModel.DataAnnotations;

namespace GenricEcommers.Models
{
    public class OfferItemsBillDiscountHindmt
    {
        [Key]
        public int Id { get; set; }

        public int OfferId { get; set; }
        public int itemId { get; set; }
        public bool IsInclude { get; set; }
    }
}