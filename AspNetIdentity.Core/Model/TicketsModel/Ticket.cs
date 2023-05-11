using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 22-03-2022
    /// </summary>
    public class Ticket : DefaultFields
    {
        [Key]
        public int TicketId { get; set; }

        public int TicketCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public int TicketStatus { get; set; }
        public int PriorityType { get; set; }
        public string PriorityName { get; set; }
        public int TicketPriorityId { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? TicketClosedOn { get; set; }
    }
}