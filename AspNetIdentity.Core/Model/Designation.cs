using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Designation : BaseModelClass
    {
        [Key]
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int DepartmentId { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }


    }
}