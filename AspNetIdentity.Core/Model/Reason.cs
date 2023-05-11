using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class Reason
    {
        [Key]
        public int ReasonId { get; set; }

        public string ReasonName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string Message { get; set; }

        [NotMapped]
        public bool StatusReason { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}