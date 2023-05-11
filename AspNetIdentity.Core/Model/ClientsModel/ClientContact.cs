using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ClientsModel
{
    public class ClientContact : DefaultFields
    {
        [Key]
        public Guid ClientContactId { get; set; } = Guid.NewGuid();
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string ClientCLinkedinProfile { get; set; }
        public string ContectNumber { get; set; }
        public int EmployeeId { get; set; }
        public string About { get; set; }
        public string EmailAddress { get; set; }
        public string position { get; set; }
        public string ContactUrl { get; set; }
    }
}