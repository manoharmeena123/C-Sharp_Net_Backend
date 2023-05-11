using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra On 19/12/2022
    /// </summary>
    public class SalaryStructure : BaseModelClass
    {
        [Key]
        public Guid StructureId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public string StructureName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}