namespace AspNetIdentity.WebApi.Model
{
    public class FilterProject
    {
        public string FilterProjectid { get; set; }

        public string ProjectId { get; set; }
        public string TechnologyId { get; set; }
        public string BillTypeId { get; set; }
        public string StatusId { get; set; }
        public string ProjectManagerId { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class FilterLead
    {
        public string FilterLeadid { get; set; }
        public string TechnologyId { get; set; }
        public string ContactId { get; set; }
        public string StatusId { get; set; }
        public string RatingId { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}