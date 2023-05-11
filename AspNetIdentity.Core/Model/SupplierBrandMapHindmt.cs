using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class SupplierBrandMapHindmt
    {
        [Key]
        public int Id { get; set; }

        //public string Name { get; set; }
        public int SupplierId { get; set; }

        public int BrandId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}