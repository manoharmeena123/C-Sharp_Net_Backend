using System;

namespace AspNetIdentity.Core.Model.TicketsModel.NewTicketEntities
{
    public class CategoriesEmployeeEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
    }
}
