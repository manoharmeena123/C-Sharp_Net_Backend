using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class CategoryFilterDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RoleId { get; set; }
        public string RoleType { get; set; }
        public int CategoryTypeId { get; set; }
        public string Category_Type { get; set; }
        public List<Questions> Questions { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}