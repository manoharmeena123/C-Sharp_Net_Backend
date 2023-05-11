using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using static AspNetIdentity.WebApi.Model.EnumClass;
using Task = System.Threading.Tasks.Task;

namespace AngularJSAuthentication.API.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //var allowedOrigin = "*";
            //int UserId = 0;
            //int RoleId = 0;
            try
            {
                logger.Debug("OAuth Start - ");
                var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
                if (allowedOrigin == null) allowedOrigin = "*";
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                ApplicationUser applicationUser = await userManager.FindAsync(context.UserName, context.Password);
                logger.Debug("ApplicationUser Found - ");
                if (applicationUser == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect........");
                    return;
                }
                if (!applicationUser.EmailConfirmed)
                {
                    context.SetError("invalid_grant", "User did not confirm email.");
                    // return;
                }
                var user = db.User.Where(x => x.UserName == applicationUser.UserName).First();
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect........");
                    return;
                }
                else
                {
                    user = db.User.Where(x => x.UserName == applicationUser.UserName).First();
                    var hashCode = user.HashCode;
                    var password = context.Password.ToString();
                    var encodingPassword = DataHelper.EncodePassword(password, hashCode);
                    if (user.Password != encodingPassword)
                    {
                        context.SetError("invalid_grant", "The user password is incorrect........");
                        return;
                    }
                    logger.Debug("User Found - ");

                    var employee = db.Employee.Where(x => x.OfficeEmail == context.UserName).FirstOrDefault();
                    if (employee == null && user.CompanyId == 0 && user.OrgId == 0 && user.LoginId == 0)
                    {
                        logger.Debug("Super Admin Part");
                        ClaimsIdentity identity = await applicationUser.GenerateUserIdentityAsync(userManager, "JWT");
                        identity.AddClaims(ExtendedClaimsProvider.GetClaims(applicationUser));
                        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                        identity.AddClaim(new Claim("FirstName", applicationUser.FirstName));
                        identity.AddClaim(new Claim("LastName", applicationUser.LastName));
                        identity.AddClaim(new Claim("displayname", "SuperAdmin"));
                        identity.AddClaim(new Claim("email", applicationUser.UserName.ToString()));
                        identity.AddClaim(new Claim("level", applicationUser.Level.ToString()));
                        identity.AddClaim(new Claim("userid", user.UserId.ToString()));
                        identity.AddClaim(new Claim("employeeid", "0"));
                        identity.AddClaim(new Claim("roletype", "SuperAdmin"));
                        identity.AddClaim(new Claim("companyid", "0"));
                        identity.AddClaim(new Claim("orgid", "0"));
                        //identity.AddClaim(new Claim("SecurityStamp", Guid.NewGuid().ToString()));

                        var props = new AuthenticationProperties(new Dictionary<string, string> { });
                        props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                                    { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                                    { "userName", context.UserName },
                                    { "Level", applicationUser.Level.ToString() },
                                    { "FirstName", applicationUser.FirstName},
                                    { "LastName", applicationUser.LastName},
                                    { "PhoneNumber", applicationUser.PhoneNumber },
                                    { "email" , applicationUser.Email },
                                    { "userid", user.UserId.ToString() },
                                    { "employeeid", "0"},
                                    { "roletype" , "SuperAdmin" },
                                    { "companyid" , "0" },
                                    { "orgid" , "0" },
                                    { "displayname" , "SuperAdmin" },
                                    { "ProfileImage" , "" },
                                    { "designation" , "Super Admin"},
                                    { "SecurityStamp" , applicationUser.SecurityStamp}
                        });
                        var ticket = new AuthenticationTicket(identity, props);
                        context.Validated(ticket);
                    }
                    else if (employee != null)
                    {
                        if (employee.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                        {

                            logger.Debug("Employee Part - ");
                            var designation = db.Designation.Where(x => x.DesignationId == employee.DesignationId).FirstOrDefault();
                            //var module = db.UIPermissionModules.Where(x => x.DesignationId == employee.DesignationId && x.View).FirstOrDefault();
                            var department = Enum.GetName(typeof(LoginRolesConstants), user.LoginId);
                            ClaimsIdentity identity = await applicationUser.GenerateUserIdentityAsync(userManager, "JWT");
                            var props = new AuthenticationProperties(new Dictionary<string, string>());
                            //if (module == null)
                            //{
                            identity.AddClaims(ExtendedClaimsProvider.GetClaims(applicationUser));
                            identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                            identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                            identity.AddClaim(new Claim("FirstName", applicationUser.FirstName));
                            identity.AddClaim(new Claim("LastName", applicationUser.LastName));
                            identity.AddClaim(new Claim("email", applicationUser.UserName.ToString()));
                            identity.AddClaim(new Claim("Level", applicationUser.Level.ToString()));
                            identity.AddClaim(new Claim("userid", user.UserId.ToString()));
                            identity.AddClaim(new Claim("employeeid", employee.EmployeeId.ToString()));
                            identity.AddClaim(new Claim("displayname", employee.DisplayName));
                            identity.AddClaim(new Claim("roletype", department));
                            identity.AddClaim(new Claim("companyid", employee.CompanyId.ToString()));
                            identity.AddClaim(new Claim("orgid", employee.OrgId.ToString()));
                            identity.AddClaim(new Claim("DesignationId", employee.DesignationId.ToString()));
                            //identity.AddClaim(new Claim("ShiftGroupId", employee.ShiftGroupId.HasValue ? employee.ShiftGroupId.ToString() : Guid.Empty.ToString()));

                            props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                    { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                                    { "userName", context.UserName },
                                    { "Level", applicationUser.Level.ToString() },
                                    { "FirstName", applicationUser.FirstName },
                                    { "LastName", applicationUser.LastName },
                                    { "PhoneNumber", applicationUser.PhoneNumber },
                                    { "email" ,applicationUser.UserName },
                                    { "userid", user.UserId.ToString() },
                                    { "employeeid", employee.EmployeeId.ToString() },
                                    { "roletype" , department },
                                    { "companyid" , employee.CompanyId.ToString() },
                                    { "orgid" , employee.OrgId.ToString() },
                                    { "displayname" , employee.DisplayName },
                                    { "designation" , designation.ToString() },
                                    { "ProfileImage" , employee.ProfileImageUrl == null ? "" :  employee.ProfileImageUrl},
                                    { "SecurityStamp" , applicationUser.SecurityStamp},
                                    {"DesignationName", designation.DesignationName.ToString() },
                                    //{"ShiftGroupId", employee.ShiftGroupId.HasValue ? employee.ShiftGroupId.ToString() : Guid.Empty.ToString()},
                            });
                            //}
                            //else
                            //{
                            //    identity.AddClaims(ExtendedClaimsProvider.GetClaims(applicationUser));
                            //    identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                            //    identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                            //    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                            //    identity.AddClaim(new Claim("FirstName", applicationUser.FirstName));
                            //    identity.AddClaim(new Claim("LastName", applicationUser.LastName));
                            //    identity.AddClaim(new Claim("email", applicationUser.UserName.ToString()));
                            //    identity.AddClaim(new Claim("Level", applicationUser.Level.ToString()));
                            //    identity.AddClaim(new Claim("userid", user.UserId.ToString()));
                            //    identity.AddClaim(new Claim("employeeid", employee.EmployeeId.ToString()));
                            //    identity.AddClaim(new Claim("displayname", employee.DisplayName));
                            //    identity.AddClaim(new Claim("roletype", department));
                            //    identity.AddClaim(new Claim("companyid", employee.CompanyId.ToString()));
                            //    identity.AddClaim(new Claim("orgid", employee.OrgId.ToString()));
                            //    identity.AddClaim(new Claim("DesignationId", employee.DesignationId.ToString()));
                            //    identity.AddClaim(new Claim("DesignationName", designation.DesignationName.ToString()));
                            //    //identity.AddClaim(new Claim("ModuleName", module.ModuleName.ToString()));
                            //    //identity.AddClaim(new Claim("view", module.View.ToString()));
                            //    //identity.AddClaim(new Claim("ShiftGroupId", employee.ShiftGroupId.HasValue ? employee.ShiftGroupId.ToString() : Guid.Empty.ToString()));

                            //    props = new AuthenticationProperties(new Dictionary<string, string>
                            //{
                            //        { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                            //        { "userName", context.UserName },
                            //        { "Level", applicationUser.Level.ToString() },
                            //        { "FirstName", applicationUser.FirstName },
                            //        { "LastName", applicationUser.LastName },
                            //        { "PhoneNumber", applicationUser.PhoneNumber },
                            //        { "email" ,applicationUser.UserName },
                            //        { "userid", user.UserId.ToString() },
                            //        { "employeeid", employee.EmployeeId.ToString() },
                            //        { "roletype" , department },
                            //        { "companyid" , employee.CompanyId.ToString() },
                            //        { "orgid" , employee.OrgId.ToString() },
                            //        { "displayname" , employee.DisplayName },
                            //        { "designation" , designation.ToString() },
                            //        { "ProfileImage" , employee.ProfileImageUrl == null ? "" :  employee.ProfileImageUrl},
                            //        { "SecurityStamp" , applicationUser.SecurityStamp},

                            //        {"DesignationName", designation.DesignationName.ToString() },
                            //        {"ModuleName",module.ModuleName.ToString()},
                            //        {"view", module.View.ToString() },
                            //        //{"ShiftGroupId", employee.ShiftGroupId.HasValue ? employee.ShiftGroupId.ToString() : Guid.Empty.ToString() },
                            //});
                            //}
                            var ticket = new AuthenticationTicket(identity, props);
                            db.Dispose();
                            context.Validated(ticket);
                        }
                    }
                    else
                    {
                    }
                }

                //Admin login

                #region Old Codes

                //if (s == null)
                //{
                //    if (string.IsNullOrEmpty(user.UserName))
                //    {
                //        User u = db.User.Where(x => x.UserName == context.UserName).FirstOrDefault();
                //        UserId = u.UserId;
                //        RoleId = u.DepartmentId;

                //        if (!u.IsActive)
                //        {
                //            context.SetError("invalid_grant", "Please check your registered email address to validate email address.");
                //            return;
                //        }
                //        if (UserId == 0)
                //        {
                //            context.SetError("invalid_grant", "The user name or password is incorrect.");
                //            return;
                //        }

                //        ClaimsIdentity identity = await user.GenerateUserIdentityAsync(userManager, "JWT");
                //        identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));

                //        //UserAccessPermission uap = con.getRoleDetail(p.Permissions); // Get Role Access Detail
                //        var rolesIds = user.Roles.Select(x => x.RoleId).ToList();

                //        //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                //        //identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                //        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                //        //identity.AddClaim(new Claim(ClaimTypes.Role, user));
                //        identity.AddClaim(new Claim("firsttime", "true"));
                //        // identity.AddClaim(new Claim("compid", p.CompanyId.ToString()));
                //        identity.AddClaim(new Claim("email", user.Email));
                //        identity.AddClaim(new Claim("FirstName", ""));
                //        identity.AddClaim(new Claim("LastName", ""));
                //        // identity.AddClaim(new Claim("ImageUrl", p.ImageUrl));
                //        identity.AddClaim(new Claim("Email", user.Email.ToString()));
                //        identity.AddClaim(new Claim("Level", user.Level.ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        // identity.AddClaim(new Claim("DisplayName", p.DisplayName));
                //        // identity.AddClaim(new Claim("username", (s.UserName).ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        identity.AddClaim(new Claim("Roleids", string.Join(",", rolesIds)));
                //        //identity.AddClaim(new Claim("pagePermissions", JsonConvert.SerializeObject(pagePermissions)));
                //        User_Id = UserId;
                //        //ac_Type = s.RoleName;
                //        var props = new AuthenticationProperties(new Dictionary<string, string> { });
                //        //if (s.RoleId ==  )
                //        //{
                //        props = new AuthenticationProperties(new Dictionary<string, string>
                //        {
                //            { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                //            { "userName", context.UserName },
                //            //{ "compid", p.CompanyId.ToString() },
                //            { "Level", user.Level.ToString() },
                //            { "FirstName", ""},
                //            { "LastName", ""},
                //            //{ "UserProfileImage", s.UserProfileImage},
                //            { "compid", user.PhoneNumber },
                //            //{ "role" ,s.RoleName },
                //            { "email" ,user.Email },
                //            { "userid", UserId.ToString() },
                //            //{ "LScode",  s.LScode },
                //            //{ "pagePermissions", JsonConvert.SerializeObject(pagePermissions)}
                //        });
                //        var ticket = new AuthenticationTicket(identity, props);
                //        context.Validated(ticket);
                //    }
                //    else  // Authorize APK
                //    {
                //        User u = db.User.Where(x => x.UserName == context.UserName).FirstOrDefault();
                //        RoleId = u.DepartmentId;
                //        UserId = u.UserId;
                //        ClaimsIdentity identity = await user.GenerateUserIdentityAsync(userManager, "JWT");
                //        identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));

                //        //UserAccessPermission uap = con.getRoleDetail(p.Permissions); // Get Role Access Detail
                //        var Role = db.Role.Where(x => x.RoleId == RoleId).FirstOrDefault();

                //        //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                //        //identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                //        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                //        //identity.AddClaim(new Claim(ClaimTypes.Role, user));
                //        identity.AddClaim(new Claim("firsttime", "true"));
                //        identity.AddClaim(new Claim("compid", "0"));
                //        identity.AddClaim(new Claim("orgid", "0"));
                //        identity.AddClaim(new Claim("email", user.Email));
                //        identity.AddClaim(new Claim("FirstName", ""));
                //        identity.AddClaim(new Claim("LastName", ""));
                //        // identity.AddClaim(new Claim("ImageUrl", p.ImageUrl));
                //        identity.AddClaim(new Claim("Email", user.Email.ToString()));
                //        identity.AddClaim(new Claim("Level", user.Level.ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        // identity.AddClaim(new Claim("DisplayName", p.DisplayName));
                //        // identity.AddClaim(new Claim("username", (s.UserName).ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        identity.AddClaim(new Claim("Roleids", string.Join(",", RoleId)));

                //        var props = new AuthenticationProperties(new Dictionary<string, string> { });

                //        props = new AuthenticationProperties(new Dictionary<string, string>
                //        {
                //            { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                //            { "userName", context.UserName },
                //            { "Level", user.Level.ToString() },
                //            { "FirstName", ""},
                //            { "LastName", ""},
                //            { "compid", "0"},
                //            { "Roleids" ,RoleId.ToString()},
                //            { "email" ,user.Email },
                //            { "userid", UserId.ToString() },
                //            //{"rolename",Role.RoleType},
                //            { "orgid", "0"},
                //        });
                //        var ticket = new AuthenticationTicket(identity, props);
                //        context.Validated(ticket);
                //    }
                //}
                //else
                //{
                //    if (string.IsNullOrEmpty(user.UserName))
                //    {
                //        UserId = s.EmployeeId;
                //        RoleId = s.RoleId;

                //        if (!s.IsActive)
                //        {
                //            context.SetError("invalid_grant", "Please check your registered email address to validate email address.");
                //            return;
                //        }
                //        if (UserId == 0)
                //        {
                //            context.SetError("invalid_grant", "The user name or password is incorrect.");
                //            return;
                //        }

                //        ClaimsIdentity identity = await user.GenerateUserIdentityAsync(userManager, "JWT");
                //        identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));

                //        //UserAccessPermission uap = con.getRoleDetail(p.Permissions); // Get Role Access Detail
                //        var rolesIds = user.Roles.Select(x => x.RoleId).ToList();

                //        //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                //        //identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                //        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                //        //identity.AddClaim(new Claim(ClaimTypes.Role, user));
                //        identity.AddClaim(new Claim("firsttime", "true"));
                //        // identity.AddClaim(new Claim("compid", p.CompanyId.ToString()));
                //        identity.AddClaim(new Claim("email", user.Email));
                //        identity.AddClaim(new Claim("FirstName", s.FirstName));
                //        identity.AddClaim(new Claim("LastName", s.LastName));
                //        // identity.AddClaim(new Claim("ImageUrl", p.ImageUrl));
                //        identity.AddClaim(new Claim("Email", user.Email.ToString()));
                //        identity.AddClaim(new Claim("Level", user.Level.ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        // identity.AddClaim(new Claim("DisplayName", p.DisplayName));
                //        // identity.AddClaim(new Claim("username", (s.UserName).ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        identity.AddClaim(new Claim("Roleids", string.Join(",", rolesIds)));
                //        //identity.AddClaim(new Claim("pagePermissions", JsonConvert.SerializeObject(pagePermissions)));
                //        User_Id = UserId;
                //        //ac_Type = s.RoleName;
                //        var props = new AuthenticationProperties(new Dictionary<string, string> { });
                //        //if (s.RoleId ==  )
                //        //{
                //        props = new AuthenticationProperties(new Dictionary<string, string>
                //        {
                //            { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                //            { "userName", context.UserName },
                //            //{ "compid", p.CompanyId.ToString() },
                //            { "Level", user.Level.ToString() },
                //            { "FirstName", s.FirstName},
                //            { "LastName", s.LastName},
                //            //{ "UserProfileImage", s.UserProfileImage},
                //            { "compid", user.PhoneNumber },
                //            //{ "role" ,s.RoleName },
                //            { "email" ,user.Email },
                //            { "userid", UserId.ToString() },
                //            //{ "LScode",  s.LScode },
                //            //{ "pagePermissions", JsonConvert.SerializeObject(pagePermissions)}
                //        });
                //        var ticket = new AuthenticationTicket(identity, props);
                //        context.Validated(ticket);
                //    }
                //    else  // Authorize APK
                //    {
                //        RoleId = s.RoleId;
                //        UserId = s.EmployeeId;
                //        ClaimsIdentity identity = await user.GenerateUserIdentityAsync(userManager, "JWT");
                //        identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));

                //        //UserAccessPermission uap = con.getRoleDetail(p.Permissions); // Get Role Access Detail
                //        var Role = db.Role.Where(x => x.RoleId == RoleId).FirstOrDefault();

                //        // var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                //        // identity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
                //        identity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(identity));
                //        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                //        // identity.AddClaim(new Claim(ClaimTypes.Role, user));
                //        identity.AddClaim(new Claim("firsttime", "true"));
                //        identity.AddClaim(new Claim("compid", s.CompanyId.ToString()));
                //        identity.AddClaim(new Claim("orgid", s.OrgId.ToString()));
                //        identity.AddClaim(new Claim("email", user.Email));
                //        identity.AddClaim(new Claim("FirstName", s.FirstName));
                //        identity.AddClaim(new Claim("LastName", s.LastName));
                //        // identity.AddClaim(new Claim("ImageUrl", p.ImageUrl));
                //        identity.AddClaim(new Claim("Email", user.Email.ToString()));
                //        identity.AddClaim(new Claim("Level", user.Level.ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        // identity.AddClaim(new Claim("DisplayName", p.DisplayName));
                //        // identity.AddClaim(new Claim("username", (s.UserName).ToString()));
                //        identity.AddClaim(new Claim("userid", UserId.ToString()));
                //        // identity.AddClaim(new Claim("Roleids", string.Join(",", RoleId)));

                //        var props = new AuthenticationProperties(new Dictionary<string, string> { });

                //        props = new AuthenticationProperties(new Dictionary<string, string>
                //        {
                //            { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId },
                //            { "userName", context.UserName },
                //            { "Level", user.Level.ToString() },
                //            { "FirstName", s.FirstName},
                //            { "LastName", s.LastName},
                //            { "compid", s.CompanyId.ToString()},
                //            { "Roleids" ,RoleId.ToString()},
                //            { "email" ,user.Email },
                //            { "userid", UserId.ToString() },
                //            //{"rolename",Role.RoleType},
                //            { "orgid", s.OrgId.ToString()},
                //        });
                //        var ticket = new AuthenticationTicket(identity, props);
                //        context.Validated(ticket);
                //    }
                //}

                #endregion Old Codes
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                logger.Error(ex, line.ToString());
                context.SetError("exception", "oAuthExc - " + ex);
                return;
            }
        }

        //---23.02.2022
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            //User traking Code strated....
            //UserTraking us = new UserTraking();
            //us.PeopleId = User_Id + "";
            //us.Type = ac_Type;
            //us.LoginTime = DateTime.Now;
            //us.Remark = "login page ,";
            //dc.UserTrakings.Add(us);
            //dc.SaveChanges();
            //END User traking Code....
            return Task.FromResult<object>(null);
        }
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //--Ends
    }
}