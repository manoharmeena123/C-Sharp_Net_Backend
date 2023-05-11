using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class SubCategoryHindmt
    {
        [Key]
        public int SubCategoryId { get; set; }

        //public int CompanyId { get; set; }
        public int Categoryid { get; set; }

        //public int Warehouseid { get; set; }
        public string CategoryName { get; set; }

        public string SubcategoryName { get; set; }
        public string HindiName { get; set; }
        public string Discription { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPramotional { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }

        public int itemcount { get; set; }
        public DateTime UpdatedOn { get; internal set; }
        public int UserId { get; internal set; }
        public bool IsDeleted { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
    }
}