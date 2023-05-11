using System;

namespace AspNetIdentity.WebApi.Model
{
    public class GraphData
    {
        public string GraphDataId { get; set; }
        public Nullable<System.DateTime> Month { get; set; }
        public int Count { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}