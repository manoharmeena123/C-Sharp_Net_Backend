using System.Collections.Generic;
using System.Linq;

namespace AspNetIdentity.Core.Common
{
    public class PagingResponse<T>
    {
        public PagingResponse() { }
        public PagingResponse(List<T> list, int page, int count)
        {
            TotalLength = list.Count;
            Count = count;
            ListData = list.Skip((page - 1) * count).Take(count).ToList();
        }

        public long TotalLength { get; set; }
        public long Count { get; set; }
        public List<T> ListData { get; set; }
    }
    public class PagingRequest
    {
        public int Page { get; set; }
        public int Count { get; set; }
    }
}
