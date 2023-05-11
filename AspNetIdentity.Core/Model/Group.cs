using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Group : DefaultFields
    {
        [Key]
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        public string Description { get; set; }
        public string GroupImageURL { get; set; }
        public string Messagedesc { get; set; }
        public int Memberid { get; set; }
    }
}