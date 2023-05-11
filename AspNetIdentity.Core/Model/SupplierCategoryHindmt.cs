using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class SupplierCategoryHindmt
    {
        [Key]
        public int SupplierCaegoryId { get; set; }

        public int CompanyId { get; set; }
        public string CategoryName { get; set; }

        [NotMapped]
        public String Exception { get; set; }
    }
}