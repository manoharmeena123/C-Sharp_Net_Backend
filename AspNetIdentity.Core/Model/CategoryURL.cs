using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class CategoryURL
    {
        [Key]
        public int CategoryURLId { get; set; }

        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public int TrackingCategoryId { get; set; }
        public string TrackingCategoryName { get; set; }
        public bool IsProductive { get; set; }
        public bool IsNonProductive { get; set; }
        public bool IsBlock { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string Message { get; set; }
    }
}