using AspNetIdentity.WebApi.Model.TimeSheet;
using System;

namespace AspNetIdentity.Core.ViewModel.SprintViewModel
{
    /// <summary>
    /// Created By Ravi Vyas On 06-04-2023
    /// </summary>
    public class BaseSprintViewModel
    {
        public Guid SprintId { get; set; }
        public string SprintName { get; set; }
        public int ProjectId { get; set; }
        public string SprintDescription { get; set; }

    }
    public class RequestSprintViewModel
    {
        public class RequestCreateSprint : BaseSprintViewModel
        {
            public DateTimeOffset EndDate { get; set; }
            public SprintStatusConstant SprintStatus { get; set; }
            public DateTimeOffset StartDate { get; set; }
        }
        //public class RequestForGetAllSprint : 
        //{

        //}
    }
}
