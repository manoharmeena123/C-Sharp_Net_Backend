using System;
using System.Security.Claims;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class User
    {
        public static ClaimsIdentity Identity { get; internal set; }
        public int UserId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string HashCode { get; set; } = String.Empty;
        public int DepartmentId { get; set; } = 0;
        public int OTP { get; set; } = 0;
        public LoginRolesConstants LoginId { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int CompanyId { get; set; } = 0;
        public int OrgId { get; set; } = 0;

    }

}