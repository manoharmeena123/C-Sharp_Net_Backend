using AspNetIdentity.Core.Enum;

namespace AspNetIdentity.Core.Common
{
    public class RequestShortBy
    {
        public ShortByEnumClass? ShortBy { get; set; }
    }
    public class RequestShortByWithPagging : PagingRequest
    { 
        public ShortByEnumClass? ShortBy { get; set; }
    }
    public class ResponseShortByEnumClass
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}
