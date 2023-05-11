namespace AspNetIdentity.WebApi.Model
{
    public class BillType
    {
        public int BillTypeId { get; set; }
        public string Bill_Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}