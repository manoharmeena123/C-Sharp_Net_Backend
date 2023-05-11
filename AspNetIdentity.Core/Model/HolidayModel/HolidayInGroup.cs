using System;

namespace AspNetIdentity.WebApi.Model.HolidayModel
{
    public class HolidayInGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; } = Guid.Empty;
        public Guid HolidayId { get; set; } = Guid.Empty;
    }
}