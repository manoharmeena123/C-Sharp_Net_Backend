using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class GSeatCost : DefaultFields
    {
        [Key]
        public int GSeatCostId { get; set; }

        public double GlobalAmount { get; set; }
        public bool Update { get; set; }
        public bool Create { get; set; }
    }
}