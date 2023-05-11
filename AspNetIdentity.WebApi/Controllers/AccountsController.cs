using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using AspNetIdentity.WebApi.Result;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Helper.ClientHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;
namespace AspNetIdentity.WebApi.Controllers
{

    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        private ApplicationDbContext _db;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public AccountsController()
        {
            _db = new ApplicationDbContext();
        }

        #region Create Super Admin

        [HttpPost]
        [Route("createSuperUser")]
        public async Task<ResponseBodyModel> CreateSuperUser(CreateUserBindingModel createUserModel)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            if (createUserModel == null)
            {
                res.Message = "Model Is Invalid";
                res.Status = false;
            }
            else
            {
                Model.User usersData = new Model.User();
                var Password = createUserModel.Password;
                var keynew = DataHelper.GeneratePasswords(10);
                var passw = DataHelper.EncodePassword(Password, keynew);
                byte Levels = 4;
                var split = createUserModel.Username.Split('@');
                createUserModel.Username = split[0] + "@" + split[1].ToLower();

                var user = new ApplicationUser()
                {
                    FirstName = createUserModel.FirstName,
                    LastName = createUserModel.LastName,
                    PhoneNumber = "",
                    Level = Levels,
                    JoinDate = DateTime.Now.Date,
                    EmailConfirmed = true,
                    Email = createUserModel.Username,
                    PasswordHash = keynew,
                    UserName = createUserModel.Username
                };
                IdentityResult result = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
                if (result.Succeeded)
                {
                    usersData.UserName = createUserModel.Username;
                    usersData.Password = passw;
                    usersData.HashCode = keynew;
                    usersData.DepartmentId = 0;
                    usersData.LoginId = LoginRolesConstants.SuperAdmin;
                    usersData.CreatedOn = DateTime.Now;
                    usersData.UpdatedOn = DateTime.Now;
                    usersData.IsActive = true;
                    usersData.IsDeleted = false;
                    usersData.CompanyId = 0;
                    usersData.OrgId = 0;

                    _db.User.Add(usersData);
                    _db.SaveChanges();

                    res.Message = "Data Saved Successfully";
                    res.Status = true;
                    res.Data = usersData;
                }
                else
                {
                    res.Message = result.Errors.First();
                    res.Status = false;
                }
            }
            return res;
        }

        #endregion Create Super Admin

        #region Api To Get Super Admin DashBoard

        /// <summary>
        /// Created By Harshit Mitra on 05-05-2022
        /// API >> Get >> api/accounts/getsuperadmindashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getsuperadmindashboard")]
        public async Task<ResponseBodyModel> GetSuperAdminLineGraph()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            SuperAdminDashBoard response = new SuperAdminDashBoard();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                if (claims.roleType == "SuperAdmin")
                {
                    var company = await _db.Company.Where(x => x.IsDeleted == false && x.IsActive == true).ToListAsync();
                    if (company.Count > 0)
                    {
                        #region For Line Graph

                        List<OuterSeriesPartLineGraph> graphList = new List<OuterSeriesPartLineGraph>();
                        foreach (var item in company)
                        {
                            List<InnerSeriesPartLineGraph> innerList = new List<InnerSeriesPartLineGraph>();
                            OuterSeriesPartLineGraph obj = new OuterSeriesPartLineGraph();
                            for (int i = 1; i <= currentMonth; i++)
                            {
                                InnerSeriesPartLineGraph inner = new InnerSeriesPartLineGraph
                                {
                                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i),
                                    Value = _db.Employee.Where(x => x.IsActive && !x.IsDeleted &&
                                            x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && x.CompanyId == item.CompanyId &&
                                            x.CreatedOn.Month <= i && x.CreatedOn.Year <= currentYear).ToList().Count
                                };
                                innerList.Add(inner);
                            }
                            obj.Name = item.RegisterCompanyName;
                            obj.Series = innerList;
                            graphList.Add(obj);
                        }
                        response.LineGraph = graphList;

                        #endregion For Line Graph

                        #region For Table

                        var orglist = (from c in company
                                       join o in _db.OrgMaster on c.CompanyId equals o.CompanyId
                                       where o.IsActive == true && o.IsDeleted == false &&
                                       c.IsActive == true && c.IsDeleted == false
                                       select new OrgList
                                       {
                                           CompanyId = c.CompanyId,
                                           CompanyName = c.RegisterCompanyName,
                                           OrgId = o.OrgId,
                                           OrgName = o.OrgName,
                                           CreatedOn = o.CreatedOn,
                                           EmployeeCount = _db.Employee.Where(x => x.IsActive && !x.IsDeleted &&
                                                        x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                                        x.CompanyId == c.CompanyId && x.OrgId == o.OrgId).ToList().Count,
                                       }).ToList();
                        response.OrgListTable = orglist;

                        #endregion For Table

                        res.Message = "Super Admin Dashboard";
                        res.Status = true;
                        res.Data = response;
                    }
                    else
                    {
                        res.Message = "No Company Found";
                        res.Status = false;
                        res.Data = response;
                    }
                }
                else
                {
                    res.Message = "You Dont Have Authority";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Super Admin DashBoard

        #region This is use for forget password

        /// <summary>
        /// Create by Shriya on 09-06-2022
        /// Updated By Harshit Mitra On 10-06-2022
        /// API >> Post >> api/accounts/forgetemail
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forgetemail")]
        public async Task<ResponseBodyModel> ForgetMail(ModalForForget modal)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var exitingemail = await _db.Employee.Where(x => x.OfficeEmail == modal.Email &&
                            x.IsActive == true && x.IsDeleted == false).Select(x => x.OfficeEmail)
                            .FirstOrDefaultAsync();
                if (exitingemail != null)
                {
                    ApplicationUser applicationUser = await this.AppUserManager.FindByNameAsync(exitingemail);
                    if (applicationUser != null)
                    {
                        return (SendForgetMail(modal.Email, modal.BaseURL) ?
                            new ResponseBodyModel
                            {
                                Message = "Reset Password Link succesfully send in your register EmailId",
                                Status = true,
                                Data = exitingemail,
                            } :
                            new ResponseBodyModel
                            {
                                Message = "Failed To Send Mail Try After Some Time",
                                Status = false,
                            });
                    }
                }
                res.Message = "Your enter mail not found please try again ! ";
                res.Status = false;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is use for forget password

        #region This Api Use To Send ForgetMaill
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public bool SendForgetMail(string forgetemail, string baseurl)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var employee = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.OfficeEmail == forgetemail).FirstOrDefault();
            try
            {
                var key = ConfigurationManager.AppSettings["EncryptKey"];
                var data = "UserEmail=" + forgetemail;
                string token = EncryptDecrypt.EncryptData(key, data);
                //string path = baseurl + "/#/authentication/change-password-for-admin?token=" + token;
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (tokenData.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == tokenData.companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                string path = baseurl + "/#/authentication/create-new-password?token=" + token;
                //string pdflocation = ConfigurationManager.AppSettings["PDFLocation"] + CandidateId + ".pdf";
                string fcode = "<body style=' display: flex;align-items: center;justify-content: center;height:100vh;'>";
                fcode += "<div class='  flextcontainer card p-2' style='text-align: center;width: 83%; min-height: 50px;position: relative;margin-bottom: 24px;border: 1px solid #f2f4f9;border-radius: 10px;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>";
                fcode += "<img class='imgg mb-2' style='  width: 40%;margin: auto;display: block;' src='" + baseurl + "'+'/assets/logo-moreyeahs.png'>";
                fcode += "<hr>";
                fcode += "<h1 class='mt-2 mb-2' style='margin-top: 10px;margin-bottom:10px;'>Reset Your Password ?</h1>";
                fcode += "<div class='m-2 mb-3'>";
                fcode += "<label style='margin-top: 10px;margin-bottom:20px;' >You are receiving this email because you are requested for password reset for MoreYeahs. Click on the link below to set new password.</label>";
                fcode += "<br><br>";
                fcode += "</div>";
                fcode += "<div>";
                fcode += "<a  style='margin-top:20px;margin-botton:20px;background: #911924;border-color: #911924;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;text-transform: uppercase;' href='" + path + "'>Reset My Password</a>";
                fcode += "<br><br>";
                fcode += "</div>";
                fcode += "<div class='m-2 mb-3'>";
                fcode += "<label  style='margin-top: 20px;margin-bottom:10px;' >If you don't requested to change your password simply ignore this email.</label>";
                fcode += "</div>";
                fcode += "</div>";
                fcode += "</body>";
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                    Subject = "Reset Your Password",
                    MailBody = fcode,
                    MailTo = new List<string>() { forgetemail },
                    SmtpSettings = smtpsettings,
                };
                SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Api To Reset User Password
        /// <summary>
        /// Created By Harshit Mitra on 09-06-2022
        /// API >> Post >> api/accounts/resetpassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resetpassword")]
        public async Task<ResponseBodyModel> ResetPasseord(ResetPasswordHelperModelClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var key = ConfigurationManager.AppSettings["EncryptKey"];
                var decryptData = EncryptDecrypt.DecryptData(key, model.Token.Replace(" ", "+"));
                var data = decryptData.Split(new string[] { "<&>" }, StringSplitOptions.None);
                Dictionary<string, string> dictionary = data.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
                //var userEmail = dictionary.FirstOrDefault(x => x.Key == "UserEmail").Value.ToString();
                var splitdata = decryptData.Split('=');
                var userEmail = splitdata[1];
                //var token = dictionary.FirstOrDefault(x => x.Key == "VerifyToken").Value.ToString();


                ApplicationUser applicationUser = await this.AppUserManager.FindByNameAsync(userEmail);
                if (applicationUser != null)
                {
                    var userData = _db.User.Where(x => x.IsDeleted == false && x.UserName == userEmail).FirstOrDefault();
                    if (userData != null)
                    {
                        var employee = _db.Employee.Where(x => x.OfficeEmail == userEmail).FirstOrDefault();
                        if (employee != null)
                        {
                            //await this.AppUserManager.ResetPasswordAsync(applicationUser.Id, token, model.ConfirmPassword);
                            await this.AppUserManager.RemovePasswordAsync(applicationUser.Id);
                            await this.AppUserManager.AddPasswordAsync(applicationUser.Id, model.ConfirmPassword);


                            var keynew = DataHelper.GeneratePasswords(10);
                            var passw = DataHelper.EncodePassword(model.ConfirmPassword, keynew);


                            userData.Password = null;
                            userData.HashCode = null;


                            userData.Password = passw;
                            userData.HashCode = keynew;


                            employee.Password = null;
                            employee.Password = model.ConfirmPassword;


                            _db.Entry(userData).State = EntityState.Modified;
                            _db.Entry(employee).State = EntityState.Modified;
                            await _db.SaveChangesAsync();

                            ResetPaswordModel obj = new ResetPaswordModel
                            {
                                Email = userEmail,
                                Password = employee.Password,
                            };

                            res.Message = "Password Reset";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            res.Message = "User Not Found";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "User Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "User Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            _db.Dispose();
            return res;
        }



        #endregion Api To Reset User Password

        #region Helper Model Class

        public class SuperAdminDashBoard
        {
            public List<OuterSeriesPartLineGraph> LineGraph { get; set; }
            public List<OrgList> OrgListTable { get; set; }
        }

        public class OuterSeriesPartLineGraph
        {
            public string Name { get; set; }
            public List<InnerSeriesPartLineGraph> Series { get; set; }
        }

        public class InnerSeriesPartLineGraph
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class OrgList
        {
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public int EmployeeCount { get; set; }
        }

        public class ResetPasswordHelperModelClass
        {
            public string ConfirmPassword { get; set; }
            public string Token { get; set; }
        }


        public class ModalForForget
        {
            public string Email { get; set; }
            public string BaseURL { get; set; }
        }

        public class ResetPaswordModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        #endregion Helper Model Class

        #region API to search the data from the created companies on behalf of company name and organization name
        /// <summary>
        ///  API > GET > api/accounts/searchdataforsuperadmin
        ///  Created By Bhavendra Singh Jat 28-08-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("searchdataforsuperadmin")]
        public async Task<ResponseBodyModel> GetAllOrganizationDataSearch(string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "SuperAdmin")
                {
                    var company = await _db.Company.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
                    if (company.Count > 0)
                    {
                        var searchData = (from com in _db.Company
                                          join org in _db.OrgMaster on com.CompanyId equals org.CompanyId
                                          where org.IsActive && !org.IsDeleted && com.IsActive && !com.IsDeleted
                                          && (org.OrgName.ToLower().Contains(search.ToLower()) || com.RegisterCompanyName.ToLower().Contains(search.ToLower()))
                                          select new
                                          {
                                              CompanyId = com.CompanyId,
                                              CompanyName = com.RegisterCompanyName,
                                              OrgId = org.OrgId,
                                              OrgName = org.OrgName,
                                              CreatedOn = org.CreatedOn,
                                              EmployeeCount = _db.Employee.Where(x => x.IsActive && !x.IsDeleted &&
                                                           x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                                           x.CompanyId == com.CompanyId && x.OrgId == org.OrgId).ToList().Count,
                                          }).ToList();
                        if (searchData != null)
                        {
                            res.Message = "Search Result Found";
                            res.Status = true;
                            res.Data = searchData;
                        }
                        else
                        {
                            res.Message = "Search Result Not Found";
                            res.Status = false;
                            res.Data = searchData;
                        }
                    }
                    else
                    {
                        res.Message = "No Company Found";
                        res.Status = false;
                        res.Data = null;
                    }
                }
                else
                {
                    res.Message = "You Dont Have Authority";
                    res.Status = false;
                    res.Data = null;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        #endregion


        #region Migrated APIs

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize]
        public IHttpActionResult GetAllUsers()
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            Base response = new Base();
            var UserData = _db.User.Where(x => x.IsDeleted == false).ToList();
            if (UserData.Count > 0)
            {
                response.StatusReason = true;
                response.Message = "Record Found";
                response.UserData = UserData;
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!";
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetUserById")]
        [Authorize]
        public async Task<IHttpActionResult> GetUser(int Id)
        {
            ////Only SuperAdmin or Admin can delete users (Later when implement roles)

            Base response = new Base();
            var UserData = await _db.User.Where(x => x.IsDeleted == false && x.UserId == Id).ToListAsync();
            if (UserData.Count > 0)
            {
                response.StatusReason = true;
                response.Message = "Record Found";
                response.UserData = UserData;
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!";
            }
            return Ok(response);
        }

        [Route("GetUserByUsername")]
        [Authorize]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            Base response = new Base();
            var UserData = await _db.User.Where(x => x.IsDeleted == false && x.UserName == username).FirstOrDefaultAsync();
            if (UserData != null)
            {
                response.StatusReason = true;
                response.Message = "Record Found";
                response.userAssociation = UserData;
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!";
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("createUser")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            Base response = new Base();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Model.User usersData = new Model.User();
                usersData.UserName = createUserModel.Username;
                usersData.Password = createUserModel.Password;
                usersData.DepartmentId = createUserModel.RoleId;
                usersData.CreatedOn = DateTime.Now;
                usersData.UpdatedOn = DateTime.Now;
                usersData.IsActive = true;
                usersData.IsDeleted = false;
                usersData.CompanyId = createUserModel.CompanyId;
                usersData.OrgId = createUserModel.OrgId;

                _db.User.Add(usersData);
                await _db.SaveChangesAsync();

                response.StatusReason = true;
                response.Message = "Data Saved Successfully";
            }
            return Ok(response);
        }

        [HttpDelete]
        //[Route("user/{id:guid}")]
        [Route("DeleteUser")]
        [Authorize]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            Base response = new Base();
            var UserData = await _db.User.Where(x => x.IsDeleted == false && x.UserId == id).FirstOrDefaultAsync();
            if (UserData != null)
            {
                UserData.IsDeleted = true;
                _db.Entry(UserData).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                response.StatusReason = true;
                response.Message = "Deleted Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!!";
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        [Authorize]
        public async Task<IHttpActionResult> ConfirmEmail(int userId = 0, string UserName = "")
        {
            Base response = new Base();
            var UserData = await _db.User.Where(x => x.IsDeleted == false && x.UserName == UserName && x.UserId == userId).FirstOrDefaultAsync();

            if (UserData != null)
            {
                response.StatusReason = true;
                response.Message = "Email Confirmed";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Please enter correct Email & UserId";
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            Base response = new Base();
            //var keynew = DataHelper.GeneratePassword(10);
            //var passw = DataHelper.EncodePassword(model.OldPassword, model.OldPassword);
            //var hasCode = model.OldPassword.HashCode;
            //var newPwd = DataHelper.EncodePassword(model.OldPassword, hasCode);

            var CheckUser = await _db.User.Where(x => x.IsDeleted == false && x.EmployeeId == model.UserId).FirstOrDefaultAsync();
            if (CheckUser != null)
            {
                var hashCode = CheckUser.HashCode;
                var password = model.OldPassword;
                var encodingPassword = DataHelper.EncodePassword(password, hashCode);
                var UserData = _db.User.Where(x => x.IsDeleted == false && x.EmployeeId == CheckUser.UserId && x.Password.Equals(encodingPassword)).FirstOrDefault();
                if (UserData != null)
                {
                    var Password = model.NewPassword;
                    var keynew = DataHelper.GeneratePasswords(10);
                    var passw = DataHelper.EncodePassword(Password, keynew);
                    UserData.Password = passw;
                    UserData.UpdatedOn = DateTime.Now;
                    _db.Entry(UserData).State = System.Data.Entity.EntityState.Modified;

                    var Emp = (from ad in _db.Employee where ad.EmployeeId == UserData.EmployeeId select ad).FirstOrDefault();

                    Emp.Password = model.NewPassword;
                    _db.Entry(Emp).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    response.StatusReason = true;

                    //UserData.Password = model.NewPassword;
                    _db.SaveChanges();
                }

                response.StatusReason = true;
                response.Message = "Password Updated Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Email Address not exist!!";
            }
            return Ok(response);
        }

        //[HttpPut]
        //[Route("ChangeUserPassword")]
        //public async Task<IHttpActionResult> ChangeUserPassword(ChangePasswordBindingModel model)
        //{
        //    Base response = new Base();
        //    var keynew = DataHelper.GeneratePassword(10);
        //    var passw = DataHelper.EncodePassword(model.OldPassword, keynew);
        //    var checkUser = (from ad in db.User where ad.UserName == model.UserName select ad).FirstOrDefault();

        //    if (checkUser != null)
        //    {
        //        var hasCode = checkUser.HashCode;
        //        var newPwd = DataHelper.EncodePassword(model.NewPassword, hasCode);
        //        checkUser.Password = newPwd;
        //        checkUser.UpdatedOn = DateTime.Now;
        //        db.Entry(checkUser).State = System.Data.Entity.EntityState.Modified;
        //        var Emp = (from ad in db.Employee where ad.EmployeeId == checkUser.EmployeeId select ad).FirstOrDefault();
        //        Emp.Password = model.NewPassword;
        //        db.Entry(Emp).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        response.StatusReason = true;
        //        response.Message = "Password Updated Successfully";
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = "Email Address not exist!!";
        //    }

        //    return Ok(response);
        //}

        [HttpPut]
        [Route("ChangeUserPassword")]
        [Authorize]
        public async Task<IHttpActionResult> ChangeUserPassword(ChangePasswordBindingModel model)
        {
            Base response = new Base();
            var emp = _db.Employee.Any(x => x.Password == model.OldPassword);
            //var keynew = DataHelper.GeneratePassword(10);
            // var passw = DataHelper.EncodePassword(model.OldPassword, keynew);
            var checkUser = await (from ad in _db.User where ad.EmployeeId == model.UserId select ad).FirstOrDefaultAsync();
            if (emp == true && checkUser != null)
            {
                var hasCode = DataHelper.GeneratePasswords(10);
                // var hasCode = checkUser.HashCode;
                var newPwd = DataHelper.EncodePassword(model.NewPassword, hasCode);
                checkUser.Password = newPwd;
                checkUser.HashCode = hasCode;
                checkUser.UpdatedOn = DateTime.Now;
                _db.Entry(checkUser).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                var Emp = (from ad in _db.Employee where ad.EmployeeId == checkUser.EmployeeId select ad).FirstOrDefault();
                Emp.Password = model.NewPassword;
                _db.Entry(Emp).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = manager.FindByName(checkUser.UserName);

                manager.RemovePassword(user.Id);
                manager.AddPassword(user.Id, model.NewPassword);

                response.StatusReason = true;
                response.Message = "Password Updated Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Incorrect Old Password!!";
            }
            return Ok(response);
        }

        //[HttpPut]
        //[Route("ResetPassword")]
        //public async Task<IHttpActionResult> ResetPassword(SetPasswordBindingModel model)
        //{
        //    Base response = new Base();
        //    var UserData = db.User.Where(x => x.IsDeleted == false && x.UserName == model.Email && x.UserName == model.UserName).FirstOrDefault();
        //    if (UserData != null)
        //    {
        //        UserData.Password = model.NewPassword;
        //        db.SaveChanges();
        //        response.StatusReason = true;
        //        response.Message = "Password Reset Successfully";
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = "No Record Found!!";
        //    }

        //    return Ok(response);
        //}

        [Route("user/{id:guid}/roles")]
        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] int id, [FromBody] int rolesToAssign)
        {
            Base response = new Base();
            var UserData = await _db.User.Where(x => x.IsDeleted == false && x.UserId == id).FirstOrDefaultAsync();
            if (UserData != null)
            {
                UserData.DepartmentId = rolesToAssign;
                _db.SaveChanges();
                response.StatusReason = true;
                response.Message = "Role assigned Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!!";
            }

            return Ok(response);
        }

        //[HttpGet]
        //[Route("Login")]
        //public IHttpActionResult Login(User userData)
        //{
        //    Base response = new Base();
        //    var UserData = db.User.Where(x => x.IsDeleted == false && x.Email == userData.Email && x.Password == userData.Password).FirstOrDefault();
        //    if (UserData != null)
        //    {
        //        response.userAssociation = UserData;
        //        response.StatusReason = true;
        //        response.Message = "successfully logged in";
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = "Something went wrong";
        //    }

        //    return Ok(response);
        //}

        #endregion Migrated APIs





        [HttpPost]
        [Route("deleteemp")]
        public async Task<IHttpActionResult> DeleteEployee(string OfficalEmail)
        {
            var result = await this.AppUserManager.FindByNameAsync(OfficalEmail);
            await this.AppUserManager.DeleteAsync(result);
            var emp = _db.Employee.FirstOrDefault(x => x.OfficeEmail == OfficalEmail);
            var user = _db.User.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
            _db.Entry(emp).State = EntityState.Deleted;
            _db.Entry(user).State = EntityState.Deleted;

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("Logout")]
        [Authorize]
        public IHttpActionResult Logout()
        {
            this.AppUserManager.GetLockoutEnabledAsync(User.Identity.GetUserId());
            return Ok();
        }

        [HttpPut]
        [Route("AssignClaimsToUser")]
        [Authorize]
        public async Task<IHttpActionResult> AssignClaimsToUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToAssign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            foreach (ClaimBindingModel claimModel in claimsToAssign)
            {
                if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type))
                {
                    await this.AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
                }

                await this.AppUserManager.AddClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
            }

            return Ok();
        }

        [HttpPut]
        [Route("RemoveClaimsFromUser")]
        [Authorize]
        public async Task<IHttpActionResult> RemoveClaimsFromUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToRemove)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            foreach (ClaimBindingModel claimModel in claimsToRemove)
            {
                if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type))
                {
                    await this.AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
                }
            }

            return Ok();
        }

        [HttpPut]
        [Route("ResetPassword")]
        [Authorize]
        public async Task<IHttpActionResult> ResetPassword(SetPasswordBindingModel model)
        {
            Base response = new Base();
            var keynew = DataHelper.GeneratePasswords(10);
            var passw = DataHelper.EncodePassword(model.NewPassword, keynew);
            var userData = _db.User.Where(x => x.IsDeleted == false && x.UserName == model.UserName && x.OTP == model.OTP).FirstOrDefault();
            if (userData != null)
            {
                var hasCode = userData.HashCode;
                var newPwd = DataHelper.EncodePassword(model.NewPassword, hasCode);

                userData.Password = newPwd;
                userData.UpdatedOn = DateTime.Now;
                _db.Entry(userData).State = System.Data.Entity.EntityState.Modified;
                var Emp = (from ad in _db.Employee where ad.EmployeeId == userData.EmployeeId select ad).FirstOrDefault();

                Emp.Password = model.NewPassword;
                _db.Entry(Emp).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();
                response.StatusReason = true;
                response.Message = "Password Reset Successfully";

                //UserEmailDTOResponse responsemail = new UserEmailDTOResponse();
                //{
                // String body = "Your message : <a href ='https://hrms.moreyeahs.in/#/authentication/reset-password'>
                // UserEmail mailmodel = new UserEmail();
                // mailmodel.To = Emp.MoreyeahsMailId;
                // mailmodel.Subject = "Reset Password Link : MoreYeahs HRMS"; //add subject here
                // mailmodel.Body = "Hi there, Please reset your password with the below link " + body
                // + "Best Regards <br>" +
                // "MoreYeahs HR and Management<br>";
                // var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                // response.Message = "Mail Sent Sucessfully";
                // response.StatusReason = true;
                //}
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Please insert Correct OTP or UserName";
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("GenrateOTP")]
        [Authorize]
        public async Task<IHttpActionResult> GenrateOTP(SetPasswordBindingModel model)
        {
            Base response = new Base();
            Random random = new Random();
            int value = random.Next(10000);
            var Getdata = await _db.User.Where(x => x.UserName == model.UserName).FirstOrDefaultAsync();
            if (Getdata != null)
            {
                Getdata.OTP = value;
                _db.SaveChanges();
                UserEmailDTOResponse responsemail = new UserEmailDTOResponse();
                {
                    UserEmail mailmodel = new UserEmail();
                    mailmodel.To = model.UserName;
                    mailmodel.Subject = "Reset Password Otp : MoreYeahs HRMS"; //add subject here
                    mailmodel.Body = "OTP For Reset Password is " + value;
                    var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                    response.Message = "Mail Sent Sucessfully";
                    response.StatusReason = true;
                }

                response.StatusReason = true;
                response.Message = "OTP is Send To Your Email.";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Email Address not exist!!";
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("SendURL")]
        [Authorize]
        public async Task<IHttpActionResult> sendURl(SetPasswordBindingModel model)
        {
            Base response = new Base();
            var Getdata = await _db.User.Where(x => x.UserName == model.UserName).FirstOrDefaultAsync();
            var Emp = (from ad in _db.Employee where ad.EmployeeId == Getdata.EmployeeId select ad).FirstOrDefault();
            if (Getdata != null)
            {
                UserEmailDTOResponse responsemail = new UserEmailDTOResponse();
                {
                    String body = @"Your message : <a href =""https://uathrmsfrontend.moreyeahs.in/#/authentication/reset-password"">Click here </a>";
                    UserEmail mailmodel = new UserEmail();
                    //mailmodel.To = Emp.MoreyeahsMailId;
                    mailmodel.To = Emp.OfficeEmail;
                    mailmodel.Subject = "Reset Password Link : MoreYeahs HRMS "; //add subject here
                    mailmodel.Body = "Hi there, Please reset your password with the below link <br>" + body
                    + "Best Regards <br>" +
                    "MoreYeahs HR and Management<br>";
                    var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                    response.Message = "Mail Sent Sucessfully";
                    response.StatusReason = true;
                }

                response.StatusReason = true;
                response.Message = "Instructions are Sent To Your Email.";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "Email Address not exist!!";
            }
            return Ok(response);
        }


        #region API For User Login 
        /// <summary>
        /// Created By Harshit Mitra On 22-11-2022
        /// API >> POST >> api/accounts/userlogin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userlogin")]
        [AllowAnonymous]
        public async Task<object> Login(UserLoginRequest model)
        {
            try
            {
                logger.Debug("api/accounts/userlogin | Model : " + JsonConvert.SerializeObject(model));
                var applicationUser = await AppUserManager.FindByNameAsync(model.UserName);
                if (applicationUser != null)
                {
                    if (await AppUserManager.CheckPasswordAsync(applicationUser, model.Password))
                    {
                        var user = _db.User.Where(x => x.UserName == applicationUser.UserName).First();
                        if (user != null)
                        {
                            var hashCode = user.HashCode;
                            var password = model.Password;
                            var encodingPassword = DataHelper.EncodePassword(password, hashCode);
                            if (user.Password == encodingPassword)
                            {
                                var returnData = TokenConfig.GetToken(user, user.LoginId);
                                _db.Dispose();
                                return returnData;
                            }
                            else
                                return new { Message = "Password Is Incorrect", Status = false, };
                        }
                        else
                            return new { Message = "Password Is Incorrect", Status = false, };
                    }
                    return new { Message = "Password Is Incorrect", Status = false, };
                }
                return new { Message = "User Not Found", Status = false, };
            }
            catch (Exception ex)
            {
                logger.Error("api/accounts/userlogin | Error : " + JsonConvert.SerializeObject(ex));
                return new { /*Message = "User Not Found"*/ex.Message, Status = false };
            }
        }
        public class UserLoginRequest
        {
            [Required]
            [EmailAddress]
            public string UserName { get; set; }
            [Required]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

        }
        #endregion


        #region API For Moblie User Login 
        /// <summary>
        /// Created By Harshit Mitra On 22-11-2022
        /// API >> POST >> api/accounts/micorosoftmobilelogin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("micorosoftmobilelogin")]
        [AllowAnonymous]
        public async Task<object> MobileLogin(MoblieUserLoginRequest model)
        {
            try
            {
                logger.Debug("api/accounts/micorosoftmobilelogin | Model : " + model);
                var applicationUser = await AppUserManager.FindByNameAsync(model.UserName);
                if (applicationUser != null)
                {
                    var user = _db.User.Where(x => x.UserName == applicationUser.UserName).First();
                    if (user != null)
                    {
                        var returnData = TokenConfig.GetToken(user, user.LoginId);
                        _db.Dispose();
                        return returnData;
                    }
                    else
                        return new { Message = "User Not Found", Status = false, };
                }
                return new { Message = "User Not Found", Status = false, };
            }
            catch (Exception ex)
            {
                logger.Error("api/accounts/micorosoftmobilelogin | Error : " + JsonConvert.SerializeObject(ex));
                return new { /*Message = "User Not Found"*/ex.Message, Status = false };
            }
        }
        public class MoblieUserLoginRequest
        {
            [Required]
            [EmailAddress]
            public string UserName
            {
                get; set;
            }
        }
        #endregion
        //public async Task<object> ValidateToken(string token)
        //{
        //    return TokenConfig.GetToken(user, user.LoginId);
        //}

        #region External Login Method

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string redirect_uri, string error = null)
        {
            StackFrame callStack = new StackFrame(1, true);
            try
            {
                logger.Debug("api/Account/ExternalLogin | provider : " + provider + " | redirect_uri : " + redirect_uri);
                if (error != null)
                {
                    return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
                }
                if (!User.Identity.IsAuthenticated)
                {
                    return new ChallengeResult(provider, this);
                }
                ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
                if (externalLogin == null)
                {
                    return InternalServerError();
                }
                if (externalLogin.LoginProvider != provider)
                {
                    Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    return new ChallengeResult(provider, this);
                }
                ApplicationUser applicationUser = await this.AppUserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));
                bool hasRegistered = applicationUser != null;
                if (hasRegistered)
                {
                    var user = _db.User.Where(x => x.UserName == applicationUser.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        //return TokenConfig.GetToken(user, user.LoginId);
                        return Redirect(redirect_uri + "#/authentication/login?token=" + TokenConfig.GetToken(user, user.LoginId, true));
                    }
                    //return Ok(TokenConfig.GetToken(user.UserName));
                    //return new { Message = "User Not Found", Status = false, };
                    return Redirect(redirect_uri + "#/authentication/login?token=Access_Denied");
                }
                else
                {
                    var info = await Authentication.GetExternalLoginInfoAsync();
                    applicationUser = await AppUserManager.FindByNameAsync(info.Email);
                    if (applicationUser == null)
                    {
                        return Redirect(redirect_uri + "#/authentication/login?token=Access_Denied");
                    }
                    else
                    {
                        var result = await AppUserManager.AddLoginAsync(applicationUser.Id, info.Login);
                        if (!result.Succeeded)
                        {
                            return Redirect(redirect_uri + "#/authentication/login?token=Access_Denied");
                        }
                        else
                        {
                            var user = _db.User.Where(x => x.UserName == applicationUser.UserName).First();
                            if (user != null)
                            {
                                return Redirect(redirect_uri + "#/authentication/login?token=" + TokenConfig.GetToken(user, user.LoginId, true));
                            }
                            return Redirect(redirect_uri + "#/authentication/login?token=Access_Denied");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Account/ExternalLogin | Error : " + JsonConvert.SerializeObject(ex));
                return BadRequest(ex.Message);
            }
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = info.DefaultUserName,
                LastName = info.DefaultUserName,
                JoinDate = DateTime.Now,
                CompanyId = 1,
            };
            try
            {
                IdentityResult result = await AppUserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                result = await AppUserManager.AddLoginAsync(user.Id, info.Login);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Account/RegisterExternal | Error : " + JsonConvert.SerializeObject(ex));
                throw;
            }
            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [HttpGet]
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public ResponseBodyModel GetExternalLogins(string returnUrl, bool generateState = false)
        {
            logger.Debug("Getting Into Get External Login | Return Url : " + returnUrl);
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();
            try
            {
                string state;

                if (generateState)
                {
                    const int strengthInBits = 256;
                    state = RandomOAuthStateGenerator.Generate(strengthInBits);
                }
                else
                {
                    state = null;
                }
                foreach (AuthenticationDescription description in descriptions)
                {
                    ExternalLoginViewModel login = new ExternalLoginViewModel
                    {
                        Name = description.Caption,
                        Url = Url.Route("ExternalLogin", new
                        {
                            provider = description.AuthenticationType,
                            response_type = "token",
                            client_id = "self",
                            redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                            state = state
                        }),
                        State = state
                    };
                    logins.Add(login);
                }
                return new ResponseBodyModel
                {
                    Message = "Login Urls",
                    Status = true,
                    Data = logins,
                };
            }
            catch (Exception ex)
            {
                logger.Error("Exception : " + JsonConvert.SerializeObject(ex));
                throw;
            }
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private IHttpActionResult GetErrorResult(IdentityResult result)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            if (result == null)
            {
                return InternalServerError();
            }



            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }



                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }



                return BadRequest(ModelState);
            }



            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }



            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));



                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                    claims.Add(new Claim(ClaimTypes.Email, Email, null, LoginProvider));
                }



                return claims;
            }



            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }



                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);



                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                     || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }



                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }



                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    Email = identity.FindFirstValue(ClaimTypes.Email)



                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;



                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }



                int strengthInBytes = strengthInBits / bitsPerByte;



                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
        public class ExternalLoginViewModel
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public string State { get; set; }
        }

        public class ManageInfoViewModel
        {
            public string LocalLoginProvider { get; set; }

            public string Email { get; set; }

            public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

            public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
        }

        public class UserInfoViewModel
        {
            public string Email { get; set; }

            public bool HasRegistered { get; set; }

            public string LoginProvider { get; set; }
        }

        public class UserLoginInfoViewModel
        {
            public string LoginProvider { get; set; }

            public string ProviderKey { get; set; }
        }
        #endregion

        #endregion

        #region API To Get User Login Info 

        [HttpPost]
        [Route("getuserlogindetail")]
        public object GetUserLoginDetail(string token)
        {
            try
            {
                if (!String.IsNullOrEmpty(token) && token != "null")
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token);
                    var tokenS = jsonToken as JwtSecurityToken;
                    return ClaimsHelper.GetTokenData(tokenS);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Debug("api/account/getuserlogindetail", ex, token);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region API For User Login To Tsf
        /// <summary>
        /// Created By Ankit Jain On 03-04-2023
        /// API >> POST >> api/accounts/userlogintsf
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userlogintsf")]
        [AllowAnonymous]
        public async Task<object> Logintsf(UserLoginRequestTsf model)
        {
            try
            {
                logger.Debug("api/accounts/userlogin | Model : " + JsonConvert.SerializeObject(model));
                var applicationUser = await AppUserManager.FindByNameAsync(model.OfficeEmail);
                if (applicationUser != null)
                {
                    var employee = _db.Employee.Where(x => x.OfficeEmail == applicationUser.UserName).FirstOrDefault();
                    if (employee != null)
                    {
                        var returnData = TokenConfig.GetTokenTsf(employee);
                        _db.Dispose();
                        return returnData;
                    }
                }
                return new { Message = "User Not Found", Status = false, };
            }
            catch (Exception ex)
            {
                logger.Error("api/accounts/userlogintsf | Error : " + JsonConvert.SerializeObject(ex));
                return new { /*Message = "User Not Found"*/ex.Message, Status = false };
            }
        }
        public class UserLoginRequestTsf
        {
            [Required]
            [EmailAddress]
            public string OfficeEmail { get; set; }
        }
        #endregion API For User Login To Tsf

    }

}