using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class GroupUser : DefaultFields
    {
        [Key]
        public int UserInGroupId { get; set; }

        public int GroupId { get; set; }
        public int? EmployeeId { get; set; }
        public string UserName { get; set; }
    }
}