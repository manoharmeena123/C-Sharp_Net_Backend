using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string Category_Type { get; set; }
        public int CategoryTypeId { get; set; }
        public string CategoryName { get; set; }

        public int UsertypeId { get; set; }

        public List<Questions> Questions { get; set; }

        [NotMapped]
        public List<int> listUsertypeId { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}