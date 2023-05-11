using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    public class HolidayGroup : BaseModelClass
    {
        [Key]
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
    }
}