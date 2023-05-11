using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EndorsementType
    {
        [Key]
        public int EndorsementTypeId { get; set; }

        public string EndorsementsType { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}