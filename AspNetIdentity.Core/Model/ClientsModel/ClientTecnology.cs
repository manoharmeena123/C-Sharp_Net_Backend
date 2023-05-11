using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ClientsModel
{
    public class ClientTecnology : BaseModelClass
    {
        [Key]
        public Guid ClientTecnologyId { get; set; } = Guid.NewGuid();
        public string ClientTechnoName { get; set; }
    }
}