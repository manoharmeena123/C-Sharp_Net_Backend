using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        //public string Category_Type { get; set; }

        public int CategoryTypeId { get; set; }

        public string CategoryName { get; set; }

        public int UsertypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}