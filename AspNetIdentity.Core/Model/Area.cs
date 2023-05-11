using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }

        public string AreaName { get; set; }
        public int CityId { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        public bool active { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public bool Deleted { get; set; }
    }
}