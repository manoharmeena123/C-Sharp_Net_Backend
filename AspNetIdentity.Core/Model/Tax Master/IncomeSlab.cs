using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Tax_Master
{
    public class IncomeSlab : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Details { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public int Texpercent { get; set; }
    }
}