using System;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class ProjectDocument : BaseModelClass
    {
        public Guid ProjectDocumentId { get; set; } = Guid.NewGuid();
        public int ProjectId { get; set; }
        public DocumentTypeConstants DocumentTypeConstants { get; set; }
        public string DocumentTitleName { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
    }
}