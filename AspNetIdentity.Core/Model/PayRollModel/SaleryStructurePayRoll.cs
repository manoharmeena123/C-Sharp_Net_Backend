using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    /// <summary>
    /// Created By Harshit Mitra On 06-06-2022
    /// </summary>
    public class SaleryStructurePayRoll : DefaultFields
    {
        [Key]
        public int StructureId { get; set; }

        public int PayGroupId { get; set; }
        public string StructureName { get; set; }
        public string Description { get; set; }
    }
}