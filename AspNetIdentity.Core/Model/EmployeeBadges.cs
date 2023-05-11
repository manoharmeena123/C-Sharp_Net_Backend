using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeBadges
    {
        [Key]
        public int BadgeId { get; set; }

        public string BadgeName { get; set; }
        public string ImageUrl { get; set; }
        public int BadgeType { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}