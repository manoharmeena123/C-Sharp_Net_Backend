using System;
using System.Collections.Generic;

namespace AspNetIdentity.Core.Model.TsfModule
{
    public class PollsOptionsEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PollsId { get; set; } = Guid.Empty;
        public string Options { get; set; }
        public List<int> ApprovedIds { get; set; } = new List<int>();
    }
}
