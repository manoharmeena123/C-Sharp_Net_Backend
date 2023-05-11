using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EventModel
{
    public class EventModel : BaseModelClass
    {
        [Key]
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string EventTitle { get; set; } = String.Empty;
        public string EventDescription { get; set; } = String.Empty;
        public string EventVenue { get; set; } = String.Empty;
        public TimeSpan EventTimeStart { get; set; } = TimeSpan.Zero;
        public TimeSpan EventTimeEnd { get; set; } = TimeSpan.Zero;
        public DateTimeOffset EventDate { get; set; } = DateTimeOffset.UtcNow;
    }
}