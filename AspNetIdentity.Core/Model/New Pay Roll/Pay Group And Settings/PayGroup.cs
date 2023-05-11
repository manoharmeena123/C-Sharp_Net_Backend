using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 08/12/2022
    /// </summary>
    public class PayGroup : BaseModelClass
    {
        [Key]
        public Guid PayGroupId { get; set; } = Guid.NewGuid();
        public string PayGroupName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}