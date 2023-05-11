using AspNetIdentity.WebApi.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 18-02-2022
    /// </summary>
    public class DefaultFields
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; set; } = null;
        public DateTime? DeletedOn { get; set; } = null;

        [NotMapped]
        public string CreatedByName { get; set; }

        [NotMapped]
        public string UpdatedByName { get; set; }

        [NotMapped]
        public string DeletedByName { get; set; }
    }

    public class BaseModelClass
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int CompanyId { get; set; } = 0;
        public int OrgId { get; set; } = 0;
        public bool IsDefaultCreated { get; set; } = false;
        public int CreatedBy { get; set; } = 0;
        public int? UpdatedBy { get; set; } = null;
        public int? DeletedBy { get; set; } = null;
        public DateTimeOffset CreatedOn { get; set; } = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
        public DateTimeOffset? UpdatedOn { get; set; } = null;
        public DateTimeOffset? DeletedOn { get; set; } = null;
    }
}