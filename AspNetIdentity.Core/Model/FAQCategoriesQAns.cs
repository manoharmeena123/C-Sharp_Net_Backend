using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class FAQCategoriesQAns
    {
        [Key]
        public int FaqCategoriesQAnsId { get; set; }
        public int FAQCategoriesId { get; set; }
        public string FaqQuesions { get; set; }
        //public int EmployeeId { get; set; }
        public string FaqAns { get; set; }
        public string FaqImage { get; set; }
        public string FaqTags { get; set; }
        public bool IsPublish { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public bool HrAdded { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByname { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }

        public AboutUsStatusConstants Status { get; set; }

    }
}