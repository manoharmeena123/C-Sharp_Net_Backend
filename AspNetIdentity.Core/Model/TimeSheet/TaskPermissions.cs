using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskPermissions : BaseModelClass
    {
        [Key]
        public Guid TaskPermissionId { get; set; } = Guid.NewGuid();
        public int ProjectId { get; set; }
        public int AssigneEmployeeId { get; set; }
        public bool IsCreateTask { get; set; } = true;
        public bool IsDeleteTask { get; set; } = false;
        public bool IsApprovedTask { get; set; } = false;
        public bool IsExeclUploade { get; set; } = false;
        public bool IsReEvaluetTask { get; set; } = false;
        public bool IsUpdate { get; set; } = false;
        public bool IsOtherTaskCreate { get; set; } = false;
        public bool IsBoardVisible { get; set; } = false;
        public bool ViewAlProjectTask { get; set; } = false;

    }
}