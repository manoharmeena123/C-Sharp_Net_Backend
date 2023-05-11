using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        public int StateId { get; set; }
        public string CityName { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}