using AspNetIdentity.Core.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TicketsModel.NewTicketEntities
{
    public class PrioritiesEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; } = Guid.Empty;
        public PriorityTypeEnum PriorityType { get; set; }
    }
}
