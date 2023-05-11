using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class RelationOfEmp : DefaultFields
    {
        [Key]
        public int RelationId { get; set; }

        public int EmployeeId { get; set; }
        public string Relation { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Profession { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}