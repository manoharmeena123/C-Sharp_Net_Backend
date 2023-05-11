using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 14/12/2022
    /// </summary>
    public class AdHocComponent : BaseModelClass
    {
        [Key]
        public Guid ComponentId { get; set; } = Guid.NewGuid();
        public AdHocComponentTypeConstants ComponentType { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool HasTaxBenefits { get; set; } = false;
    }
}