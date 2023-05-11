using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class StateHindmt
    {
        [Key]
        public int Stateid { get; set; }

        public string StateName { get; set; }
        public string AliasName { get; set; }
        public bool active { get; set; }
        public int CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public bool Deleted { get; set; }

        public bool IsSupplier { get; set; } //add for Supplier
        public int GSTNo { get; set; }
    }
}