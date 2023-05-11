using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class RadiusHindmt
    {
        [Key]
        public int Radiusid { get; set; }

        public string RadiusName { get; set; }
        public string Discription { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }
}