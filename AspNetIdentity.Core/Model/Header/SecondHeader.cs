using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Header
{
    public class SecondHeader : DefaultFields
    {
        [Key]
        public int SecondHeaderId { get; set; }
        public int NavigationId { get; set; }
        public string HeaderName { get; set; }

    }
}