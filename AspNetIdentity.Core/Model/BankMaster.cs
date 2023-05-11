using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class BankMaster : DefaultFields
    {
        [Key]
        public int BankId { get; set; }

        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AccountNumber { get; set; }
    }
}