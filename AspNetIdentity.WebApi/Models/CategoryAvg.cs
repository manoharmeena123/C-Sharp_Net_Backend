using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Models
{
    public class CategoryAvg
    {
        public int Avg { get; set; }
        public int CategoryId { get; set; }
        public List<CategoryAvg> CategoryDataList { get; set; }
        public List<GetcatName> CategoryNameDataList { get; set; }

        public string CategoryName { get; set; }
    }
}