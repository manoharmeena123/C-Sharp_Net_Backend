using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ExtraMasterModel
{
    /// <summary>
    /// Created By Harshit Mitra on 09/12/2022
    /// </summary>
    public class BusinessNature : BaseModelClass
    {
        [Key]
        public Guid BusinessNatureId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = String.Empty;
    }
}