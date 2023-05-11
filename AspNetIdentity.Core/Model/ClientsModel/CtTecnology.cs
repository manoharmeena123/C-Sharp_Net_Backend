using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ClientsModel
{
    public class CtTecnology : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ClientId { get; set; } = 0;
        public Guid ClientTecnologyId { get; set; }
    }
}