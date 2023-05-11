using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CategoryType
    {
        [Key]
        public int CategoryTypeId { get; set; }

        public string Category_Type { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        // public int RoleId { get; set; }

        // public int QuestionId { get; set; }
    }
}