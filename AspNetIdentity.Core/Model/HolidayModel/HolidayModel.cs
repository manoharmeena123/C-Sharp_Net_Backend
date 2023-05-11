using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    public class HolidayModel : BaseModelClass
    {
        [Key]
        public Guid HolidayId { get; set; } = Guid.NewGuid();
        public string HolidayName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsFloaterOptional { get; set; } = false;
        public string ImageUrl { get; set; } = String.Empty;
        public string TextColor { get; set; } = String.Empty;
        public DateTimeOffset HolidayDate { get; set; } = DateTimeOffset.UtcNow;
    }
}