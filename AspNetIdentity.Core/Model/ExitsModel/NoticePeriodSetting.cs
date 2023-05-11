using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.ExitsModel
{
    /// <summary>
    /// Created By Harshit Mitra On 08-08-2022
    /// </summary>
    public class NoticePeriodSetting : DefaultFields
    {
        [Key]
        public int NoticePeriodId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public NoticePeriodDurationConstants Type { get; set; }

        [NotMapped]
        public string TypeName { get; set; }

        public bool IsDefault { get; set; }
    }
}