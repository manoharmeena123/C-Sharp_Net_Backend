namespace AspNetIdentity.WebApi.Model
{
    public class CandidateWebRole
    {
        public int CandidateWebRoleId { get; set; }
        public string CandidateWebRoleType { get; set; }

        public int TechnologyID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}