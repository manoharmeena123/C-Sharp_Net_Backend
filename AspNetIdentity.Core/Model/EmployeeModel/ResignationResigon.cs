using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmployeeModel
{
    public class ResignationResigon : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}