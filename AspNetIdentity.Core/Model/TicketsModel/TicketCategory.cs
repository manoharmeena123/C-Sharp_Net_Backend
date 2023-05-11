using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class TicketCategory : DefaultFields
    {
        [Key]
        public int TicketCategoryId { get; set; }

        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int Resolvewithin { get; set; }
        public bool IsMore { get; set; }

        [NotMapped]
        public List<int> Employees { get; set; }

        [NotMapped]
        public List<TicketCategoryPriority> Priorities { get; set; }
    }
}