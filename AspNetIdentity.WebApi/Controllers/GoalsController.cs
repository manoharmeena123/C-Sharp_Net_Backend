using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/goal")]
    public class GoalsController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api To Get GoalType

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// API >> Get >> api/goal/getgolatype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getgolatype")]
        public ResponseBodyModel GetAllBloodType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var bloodGroup = Enum.GetValues(typeof(GoalTypeEnum_PTStateDuration))
                                .Cast<GoalTypeEnum_PTStateDuration>()
                                .Select(x => new GolaTypeList
                                {
                                    GoalTypeId = (int)x,
                                    GoalTypeName = Enum.GetName(typeof(GoalTypeEnum_PTStateDuration), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Goal List";
                res.Status = true;
                res.Data = bloodGroup;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get GoalType

        #region This Api is Used To Add Goal

        /// <summary>
        /// created by shriya created on 02-05-2022
        /// </summary> api>>Post>> api/goal/AddGoal
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGoal")]
        public async Task<ResponseBodyModel> AddGoal(Goalsadded model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Goal goaldata = new Goal();
                goaldata.GoalName = model.GoalName;
                goaldata.Description = model.Description;
                goaldata.GoalType = model.GoalType;
                goaldata.AssignById = claims.employeeId;
                goaldata.AssignToId = model.AssignToId;
                goaldata.StartDate = model.StartDate;
                goaldata.ExpEndDate = model.GoalType == GoalTypeEnum_PTStateDuration.Monthly ? goaldata.StartDate.AddMonths(1) : (model.GoalType == GoalTypeEnum_PTStateDuration.Quarterly ? goaldata.StartDate.AddMonths(3) : (model.GoalType == GoalTypeEnum_PTStateDuration.Half_Yearly ? goaldata.StartDate.AddMonths(6) : goaldata.StartDate.AddMonths(12)));
                goaldata.CreatedBy = claims.employeeId;
                goaldata.CreatedOn = DateTime.Now;
                goaldata.Status = GoalStatusConstants.Pending;
                goaldata.IsActive = true;
                goaldata.IsDeleted = false;
                goaldata.CompanyId = claims.companyId;
                goaldata.OrgId = claims.orgId;
                _db.Goals.Add(goaldata);
                await _db.SaveChangesAsync();

                _ = SendMailCreatedGoal(goaldata);

                res.Message = "Goal added successfully";
                res.Status = true;
                res.Data = goaldata;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is Used To Add Goal

        #region This Api is Used To Get All Goal List

        /// <summary>
        /// created by shriya , create on 02-05-2022
        /// </summary> Api>>Get>> api/goal/GetAllGoalList
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllGoalList")]
        public async Task<ResponseBodyModel> GetAllGoalList()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                List<DataGoals> GoalDataList = new List<DataGoals>();
                var goaldata = await _db.Goals.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in goaldata)
                {
                    DataGoals goals = new DataGoals
                    {
                        GoalId = item.GoalId,
                        GoalName = item.GoalName,
                        GoalType = item.GoalType.ToString(),
                        StartDate = item.StartDate,
                        Status = Enum.GetName(typeof(GoalStatusConstants), item.Status),
                        Description = item.Description,
                        ExpEndDate = item.ExpEndDate,
                        GoalDocuments = item.GoalDocuments,
                        AssignToId = item.AssignToId,
                        AssignById = item.AssignById
                    };
                    goals.AssignByName = _db.Employee.Where(x => x.EmployeeId == goals.AssignById).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    goals.AssignToName = _db.Employee.Where(x => x.EmployeeId == goals.AssignToId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                    GoalDataList.Add(goals);
                }
                if (GoalDataList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "All goal list";
                    res.Data = GoalDataList.OrderByDescending(x => x.GoalId).ToList();
                }
                else
                {
                    res.Status = false;
                    res.Message = "list not found ";
                    res.Data = GoalDataList;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This Api is Used To Get All Goal List

        #region This Api Used To Get Goals By Id

        /// <summary>
        /// API >> Get >> api/goal/getgoalid
        /// Modify By Ankit on 06-05-2022
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("getgoalid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetGoalId(int goalId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Goals.FirstOrDefaultAsync(x => x.GoalId == goalId && x.IsDeleted == false &&
                        x.IsActive == true);
                if (department != null)
                {
                    res.Message = "Goals Found";
                    res.Status = true;
                    res.Data = department;
                }
                else
                {
                    res.Message = "No Goals Found!!";
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

        #endregion This Api Used To Get Goals By Id

        #region status Update in goals , updated by user whose assign to the goal

        /// <summary>
        /// Create by Shriya Create on 10-05-2022
        /// </summary> route api/goal/updategoalstatus
        [HttpPost]
        [Route("updategoalstatus")]
        public async Task<ResponseBodyModel> UpdateGoalStatusData(GoalupdateStatus model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var goaldata = await _db.Goals.Where(x => x.IsActive == true && x.IsDeleted == false && x.GoalId == model.GoalId).FirstOrDefaultAsync();
                if (goaldata != null)
                {
                    goaldata.Reason = model.Reason;
                    goaldata.Status = model.Status;
                    goaldata.GoalPercentage = model.Percentage;
                    _db.Entry(goaldata).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    _ = SendMailUpdateGoalstatus(goaldata);
                    res.Status = true;
                    res.Message = "Status updated successfully";
                    res.Data = goaldata;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Status not updated";
                    res.Data = goaldata;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion status Update in goals , updated by user whose assign to the goal

        #region This Api is Used To Reason to extend Date by Goal

        /// <summary>
        /// Create by Ankit Create on 13-05-2022
        /// </summary> route api/goal/addrextendate
        [HttpPost]
        [Route("addrextendate")]
        public async Task<ResponseBodyModel> AddRextenDate(GoalStatus model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var goaldata = await _db.Goals.Where(x => x.IsActive == true && x.IsDeleted == false && x.GoalId == model.GoalId
                && x.Status != GoalStatusConstants.ExtendRequest).FirstOrDefaultAsync();
                if (goaldata != null)
                {
                    goaldata.GoalId = model.GoalId;
                    goaldata.ReasonToExtenGoal = model.ReasonToExtenGoal;
                    goaldata.StartDate = model.Date;
                    goaldata.UpdatedBy = claims.userId;
                    goaldata.UpdatedOn = DateTime.Now;
                    _db.Entry(goaldata).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "submited successfully";
                    res.Data = goaldata;
                }
                else
                {
                    goaldata.Status = GoalStatusConstants.Pending;
                    goaldata.UpdatedBy = claims.userId;
                    goaldata.UpdatedOn = DateTime.Now;
                    _db.Entry(goaldata).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = false;
                    res.Message = "submited not successfully";
                    res.Data = goaldata;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This Api is Used To Reason to extend Date by Goal

        #region API FOR Get Goal Reason And Extend Date
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Get >> api/goal/getgoalextendreason
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("getgoalextendreason")]
        [HttpGet]
        public async Task<IHttpActionResult> GetExtendGoalReason(int goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = await _db.Goals.Where(x => x.GoalId == goalId && x.IsActive && !x.IsDeleted)
                       .Select(x => new
                       {
                           x.ReasonToExtenGoal,
                           x.StartDate
                       }).FirstOrDefaultAsync();
                    if (getData != null)
                    {
                        res.Message = "Get Extend Reason Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = " Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/getgoaldeletereason", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API FOR Change Status In Extend Goal
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/extendstatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("extendstatus")]
        [HttpPost]
        public async Task<IHttpActionResult> RejectedGoalData(ExtendGoal model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Status = GoalStatusConstants.Extended;
                        getData.StartDate = getData.StartDate;
                        getData.IsActive = false;
                        getData.IsDeleted = true;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;
                        getData.IsExtended = true;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        _ = SendMailExtendGoal(getData);
                        res.Message = "Extended Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/rejectedgoal", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class ExtendGoal
        {
            public int GoalId { get; set; }
        }
        #endregion

        #region This Api Use To Send Mail To Extend Gola 
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailExtendGoal(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Extended To Goal </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region API FOR Extend Goal
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/reasonextendgoal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("reasonextendgoal")]
        [HttpPost]
        public async Task<IHttpActionResult> ReasonExtendGoal(ExtendeddGoal model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Reason = model.Reason;
                        getData.Status = GoalStatusConstants.Pending;
                        getData.IsActive = true;
                        getData.IsDeleted = false;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        _ = SendMailExtendGoal(getData);
                        res.Message = "Extended Succesfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/rejectedgoal", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class ExtendeddGoal
        {
            public int GoalId { get; set; }
            public string Reason { get; set; }
        }
        #endregion

        #region Get All Goal Assign to you

        /// <summary>
        ///  create by shriya create on 04-05-2022
        /// tHIS API FOR WHICH GOAL IS ASSIGN TO LOGIN USER
        /// </summary>route api/goal/GetAllGoalByAssignToYou
        [HttpGet]
        [Route("GetAllGoalByAssignToYou")]
        public async Task<ResponseBodyModel> GetAllGoalByAssignToYou()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<DataGoals> GoalDataList = new List<DataGoals>();
                var goaldata = await _db.Goals.Where(x => x.AssignToId == claims.employeeId
                ).OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToListAsync();
                foreach (var item in goaldata)
                {
                    DataGoals goals = new DataGoals();
                    goals.GoalId = item.GoalId;
                    goals.GoalName = item.GoalName;
                    goals.GoalType = item.GoalType.ToString();
                    goals.StartDate = item.StartDate;
                    goals.Status = Enum.GetName(typeof(GoalStatusConstants), item.Status).ToString();
                    goals.ExpEndDate = item.ExpEndDate;
                    goals.AssignById = item.AssignById;
                    goals.AssignByName = _db.Employee.Where(x => x.EmployeeId == goals.AssignById).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    goals.AssignToId = item.AssignToId;
                    goals.IsDeleted = item.IsDeleted;
                    goals.IsActive = item.IsActive;
                    goals.IsRejected = item.IsRejected;
                    goals.IsExtended = item.IsExtended;
                    goals.AssignToName = _db.Employee.Where(x => x.EmployeeId == goals.AssignToId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    goals.Description = item.Description;
                    GoalDataList.Add(goals);
                }
                if (GoalDataList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Get all goal list assign to you ";
                    res.Data = GoalDataList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "list not found ";
                    res.Data = GoalDataList;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Goal Assign to you

        #region Get All Goal  Assign by you

        /// <summary>
        ///  create by shriya create on 04-05-2022
        /// tHIS API FOR WHICH GOAL IS ASSIGN by  LOGIN USER
        /// </summary>route api/goal/GetAllGoalByAssignByYou
        [HttpGet]
        [Route("GetAllGoalByAssignByYou")]
        public async Task<ResponseBodyModel> GetAllGoalByAssignbyYou()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<DataGoals> GoalDataList = new List<DataGoals>();
                var goaldata = await _db.Goals.Where(x => x.AssignById == claims.employeeId
                /*&& x.CompanyId == claims.companyId && x.OrgId == claims.orgId*/).ToListAsync();
                foreach (var item in goaldata)
                {
                    DataGoals goals = new DataGoals();
                    goals.GoalId = item.GoalId;
                    goals.GoalName = item.GoalName;
                    goals.GoalType = item.GoalType.ToString();
                    goals.StartDate = item.StartDate;
                    goals.Reason = item.Reason;
                    goals.ReasonToExtenGoal = item.ReasonToExtenGoal;
                    goals.ExtendeDate = item.StartDate;
                    goals.Status = Enum.GetName(typeof(GoalStatusConstants), item.Status).ToString();
                    goals.ExpEndDate = item.ExpEndDate;
                    goals.AssignToId = item.AssignToId;
                    goals.AssignToName = _db.Employee.Where(x => x.EmployeeId == goals.AssignToId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    goals.AssignById = item.AssignById;
                    goals.GoalDocuments = item.GoalDocuments;
                    goals.IsActive = item.IsActive;
                    goals.IsDeleted = item.IsDeleted;
                    goals.IsRejected = item.IsRejected;
                    goals.IsExtended = item.IsExtended;
                    goals.Percentage = item.GoalPercentage;
                    goals.AssignByName = _db.Employee.Where(x => x.EmployeeId == goals.AssignById).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    goals.Description = item.Description;
                    GoalDataList.Add(goals);
                }
                if (GoalDataList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Get all goal list assign By you ";
                    res.Data = GoalDataList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "list not found ";
                    res.Data = GoalDataList;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Goal  Assign by you

        #region This Api used to Update Goals

        /// <summary>
        /// Modify By Ankit On 06-05-2022
        /// API >> Update >> api/goal/editgoal
        /// </summary>
        /// <param name="updategoal"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editgoal")]
        public async Task<ResponseBodyModel> UpdateOrganaization(GoalsList model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updategoalData = await _db.Goals.Where(x => x.GoalId == model.GoalId &&
                         x.IsDeleted == false /*&& x.CompanyId == claims.companyId && x.OrgId == claims.orgId*/).FirstOrDefaultAsync();
                if (updategoalData != null)
                {
                    updategoalData.GoalId = model.GoalId;
                    updategoalData.GoalName = model.GoalName;
                    updategoalData.GoalType = model.GoalType;
                    updategoalData.AssignToId = model.AssignToId;
                    updategoalData.Description = model.Description;
                    updategoalData.StartDate = model.StartDate;
                    updategoalData.Status = GoalStatusConstants.Pending;
                    updategoalData.ExpEndDate = model.GoalType == GoalTypeEnum_PTStateDuration.Monthly ? updategoalData.StartDate.AddMonths(1) : (model.GoalType == GoalTypeEnum_PTStateDuration.Quarterly ? updategoalData.StartDate.AddMonths(3) : (model.GoalType == GoalTypeEnum_PTStateDuration.Half_Yearly ? updategoalData.StartDate.AddMonths(6) : updategoalData.StartDate.AddMonths(12)));
                    updategoalData.UpdatedBy = claims.employeeId;
                    updategoalData.UpdatedOn = DateTime.Now;
                    _db.Entry(updategoalData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Goals Updated Successfully!";
                    res.Data = updategoalData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Goals not updated";
                    res.Data = updategoalData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api used to Update Goals

        #region This Api Used to Delete Goals

        /// <summary>
        /// Modify By Ankit 06-05-2022
        /// API >> Put >> api/api/goal/deletegoals
        /// </summary>
        /// <param name="GolasId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletegoals")]
        public async Task<ResponseBodyModel> DeleteGolas(int goalId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var GoalData = await _db.Goals.Where(x => x.GoalId == goalId && x.IsActive && !x.IsDeleted)
                        .FirstOrDefaultAsync();
                if (GoalData != null)
                {
                    GoalData.EndDate = GoalData.EndDate;
                    GoalData.IsDeleted = true;
                    GoalData.IsActive = false;
                    GoalData.DeletedOn = DateTime.Now;

                    _db.Entry(GoalData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Goals Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Goals Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Used to Delete Goals

        #region This Api is used to Upload Goal Document

        /// <summary>
        /// Created By Ankit 11/05/2022
        /// </summary>Api>>Post>> api/goal/uploaddocument
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("uploaddocument")]
        public async Task<ResponseBodyModel> UploadDocument(string DocUrl, int goalId, int percentage)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var GoalData = await _db.Goals.Where(x => !x.IsDeleted && x.IsActive
                && x.GoalId == goalId
                ).FirstOrDefaultAsync();
                if (GoalData != null)
                {
                    if (DocUrl == null)
                    {
                        res.Message = "Please Upload Document";
                        res.Status = false;
                    }
                    else
                    {
                        GoalData.GoalDocuments = DocUrl;
                        GoalData.GoalPercentage = percentage;
                        GoalData.UpdatedOn = DateTime.Now;
                        _db.Entry(GoalData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        _ = SendMailUploadDocument(GoalData);
                        res.Status = true;
                        res.Message = "Document Added Successfully";
                        res.Data = GoalData;
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Document not Updated.";
                    res.Data = GoalData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload Goal Document

        #region This api use for upload documents for Goal document

        /// <summary>
        ///Created By Ankit On 18-05-2022
        /// </summary>Api>>Post>> api/goal/uploadGoaldocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadGoaldocuments")]
        public async Task<UploadImageResponseGoal> UploadGoalDocments()
        {
            UploadImageResponseGoal result = new UploadImageResponseGoal();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/goaldocuments/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\goaldocuments\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successfuly";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path;
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }

        #endregion This api use for upload documents for Goal document

        #region API FOR UPDATE Goal Reason
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/updategoalreason
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updategoalreason")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGoalReason(UpdateGoalRequset model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId
                    && x.Status != GoalStatusConstants.DeleteRequest).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Status = GoalStatusConstants.DeleteRequest;
                        getData.Reason = model.Reason;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;

                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        _ = SendMailGoalReason(getData);
                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        getData.Status = GoalStatusConstants.Pending;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = "Already Delete Request Rejected !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/updategoalreason", ex.Message, model);
                return BadRequest("Failed");
            }
        }

        public class UpdateGoalRequset
        {
            public int GoalId { get; set; }
            public string Reason { get; set; }
        }
        #endregion

        #region API FOR UPDATE Goal Reject
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/updategoalreject
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updategoalreject")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGoalRejectData(UpdateGoalReject model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Status = GoalStatusConstants.Reject;
                        getData.Reason = model.Reason;
                        getData.GoalPercentage = 0;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;

                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        _ = SendMailGoalRejected(getData);
                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Reason Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/updategoalreject", ex.Message, model);
                return BadRequest("Failed");
            }
        }

        public class UpdateGoalReject
        {
            public int GoalId { get; set; }
            public string Reason { get; set; }
        }
        #endregion

        #region API FOR UPDATE Status
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/updategoalstatusapproved
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updategoalstatusapproved")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGoalStatusapproved(UpdateGoalStatus model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Status = GoalStatusConstants.Approved;
                        getData.GoalPercentage = model.Percentage;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;

                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        SendMailGoal(getData.GoalId);
                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Reason Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/updategoalstatusapproved", ex.Message, model);
                return BadRequest("Failed");
            }
        }

        public class UpdateGoalStatus
        {
            public int GoalId { get; set; }
            public int Percentage { get; set; }
        }
        #endregion

        #region This Api Use To Send Mail To Gola Updated Reason
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailGoalReason(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Reason To Goal Updated </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use To Send Mail To Gola Rejected
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailGoalRejected(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Rejected Goal </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "<br />";
                body += employeeData.Status;
                body += "<br />";
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use To Send Mail To Upload Document
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailUploadDocument(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Upload Document </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use To Send Mail To Update Goal Status
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailUpdateGoalstatus(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>A Goal Accepted </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "<br />";
                body += employeeData.Status;
                body += "<br />";
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use To Send Mail To Created Goal 
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailCreatedGoal(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FFFFFF; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Created A Goals </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "<br />";
                body += employeeData.Description;
                body += "<br />";
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];
                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use To Send Mail To Goal Approved
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailGoal(int goalId)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Goals Approved</h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Approved By<p>";
                body += createEmployee.DisplayName;
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];
                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;

                oMail.TextBody = "Find your Task";
                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region API FOR Get Goal Document
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Get >> api/goal/getgoaldocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("getgoaldocument")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGoalDocumentData(int goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = await _db.Goals.Where(x => x.GoalId == goalId && x.IsActive && !x.IsDeleted)
                       .Select(x => new
                       {
                           x.GoalDocuments
                       }).FirstOrDefaultAsync();
                    if (getData != null)
                    {
                        res.Message = "Get Document Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = " Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/getgoaldocument", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API FOR Get Goal Reason To Delete
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Get >> api/goal/getgoaldeletereason
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("getgoaldeletereason")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGoalDeleteReason(int goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = await _db.Goals.Where(x => x.GoalId == goalId && x.IsActive && !x.IsDeleted)
                       .Select(x => new
                       {
                           x.Reason
                       }).FirstOrDefaultAsync();
                    if (getData != null)
                    {
                        res.Message = "Get Reason Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = " Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/getgoaldeletereason", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API FOR Remove Goal
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Delete >> api/goal/removegoalreason
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("removegoalreason")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveGoalReason(RemoveGoalReject model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Status = GoalStatusConstants.Deleted;
                        getData.IsActive = false;
                        getData.IsDeleted = true;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;

                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        _ = SendMailRemoveGoalReason(getData.GoalId);
                        res.Message = "Remove Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Reason Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/removegoalreason", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class RemoveGoalReject
        {
            public int GoalId { get; set; }
        }
        #endregion

        #region This Api Use To Send Mail To Remove Gola Reason
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        [Route("getdatatorejected")]
        public bool SendMailRemoveGoalReason(int goalId)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");


                var employeeData = _db.Goals.Where(x => x.GoalId == goalId).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Remove Goal </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Deleted By <p>";
                body += createEmployee.DisplayName;
                body += "<br />";
                body += employeeData.Status;
                body += "<br />";
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region API FOR Rejectd Goal
        /// <summary>
        /// Created By ankit Jain On 01-01-2023
        /// API >> Post >> api/goal/rejectedgoal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("rejectedgoal")]
        [HttpPost]
        public async Task<IHttpActionResult> RejectedGoalData(RejectedGoal model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                else
                {
                    var getData = _db.Goals.Where(x => x.GoalId == model.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.Reason = model.Reason;
                        getData.Status = GoalStatusConstants.Pending;
                        getData.IsActive = true;
                        getData.IsDeleted = false;
                        getData.UpdatedOn = DateTime.Now;
                        getData.UpdatedBy = tokenData.employeeId;
                        getData.IsRejected = true;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        _ = SendMailRejectedGoal(getData);
                        res.Message = "Rejected Successfully  !";
                        res.Status = true;
                        res.Data = getData;
                        return Ok(res);

                    }
                    else
                    {
                        res.Message = "Reason Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("api/goal/rejectedgoal", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class RejectedGoal
        {
            public int GoalId { get; set; }
            public string Reason { get; set; }
        }
        #endregion

        #region This Api Use To Send Mail To Rejected Gola 
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public bool SendMailRejectedGoal(Goal goaldata)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                //var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Goals.Where(x => x.GoalId == goaldata.GoalId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignById && x.IsActive && !x.IsDeleted)
                    .FirstOrDefault();

                var requestEmployee = _db.Employee.Where(x => x.EmployeeId == employeeData.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                // Set email subject
                oMail.Subject = "Reminder";

                // Set email body
                string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                body += "<h3 style='background-color: rgb(241, 89, 34);'>Rejected To Goal </h3>";
                body += "Dear " + requestEmployee.DisplayName + ',';
                body += "<p>A Goal Has Been Created For You By<p>";
                body += createEmployee.DisplayName;
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                oMail.HtmlBody = body;
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                obj.Add(requestEmployee.OfficeEmail);
                oMail.To = obj;
                // Set email body
                oMail.TextBody = "Find your Task";

                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];

                // use 587 TLS port
                oServer.Port = 587;
                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// </summary>
        public class GolaTypeList
        {
            public int GoalTypeId { get; set; }
            public string GoalTypeName { get; set; }
        }

        ///<summary>
        ///craeted by shriya create on 10-05-2022
        ///</summary>
        public class GoalupdateStatus
        {
            public int GoalId { get; set; }
            public GoalStatusConstants Status { get; set; }
            public string Reason { get; set; }
            public int Percentage { get; set; }
        }

        public class GoalsList
        {
            public int GoalId { get; set; }
            public string GoalName { get; set; }
            public GoalTypeEnum_PTStateDuration GoalType { get; set; }
            public string Description { get; set; }
            public int AssignToId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        /// <summary>
        /// shriya
        /// </summary>
        public class DataGoals
        {
            public int GoalId { get; set; }
            public string GoalName { get; set; }
            public string GoalType { get; set; }
            public string Description { get; set; }
            public int AssignToId { get; set; }
            public string AssignToName { get; set; }
            public string Status { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime ExpEndDate { get; set; }
            public int AssignById { get; set; }
            public string AssignByName { get; set; }
            public string GoalDocuments { get; set; }
            public string Reason { get; set; }
            public string ReasonToExtenGoal { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsActive { get; set; }
            public DateTime? ExtendeDate { get; set; }
            public bool IsRejected { get; set; }
            public bool IsExtended { get; set; }
            public int Percentage { get; set; }
        }

        public class GoalsData
        {
            public string FullName { get; set; }
            public int GoalsID { get; set; }
            public List<int> EmployeeId { get; set; }
            public int AssignByID { get; set; }
            public string Goal { get; set; }
            public string GoalName { get; set; }
            public int GoalTypeID { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
            public string Status { get; set; }
            public string GoalDocuments { get; set; }
        }

        public class Goalsadded
        {
            public string GoalName { get; set; }
            public GoalTypeEnum_PTStateDuration GoalType { get; set; }
            public string Description { get; set; }
            public int AssignToId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime ExpEndDate { get; set; }
        }

        /// <summary>
        /// Created By Ankit
        /// </summary>
        ///
        public class GoalStatus
        {
            public int GoalId { get; set; }
            public string ReasonToExtenGoal { get; set; }
            public DateTime Date { get; set; }
        }

        /// <summary>
        /// Created By Ankit
        /// </summary>

        public class UploadImageResponseGoal
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        #endregion Helper Model Class
    }
}