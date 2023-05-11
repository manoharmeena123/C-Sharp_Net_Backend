using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    /// <summary>
    /// Created By Harshit Mitra on 06-06-2022
    /// </summary>
    public class SaleryStructureComponent : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public int StructureId { get; set; }
        public int ComponentId { get; set; }
        public string ComponentsName { get; set; }
        public string AnnulCalculation { get; set; }
        public bool CalculationDone { get; set; }
        //public CalculationTypeConstants CalculationType { get; set; }
    }
}