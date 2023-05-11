using System;
using System.Collections.Generic;

namespace AspNetIdentity.Core.ViewModel.UserAccessViewModel
{
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public class BaseCompanyAccessViewModel
    {
        public string ModuleName { get; set; } = String.Empty;
        public string ModuleCode { get; set; } = String.Empty;
        public string ModulePathURL { get; set; } = String.Empty;
    }
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public class RequestCompanyAccess
    {
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        public class SetComanyRoleRequest : BaseCompanyAccessViewModel
        {
            public bool CheckBox { get; set; } = false;
        }
        public class UpdateCompanyAccessRequest
        {
            public int CompanyId { get; set; } = 0;
            public List<SetComanyRoleRequest> ModuleAccess { get; set; } = new List<SetComanyRoleRequest>();
        }
    }
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public class ResponseCompanyAccess
    {
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        public class GetComanyRoleResponse : BaseCompanyAccessViewModel
        {
            public bool CheckBox { get; set; } = false;
        }
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        public class GetCompanyDetailsResponse
        {
            public string RegisterCompanyName { get; set; }
            public string CompanyGst { get; set; }
            public string RegisterAddress { get; set; }
            public string RegisterEmail { get; set; }
        }
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        public class GetCompanyAccessResponse
        {
            public GetCompanyDetailsResponse CompanyDetails { get; set; }
            public List<GetComanyRoleResponse> ModuleAccess { get; set; }
        }
    }
}
