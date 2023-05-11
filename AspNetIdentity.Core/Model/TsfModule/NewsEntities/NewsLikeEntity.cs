using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentity.Core.Model.TsfModule.NewsEntities
{
    public class NewsLikeEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid NewsId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
    }
}
