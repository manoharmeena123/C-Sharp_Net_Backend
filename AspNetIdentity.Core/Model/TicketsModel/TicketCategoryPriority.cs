using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class TicketCategoryPriority : DefaultFields
    {
        [Key]
        public int TicPriorityId { get; set; }

        public int TicketCategoryId { get; set; }
        public int PriorityType { get; set; }
        public string PriorityName { get; set; }
        public string PriorityDescription { get; set; }

        [NotMapped]
        public bool IsRequired { get; set; }
    }
}