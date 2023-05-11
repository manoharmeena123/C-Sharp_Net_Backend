using System;
using System.Collections.Generic;

namespace AspNetIdentity.Core.ViewModel.UserAccessViewModel
{
    public class BaseRoleViewClass
    {
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class RequestModuleAccessClass
    {
        public class CreateRoleViewClassRequest : BaseRoleViewClass
        {
            public List<RequestModuleSubModule> ModuleList { get; set; }
        }
        public class RequestModuleSubModule
        {
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }
            public bool IsAccess { get; set; }
            public List<RequestSubModuleList> SubModuleList { get; set; }
        }
        public class RequestSubModuleList
        {
            public string SubModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public bool IsAccess { get; set; }
        }
        public class UpdateRoleViewClassRequest : CreateRoleViewClassRequest
        {
            public Guid RoleId { get; set; }
        }
    }
    public class ResponseModuleAccessClass
    {
        public class GetModuleSubModuleResponse
        {
            public GetModuleSubModuleResponse()
            {
                SubModuleList = new List<GetSubModuleListResponse>();
            }
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }
            public bool IsAccess { get; set; }
            public List<GetSubModuleListResponse> SubModuleList { get; set; }
        }
        public class GetSubModuleListResponse
        {
            public string ModuleCode { get; set; }
            public string SubModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public bool IsAccess { get; set; }
        }
        public class GetRoleAndPrmissionByIdResponse : BaseRoleViewClass
        {
            public Guid RoleId { get; set; } = Guid.Empty;
            public List<GetModuleSubModuleResponse> ModuleList { get; set; }
        }
    }
}
