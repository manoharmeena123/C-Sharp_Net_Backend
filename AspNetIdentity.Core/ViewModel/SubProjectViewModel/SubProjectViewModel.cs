using System;
using System.Collections.Generic;

namespace AspNetIdentity.Core.ViewModel.SubProjectViewModel
{
    /// <summary>
    ///  Created By Ravi Vyas on 03-04-2023
    /// </summary>
    public class BaseSubProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;

    }

    #region Response
    public class ResponseProjectViewModel
    {
        public class SubModelResponse : BaseSubProjectViewModel
        {
            public DateTimeOffset CreatedDate { get; set; }
            public string ManagerName { get; set; } = string.Empty;
            public string ProjectTypeName { get; set; } = string.Empty;
            public int ResourceCount { get; set; }
            public int BillingAmount { get; set; }
            public string ProjectHealth { get; set; } = string.Empty;
            public string CreatedByName { get; set; }
            public string UpdatedByName { get; set; }
            public string ProjectStatus { get; set; }
            public int TopProjectId { get; set; }
        }

        public class ResponseProjectSubProject : BaseSubProjectViewModel
        {
            public bool HasSubProject { get; set; } = false;
            public int TopProjectId { get; set; } = 0;
            public bool IsTaskCreate { get; set; } = false;
            public bool IsApproved { get; set; } = false;
            public string ProjectManagerName { get; set; } = string.Empty;
            public List<ResponseMainProjectEmployeeData> ProjectEmployeeData { get; set; } = new List<ResponseMainProjectEmployeeData>();
            public List<ResponseProjectSubProject> BaseProject { get; set; } = new List<ResponseProjectSubProject>();

        }

        public class ResponseMainProjectEmployeeData
        {
            public int EmployeeId { get; set; } = 0;
            public string FullName { get; set; } = string.Empty;
            public string RoleName { get; set; } = string.Empty;
        }
        public class ResponseProjectListForApproval : BaseSubProjectViewModel
        {
            public bool IsApproved { get; set; } = false;
            public int TotalPendingTask { get; set; } = 0;
            public bool IsTaskCreate { get; set; } = false;
        }
    }
    #endregion
    public class RequestProjectViewModel
    {
        public class RequestForUpdateProjectInMainProject
        {

            public int ProjectId { get; set; }
            public List<int> SubProjectId { get; set; }
        }

    }


}
