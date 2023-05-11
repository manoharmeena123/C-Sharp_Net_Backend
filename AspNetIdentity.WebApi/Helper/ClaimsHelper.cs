using AspNetIdentity.Core.Common;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace AspNetIdentity.WebApi.Helper
{
    public static class ClaimsHelper
    {
        public static string EncriptKey = ConfigurationManager.AppSettings["EncryptKey"];
        #region Emossy Claims
        public static ClaimsHelperModel GetClaimsResult(ClaimsIdentity identity)
        {
            Decryption _dec = new Decryption(EncriptKey);
            if (identity?.Claims != null)
            {
                ClaimsHelperModel res = new ClaimsHelperModel();
                foreach (var item in identity.Claims)
                {
                    if (item.Type == "userId")
                        res.userId = int.Parse(item.Value);
                    if (item.Type == "companyId")
                        res.companyId = int.Parse(_dec.DecryptString(item.Value));
                    if (item.Type == "orgId")
                        res.orgId = int.Parse(item.Value);
                    if (item.Type == "employeeId")
                        res.employeeId = int.Parse(_dec.DecryptString(item.Value));
                    if (item.Type == "roleType")
                        res.roleType = item.Value.ToString();
                    if (item.Type == "displayName")
                        res.displayName = item.Value.ToString();
                    if (item.Type == "profileImage")
                        res.profileImage = item.Value.ToString();
                    if (item.Type == "companyName")
                        res.companyName = item.Value;
                    if (item.Type == "defaultShiftGroup")
                        res.DefaultShiftGroupId = ((!string.IsNullOrEmpty(item.Value)) && (!string.IsNullOrWhiteSpace(item.Value))) ? Guid.Parse(item.Value) : Guid.Empty;
                    if (item.Type == "EmpShiftGroup")
                        res.EmpShiftGroup = ((!string.IsNullOrEmpty(item.Value)) && (!string.IsNullOrWhiteSpace(item.Value))) ? Guid.Parse(item.Value) : Guid.Empty;
                    if (item.Type == "defaultWeekOff")
                        res.DefaultWeekOff = ((!string.IsNullOrEmpty(item.Value)) && (!string.IsNullOrWhiteSpace(item.Value))) ? Guid.Parse(item.Value) : Guid.Empty;
                    if (item.Type == "TimeZone")
                        res.TimeZone = item.Value.ToString();
                    if (item.Type == "IsAdminInCompany")
                        res.IsAdminInCompany = bool.Parse(item.Value);
                    if (item.Type == "EmployeeWeekOff")
                        res.EmployeeWeekOff = ((!string.IsNullOrEmpty(item.Value)) && (!string.IsNullOrWhiteSpace(item.Value))) ? Guid.Parse(item.Value) : Guid.Empty;
                    if (item.Type == "IsSmtpProvided")
                        res.IsSmtpProvided = bool.Parse(item.Value);
                }
                return res;

            }
            else
            {
                return null;
            }
        }
        public class ClaimsHelperModel
        {
            [JsonIgnore]
            public int userId { get; set; } = 0;
            public int orgId { get; set; } = 0;
            public int employeeId { get; set; } = 0;
            public string roleType { get; set; } = String.Empty;
            public string displayName { get; set; } = String.Empty;
            public int companyId { get; set; } = 0;
            public string profileImage { get; set; } = String.Empty;
            public string companyName { get; set; } = String.Empty;
            public string companyDomain { get; set; } = String.Empty;
            [JsonIgnore]
            public Guid? EmpShiftGroup { get; set; } = Guid.Empty;
            [JsonIgnore]
            public Guid? DefaultShiftGroupId { get; set; }
            [JsonIgnore]
            public Guid? DefaultWeekOff { get; set; }
            public string CompanyLogo { get; set; } = String.Empty;
            public string CompanyWebSiteURL { get; set; } = String.Empty;
            public string TimeZone { get; set; } = "India Standard Time";
            public Guid EmployeeWeekOff { get; set; } = Guid.Empty;
            public bool IsAdminInCompany { get; set; } = false;
            public bool IsSmtpProvided { get; set; } = false;
        }
        public static object GetTokenData(JwtSecurityToken tokenS)
        {
            Decryption _dec = new Decryption(EncriptKey);
            ClaimsHelperModel res = new ClaimsHelperModel();
            foreach (var item in tokenS.Claims)
            {
                if (item.Type == "userId")
                    res.userId = int.Parse(item.Value);
                if (item.Type == "companyId")
                    res.companyId = int.Parse(_dec.DecryptString(item.Value));
                if (item.Type == "orgId")
                    res.orgId = int.Parse(item.Value);
                if (item.Type == "employeeId")
                    res.employeeId = int.Parse(_dec.DecryptString(item.Value));
                if (item.Type == "roleType")
                    res.roleType = item.Value.ToString();
                if (item.Type == "displayName")
                    res.displayName = item.Value.ToString();
                if (item.Type == "profileImage")
                    res.profileImage = item.Value.ToString();
                if (item.Type == "companyName")
                    res.companyName = item.Value;
                if (item.Type == "companyDomain")
                    res.companyDomain = item.Value.ToString();
                if (item.Type == "TimeZone")
                    res.TimeZone = item.Value.ToString();
                if (item.Type == "CompanyLogo")
                    res.CompanyLogo = item.Value.ToString();
                if (item.Type == "CompanyWebSiteURL")
                    res.CompanyWebSiteURL = item.Value.ToString();
                if (item.Type == "IsAdminInCompany")
                    res.IsAdminInCompany = bool.Parse(item.Value);
            }
            return res;
        }
        #endregion

        #region Client Claims
        public static ClientTokenResponse RetrieveClientToken(ClaimsIdentity identity)
        {
            if (identity?.Claims != null)
            {
                ClientTokenResponse res = new ClientTokenResponse();
                foreach (var item in identity.Claims)
                {
                    if (item.Type == "ClientId")
                        res.ClientId = int.Parse(item.Value);
                    if (item.Type == "Credentials")
                        res.Credentials = item.Value;
                }
                return res;
            }
            else
            {
                return null;
            }
        }
        public class ClientTokenResponse
        {
            public int ClientId { get; set; } = 0;
            public string Credentials { get; set; } = String.Empty;
        }
        #endregion

    }
    public static class TsfClaims
    {
        public static string EncriptKey = ConfigurationManager.AppSettings["EncryptKey"];
        public static string GetTsfRefressTokenData(ClaimsIdentity identity)
        {
            if (identity?.Claims != null)
                return identity.Claims.Where(x => x.Type == "credentials").Select(x => x.Value.ToString()).FirstOrDefault();
            else
                return null;
        }
        public static GetTsfAccessTokenClass GetTsfAccessTokenData(ClaimsIdentity identity)
        {
            Decryption _dec = new Decryption(EncriptKey);
            GetTsfAccessTokenClass obj = new GetTsfAccessTokenClass();
            foreach (var item in identity.Claims)
            {
                if (item.Type == "employeeId")
                    obj.employeeId = int.Parse(_dec.DecryptString(item.Value));
                if (item.Type == "companyId")
                    obj.companyId = int.Parse(_dec.DecryptString(item.Value));
                if (item.Type == "credentials")
                    obj.credentials = _dec.DecryptString(item.Value).ToString();
                if (item.Type == "adminInCompany")
                    obj.adminInCompany = bool.Parse(item.Value);
            }
            return obj;
        }
        public class GetTsfAccessTokenClass
        {
            public int employeeId { get; set; } = 0;
            public int companyId { get; set; } = 0;
            public string credentials { get; set; } = string.Empty;
            public bool adminInCompany { get; set; } = false;
        }
    }
}