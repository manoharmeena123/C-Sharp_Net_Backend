using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SalaryDetail
    {
        [Key]
        public int SalaryId { get; set; }

        public string OldCTC { get; set; }
        public string CurrentCTC { get; set; }
        public string GrossSalary { get; set; }
        public string ESIC { get; set; }
        public string PF { get; set; }
        public string Medical { get; set; }
        public string TA { get; set; }
        public string DA { get; set; }
        public string HRA { get; set; }
        public string ProfessionalTax { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}