using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class ObjectiveModel : BaseModelClass
    {
        [Key]
        public Guid ObjectiveId { get; set; } = Guid.NewGuid();
        public string ObjectiveName { get; set; }
        public ObjectiveTypeConstants ObjectiveTypeEnumId { get; set; }
        public string Objectivetype { get; set; }
        public string Owner { get; set; }
        public WhoCanSeeConstants WhoCanSeeEnumId { get; set; }
        public string WhoCanSeeName { get; set; }
        public string IncludeInReview { get; set; }
        public ProgressConstants ProgressEnumId { get; set; }
        public string ProgressName { get; set; }
        public TagsConstants TagsEnumId { get; set; }
        public string TagsName { get; set; }
        public int StartValue { get; set; }
        public int TargetValue { get; set; }

    }
}