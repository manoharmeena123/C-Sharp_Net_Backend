using System;

namespace AspNetIdentity.WebApi.Model.Payment
{
    public class PMO : BaseModelClass
    {
        public Guid PMOId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string FeedBack { get; set; }
        public string Rating { get; set; }

    }
}