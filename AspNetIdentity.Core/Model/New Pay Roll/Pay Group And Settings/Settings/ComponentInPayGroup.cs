using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 16/12/2022
    /// </summary>
    public class ComponentInPayGroup : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public Guid ComponentId { get; set; } = Guid.Empty;
        public ComponentTypeInPGConstants ComponentType { get; set; } = ComponentTypeInPGConstants.RecurringComponent;
    }
}