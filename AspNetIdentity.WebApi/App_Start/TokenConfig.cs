using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi
{
    public class TokenConfig
    {
        private static string Secret = "ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM";
        public static string EncriptKey = ConfigurationManager.AppSettings["EncryptKey"];
        public static object GetToken(string userName)
        {
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Subject = new ClaimsIdentity(claims: new[]
                {
                        new Claim(type: ClaimTypes.Name, value: userName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return new { token = handler.WriteToken(token) };
        }
        public static object GetToken(Model.User user, LoginRolesConstants loginRole, bool isExternal = false)
        {
            Encryption _enc = new Encryption(EncriptKey);
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            using (var _db = new ApplicationDbContext())
            {
                var employee = _db.Employee.FirstOrDefault(x => x.OfficeEmail == user.UserName);
                if (employee == null && LoginRolesConstants.SuperAdmin == loginRole)
                {
                    descriptor.Subject = new ClaimsIdentity(claims: new[]
                    {
                    new Claim(type: ClaimTypes.Name, value: "Super Admin"),
                    new Claim("FirstName", "Super"),
                    new Claim("LastName", "Admin"),
                    new Claim("displayName", "SuperAdmin"),
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("employeeId", _enc.EncryptString("0")),
                    new Claim("roleType", "SuperAdmin"),
                    new Claim("companyId", _enc.EncryptString("0")),
                    new Claim("orgId", "0"),
                    new Claim("profileImage", ""),
                    });
                    JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
                    //return new { token = handler.WriteToken(token) };
                    var returnData = new TokenResponseClass
                    {
                        Message = "Login Success",
                        Status = true,
                        Acces_Token = handler.WriteToken(token),
                        RoleType = loginRole.ToString(),
                        DisplayName = "Supper Admin",
                        ProfileImage = "uploadimage\\assets\\logo - moreyeahs.png",
                        IsImageSaved = true,
                    };
                    string externalReturnData = handler.WriteToken(token);
                    if (isExternal)
                        return externalReturnData;
                    return returnData;
                }
                else if (employee != null)
                {
                    if (employee.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                    {
                        Company checkDefault = _db.Company.FirstOrDefault(x => x.CompanyId == employee.CompanyId);
                        if (checkDefault.IsCompanyIsLock)
                            return new TokenResponseClass
                            {
                                Message = "Please Contact To Emossy Group. " +
                                    "Company Is Locked " +
                                    "You Not Have Access To Login",
                                Status = false,
                            };
                        if (employee.OrgId != 0)
                            if (_db.OrgMaster.Where(x => x.OrgId == employee.OrgId).Any(x => x.IsOrgIsLock))
                                return new TokenResponseClass
                                {
                                    Message = "Please Contact To HR or Admin. " +
                                        "Organization Is Locked " +
                                        "You Not Have Access To Login",
                                    Status = false,
                                };
                        if (employee.IsEmployeeIsLock)
                            return new TokenResponseClass
                            {
                                Message = "Please Contact To HR. " +
                                    "You Don't Have Access To Login",
                                Status = false,
                            };
                        var checkRole = (from er in _db.EmployeeInRoles
                                         join rl in _db.RoleInUserAccessPermissions on er.RoleId equals rl.RoleId
                                         where er.EmployeeId == employee.EmployeeId
                                         select rl.HeadRoleInCompany
                                         )
                                         .ToList()
                                         .Any(x => x);
                        descriptor.Subject = new ClaimsIdentity(claims: new[]
                        {
                            new Claim(type: ClaimTypes.Name, value: user.UserName),
                            new Claim("FirstName", employee.FirstName),
                            new Claim("LastName", employee.LastName),
                            new Claim("displayName", employee.DisplayName),
                            new Claim("userId", user.UserId.ToString()),
                            new Claim("employeeId", _enc.EncryptString(employee.EmployeeId.ToString())),
                            new Claim("roleType", "Users"),
                            new Claim("companyId",_enc.EncryptString( employee.CompanyId.ToString())),
                            new Claim("orgId", employee.OrgId.ToString()),
                            new Claim("profileImage", employee.ProfileImageUrl??""),
                            new Claim("defaultShiftGroup" , checkDefault.DefaultShiftId.ToString()),
                            new Claim("defaultWeekOff" , checkDefault.DefaultShiftId.ToString()),
                            new Claim("defaultRoleId" , checkDefault.DefaultRole.ToString()),
                            new Claim("companyDomain", checkDefault.CompanyDomain??""),
                            new Claim("CompanyLogo" , checkDefault.AppAdminLogo??""),
                            new Claim("CompanyWebSiteURL" , checkDefault.CompanyWebSiteURL??""),
                            new Claim("IsAdminInCompany", checkRole.ToString()),
                            new Claim("TimeZone", checkDefault.CompanyDefaultTimeZone??"India Standard Time"),
                            new Claim("EmployeeWeekOff", employee.WeekOffId.ToString()),
                            new Claim("EmpShiftGroup", employee.ShiftGroupId.ToString()),
                            new Claim("IsSmtpProvided", checkDefault.IsSmtpProvided.ToString()),

                        });
                        JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
                        //return new { token = handler.WriteToken(token) };
                        var empImages = _db.EmpImages.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        var returnData = new TokenResponseClass
                        {
                            Message = "Login Success",
                            Status = true,
                            Acces_Token = handler.WriteToken(token),
                            RoleType = "Users",
                            DisplayName = employee.DisplayName,
                            ProfileImage = employee.ProfileImageUrl,
                            IsImageSaved = !(empImages == null),
                        };
                        string externalReturnData = handler.WriteToken(token);
                        if (isExternal)
                            return externalReturnData;
                        return returnData;
                    }
                }
            }
            if (isExternal)
                return "Access_Denied";
            return new TokenResponseClass
            {
                Message = "Employee Not Found",
                Status = false,
            };
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)handler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch
            {

                return null;
            }
        }
        public static string ValidateToke(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {

                return null;
            }
            Claim usernameClaim = identity.FindFirst(type: ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }

        #region Helper Class
        public class TokenResponseClass
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string Acces_Token { get; set; }
            public string RoleType { get; set; }
            public string DisplayName { get; set; }
            public string ProfileImage { get; set; }
            public bool IsImageSaved { get; set; } = false;
        }

        #endregion

        #region CLient Logins Token Generate
        public static object ISAGetToken(Client client, string credentials)
        {
            DateTime now = DateTime.UtcNow;
            var returnData = new ClientLoginResponse
            {
                RefreshToken = RefreshTokenClient(now, credentials),
                AccessToken = AccessToken(now, client, credentials),
            };
            return returnData;
        }
        public static string AccessToken(DateTime now, Client client, string credentials)
        {
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Expires = now.AddSeconds(30),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            descriptor.Subject = new ClaimsIdentity(claims: new[]
            {
                new Claim(type: ClaimTypes.Name, value: "Client"),
                new Claim("ClientId", client.ClientId.ToString()),
                new Claim("Credentials", credentials),
            });
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return new { token = handler.WriteToken(token) };
            return handler.WriteToken(token);
        }
        public static string RefreshTokenClient(DateTime now, string credentials)
        {
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Expires = now.AddHours(1),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            descriptor.Subject = new ClaimsIdentity(claims: new[]
            {
                new Claim("Credentials", credentials),
            });
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public class ClientLoginResponse
        {
            public string Message { get; set; } = "Login Successfully";
            public bool Status { get; set; } = true;
            public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
            public string RefreshToken { get; set; } = String.Empty;
            public string AccessToken { get; set; } = String.Empty;

        }
        #endregion

        #region TsfUser Logins Token Generate
        public static object GetTokenTsf(Employee employee)
        {
            DateTime now = DateTime.UtcNow;
            var returnData = new ClientLoginResponse
            {
                RefreshToken = RefreshTokenTsf(now, employee.OfficeEmail),
                AccessToken = AccessTokenTsf(now, employee, employee.OfficeEmail),
            };
            return returnData;
        }
        public static string AccessTokenTsf(DateTime now, Employee employee, string officeEmail)
        {
            Encryption _enc = new Encryption(EncriptKey);
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Expires = now.AddDays(1),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            descriptor.Subject = new ClaimsIdentity(claims: new[]
            {
                new Claim(type: ClaimTypes.Name, value: "User"),
                new Claim("employeeId", _enc.EncryptString(employee.EmployeeId.ToString())),
                new Claim("companyId",_enc.EncryptString(employee.CompanyId.ToString())),
                new Claim("firstName", employee.FirstName.ToString()),
                new Claim("middleName", employee.MiddleName??string.Empty),
                new Claim("lastName", employee.LastName??string.Empty),
                new Claim("displayName", employee.DisplayName.ToString()),
                new Claim("profileUrl", employee.ProfileImageUrl??string.Empty),
            });
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return new { token = handler.WriteToken(token) };
            return handler.WriteToken(token);
        }
        public static string RefreshTokenTsf(DateTime now, string officeEmail)
        {
            Encryption _enc = new Encryption(EncriptKey);
            byte[] key = Encoding.UTF8.GetBytes(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:44330",
                Audience = "https://localhost:44330",
                Expires = now.AddDays(30),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            descriptor.Subject = new ClaimsIdentity(claims: new[]
            {
                new Claim("credentials", _enc.EncryptString(officeEmail)),
            });
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        #endregion
    }
}