using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;
using TextInfo = System.Globalization.TextInfo;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/contact")]
    public class ContactController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Candidate

        [Route("AddCandidate")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddCandidate(Candidate createCandidate)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    Candidate candidateData = new Candidate();
                    candidateData.CandidateName = createCandidate.CandidateName;
                    candidateData.CurrentDesignation = createCandidate.CurrentDesignation;
                    candidateData.Experience = createCandidate.Experience;
                    candidateData.RelevantExperience = createCandidate.RelevantExperience;
                    candidateData.CurrentJobLocation = createCandidate.CurrentJobLocation;
                    candidateData.Qualifications = createCandidate.Qualifications;
                    candidateData.PositionAppliedFor = createCandidate.PositionAppliedFor;
                    candidateData.MobileNumber = createCandidate.MobileNumber;
                    candidateData.Email = createCandidate.Email;
                    candidateData.HomeTown = createCandidate.HomeTown;
                    candidateData.TechnologyKnown = createCandidate.TechnologyKnown;
                    candidateData.ExpectedCTC = createCandidate.ExpectedCTC;
                    candidateData.CurrentCTC = createCandidate.CurrentCTC;
                    candidateData.CreatedOn = DateTime.Now;
                    candidateData.Status = 28;
                    candidateData.RoleID = createCandidate.RoleID;
                    candidateData.SecondaryContact = createCandidate.SecondaryContact;
                    candidateData.NoticePeriod = createCandidate.NoticePeriod;
                    candidateData.InterViewType = createCandidate.InterViewType;
                    //candidateData.CreatedOn = DateTime.Now;
                    //candidateData.CreatedBy = claims.employeeid;
                    candidateData.Availabilitys = createCandidate.Availabilitys;
                    candidateData.CompanyId = claims.companyId;

                    //var str = createCandidate.uploadResume.Split('\').Last();

                    string[] testfiles = createCandidate.UploadResume.Split(new char[] { '\\' });
                    var finalString = testfiles.LastOrDefault();

                    candidateData.UploadResume = finalString;

                    _db.Candidates.Add(candidateData);
                    _db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Candidate Created Successfully";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("UploadResumeFile")]
        [Authorize]
        public IHttpActionResult UploadResumeFile()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                string fname = "";
                var Request = HttpContext.Current.Request;
                if (Request.Files.Count > 0)
                {
                    HttpFileCollection files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];

                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.
                        fname = Path.Combine(HttpContext.Current.Server.MapPath("~/Resume/"), fname);
                        file.SaveAs(fname);

                        var name = file.FileName.ToString();
                        response.Message = "Resume Uploaded successfully";
                        response.StatusReason = true;
                    }
                }
                else
                {
                    response.Message = "No files selected";
                    response.StatusReason = false;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateAdministratermail")]
        [Authorize]
        public IHttpActionResult CreateAdministratermail(Administrater Administrater)// we are using contact table as a employee
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                Administrater AdministraterData = new Administrater();
                {
                    AdministraterData = new Administrater();
                    AdministraterData.HR = Administrater.HR;
                    AdministraterData.Employee = Administrater.Employee;
                }
                var hr = _db.Employee.Where(e => e.EmployeeId == Administrater.HR).FirstOrDefault();
                var hrManager = _db.Employee.Where(e => e.RoleId == 120).FirstOrDefault();
                var EmployeeData = _db.Employee.Where(e => e.EmployeeId == Administrater.Employee).FirstOrDefault();

                //string emps = "";
                //foreach (var emp in Administrater.Employee)
                //{
                //    var data = db.Employee.Where(x => x.EmployeeId == emp).FirstOrDefault();
                //    emps+=data.FirstName+" "+data.LastName+", ";
                //}
                //emps=emps.Trim(' ');
                //emps = emps.TrimEnd(',');
                ////var EmployeeData = db.Employee.Where(x => x.EmployeeId == 2011).FirstOrDefault();
                //foreach(var item in hrManager)
                //{
                //}
                if (hrManager.OfficeEmail != null)
                {
                    UserEmailDTOResponse responsemail = new UserEmailDTOResponse();
                    {
                        UserEmail mailmodel = new UserEmail();
                        mailmodel.To = hrManager.OfficeEmail;
                        //mailmodel.To = "sumit@moreyeahs.co";
                        //mailmodel.MailPassword = "";
                        mailmodel.Subject = "Request for access"; //add subject here
                        mailmodel.Body = "Hello " + hrManager.FirstName + " " + hrManager.LastName + ",<br><br>" +
                      "Please give access to <b>" + hr.FirstName + " " + hr.LastName + "</b>. Request for the access, to view Bank Details of <b>" + EmployeeData.FirstName + " " + EmployeeData.LastName + " " + ".<br>" +
                           "<br>Thanks <br>" +
                             "Team MoreYeahs";
                        var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                        response.Message = "Mail Sent Sucessfully";
                        response.StatusReason = true;
                    }
                }
                else
                {
                    response.Message = "Something went wrong";
                    response.StatusReason = false;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateCandidateStatus")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateCandidateStatus(Candidate update)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var getData = _db.Candidates.Where(x => x.CandidateId == update.CandidateId && x.IsDeleted == false).FirstOrDefault();
                if (getData != null)
                {
                    getData.Status = update.Status;
                    _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Record Updated Successfully";
                }

                if (update.Status == 31)
                {
                    UserEmail mailmodel = new UserEmail();
                    mailmodel.To = getData.Email;
                    //mailmodel.FromMail = "";
                    //mailmodel.MailPassword = "";
                    mailmodel.Subject = "Rejectedmail"; //add subject here
                    mailmodel.Body = "Dear " + "<br><br>" + getData.CandidateName + " " + "</br>" + ",<br><br> " +

                   "Thank you very much for taking time to interview with us for the position of  <b>" + getData.PositionAppliedFor + "</b> " + " " + ".It was a pleasure to learn more about your skills and accomplishments.We appreciate your interest in the company and the job.<br>" +
                    "Unfortunately ,for now we are not taking you for further consideration.<br>" +
                     "We will keep your application on file for consideration if there is a future opening that may be a perfect fit for you. <br>" +
                      "We do encourage you to apply for other openings at MoreYeahs in the future.<br><br> " +
                      "Thanks <br>" +
                         "Team MoreYeahs";

                    var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateinterviewRound")]
        [Authorize]
        public IHttpActionResult CreateInterviewRound(InterviewRound InterviewRound)// we are using contact table as a employee
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();

                List<InterviewRound> lstInterviewModel = new List<InterviewRound>();
                InterviewRound InterviewRoundData = new InterviewRound();

                foreach (var InterviewerId in InterviewRound.InterviewerId)
                {
                    //var InterviewerFirstName = db.Employee.Where(x => x.EmployeeId == item).FirstOrDefault().FirstName;
                    //var InterviewerLastName = db.Employee.Where(x => x.EmployeeId ==item).FirstOrDefault().LastName;

                    var empData = _db.Employee.Where(x => x.EmployeeId == InterviewerId).ToList();

                    InterviewRoundData = new InterviewRound();

                    InterviewRoundData.InterviewerMail = empData[0].OfficeEmail;
                    InterviewRoundData.InterviewerName = empData[0].FirstName + " " + empData[0].LastName;
                    InterviewRoundData.StartTime = InterviewRound.StartTime;
                    InterviewRoundData.EndTime = InterviewRound.EndTime;
                    InterviewRoundData.InterviewDate = InterviewRound.InterviewDate;
                    InterviewRoundData.InterviewRoundName = InterviewRound.InterviewRoundName;
                    InterviewRoundData.CandidateId = InterviewRound.CandidateId;
                    InterviewRoundData.InterviewTitle = InterviewRound.InterviewTitle;
                    InterviewRoundData.InterviewLink = InterviewRound.InterviewLink;
                    lstInterviewModel.Add(InterviewRoundData);
                }

                string nameList = "";
                string emailList = "";

                foreach (var item in lstInterviewModel)
                {
                    nameList += item.InterviewerName + ", ";
                    emailList += item.InterviewerMail + ",";
                }

                nameList = nameList.Trim(' ');
                nameList = nameList.TrimEnd(',');
                emailList = emailList.TrimEnd(',');

                InterviewRoundData.InterviewerName = nameList;
                InterviewRoundData.InterviewerMail = emailList;
                InterviewRoundData.StartTime = InterviewRound.StartTime;
                InterviewRoundData.EndTime = InterviewRound.EndTime;
                InterviewRoundData.InterviewDate = InterviewRound.InterviewDate;
                InterviewRoundData.CandidateMail = InterviewRound.CandidateMail;
                InterviewRoundData.CandidateId = InterviewRound.CandidateId;
                InterviewRoundData.InterviewRoundName = InterviewRound.InterviewRoundName;
                InterviewRoundData.InterviewLink = InterviewRound.InterviewLink;
                InterviewRoundData.InterviewTitle = InterviewRound.InterviewTitle;
                InterviewRoundData.HR = InterviewRound.HR;
                InterviewRoundData.Status = "Pending";
                InterviewRoundData.Description = InterviewRound.Description;

                _db.InterviewRound.Add(InterviewRoundData);
                _db.SaveChanges();

                var getdata = _db.Candidates.Where(e => e.CandidateId == InterviewRound.CandidateId).FirstOrDefault();
                var hr = _db.Employee.Where(e => e.EmployeeId == InterviewRound.HR).FirstOrDefault();

                response.StatusReason = true;
                response.Message = "Interview Schedule Successfully";

                UserEmailDTOResponse responsemail = new UserEmailDTOResponse();
                foreach (var item in lstInterviewModel)
                {
                    UserEmail mailmodel = new UserEmail();
                    string Mail = "";
                    string Name = "";
                    if (item.InterviewerMail.Contains(","))
                    {
                        Mail = item.InterviewerMail.Split(',').LastOrDefault();
                        Name = item.InterviewerName.Split(',').LastOrDefault();
                    }
                    else
                    {
                        Mail = item.InterviewerMail;
                        Name = item.InterviewerName;
                    }
                    Name.Trim();
                    Name.TrimEnd(new char[] { ',' });
#pragma warning disable CS0168 // The variable 'dt' is declared but never used
                    DateTime dt;
#pragma warning restore CS0168 // The variable 'dt' is declared but never used
                    DateTime dt2 = item.InterviewDate ?? DateTime.MinValue;

                    //string fname = Path.Combine(HttpContext.Current.Server.MapPath("~/Resume/"),getdata.uploadResume );
                    //mailmodel.filepath = fname;

                    string interviewDate = dt2.ToString("dd-MMM-yyyy");

                    // mailmodel.To = Mail != null ? Mail : "test@moreyeahs.com";
                    mailmodel.To = Mail != null ? Mail : "test@moreyeahs.com";
                    //mailmodel.FromMail = "";
                    //mailmodel.MailPassword = "";
                    mailmodel.Subject = item.InterviewTitle; //add subject here
                    if (getdata.UploadResume != null)
                    {
                        mailmodel.filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Resume/"), getdata.UploadResume);
                    }
                    else
                    {
                        mailmodel.filepath = null;
                    }

                    mailmodel.Body = "Hello " + Name + ",</b>" + "<br><br> " +
                        "Please be informed that some <b>" + item.InterviewRoundName + "</b> " + " " + ", interview has been scheduled for <b>" + interviewDate + "</b> " + " " + ", for <b>" + getdata.PositionAppliedFor + "</b> " +
                        " and the interview panel consists of <b>" + nameList + "</b>.<br>" +
                       "You are requested to take the interview & kindly be available for the same. <br>" +
                        item.Description + "<br>" +
                        "Please find the details of the candidate and please find attached CV below: <br><br>" +
                        "<b>Candidate name:-</b> " + getdata.CandidateName + "<br> " +
                        "<b>Contact no. :-</b> " + getdata.MobileNumber + "<br> " +
                        "<b>Interview Date:-</b> " + interviewDate + "<br>" +
                        "<b>Interview Time:-</b> " + item.StartTime + " to " + item.EndTime + " <br>" +
                        "<b>Interview Link:-</b> " + "<a href='" + item.InterviewLink + "'>" + item.InterviewLink + "</a>" + "<br><br> " +
                         "Thanks & Regards <br>" +
                         "Team MoreYeahs";

                    // "<b>Interview Round Name:-</b> " + item.InterviewRoundName + "<br><br> " +
                    // "<b>Interview Date:-</b> " + item.InterviewDate + "<br><br> " +
                    //"<b>Interview Time:-</b> " + item.StartTime + "<br><br> " +
                    // "<b>HR:-</b> " + hr.FirstName+" "+hr.LastName + "<br><br> " +
                    // "<b>Description:-</b> " + item.Description + "<br><br> " +
                    //"<b>Status:-</b> " + item.Status + "<br><br> " +

                    //"<b>This task assign to you</b><br>" +

                    // var emailresponse = UserEmailHelper.SendEmail(mailmodel);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateInterviewRound")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateInterviewRound(InterviewRound data)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();

                var getData = _db.InterviewRound.Where(x => x.InterviewId == data.InterviewId).FirstOrDefault();
                if (getData != null)
                {
                    getData.Status = data.Status;
                    _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Record Updated Successfully";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetInterviewActivity")]
        [Authorize]
        public IHttpActionResult GetInterviewActivity(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<InterviewData> list = new List<InterviewData>();
                Base response = new Base();
                var getActivity = (from ad in _db.InterviewRound
                                   join cd in _db.Candidates on ad.CandidateId equals cd.CandidateId
                                   join ed in _db.Employee on ad.HR equals ed.EmployeeId
                                   where ad.CandidateId == Id
                                   orderby ad.InterviewId descending
                                   select new
                                   {
                                       ad.InterviewId,
                                       ad.InterviewDate,
                                       ad.StartTime,
                                       ad.EndTime,
                                       ad.InterviewerMail,
                                       ad.InterviewerName,
                                       ad.CandidateMail,
                                       ad.CandidateId,
                                       cd.CandidateName,
                                       ad.InterviewRoundName,
                                       ad.InterviewLink,
                                       hr = ed.FirstName + " " + ed.LastName,

                                       ad.Description
                                   }).ToList();
                foreach (var item in getActivity)
                {
                    InterviewData data = new InterviewData();
                    data.InterviewId = item.InterviewId;
                    data.InterviewDate = item.InterviewDate;
                    data.StartTime = item.StartTime;
                    data.EndTime = item.EndTime;
                    data.InterviewerMail = item.InterviewerMail;
                    data.InterviewerName = item.InterviewerName;
                    data.CandidateMail = item.CandidateMail;
                    data.CandidateId = item.CandidateId;
                    data.CandidateName = item.CandidateName;
                    data.InterviewRoundName = item.InterviewRoundName;
                    data.InterviewLink = item.InterviewLink;
                    data.HRName = item.hr;
                    data.Description = item.Description;
                    list.Add(data);
                }
                if (list.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Data Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Data Not Found";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Route("AddTechnology")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddTechnology(Technology createTechnology)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    Technology TechnologyData = new Technology();

                    TechnologyData.TechnologyType = createTechnology.TechnologyType;

                    TechnologyData.IsActive = true;

                    TechnologyData.IsDeleted = false;

                    _db.Technology.Add(TechnologyData);
                    _db.SaveChanges();

                    response.StatusReason = true;

                    response.Message = "Technology Created Successfully";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteCandidate")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteCandidate(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                if (Id != 0)
                {
                    var candidateDelete = _db.Candidates.Where(x => x.CandidateId == Id).FirstOrDefault();

                    candidateDelete.IsDeleted = true;
                    candidateDelete.IsActive = false;
                    _db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Record Delete Successfully";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Please select the record";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateCandidate")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateCandidate(Candidate updateCandidate)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var candidateData = (from ad in _db.Candidates where ad.CandidateId == updateCandidate.CandidateId select ad).FirstOrDefault();
                candidateData.CandidateName = updateCandidate.CandidateName;
                candidateData.CurrentDesignation = updateCandidate.CurrentDesignation;
                candidateData.Experience = updateCandidate.Experience;
                candidateData.RelevantExperience = updateCandidate.RelevantExperience;
                candidateData.CurrentJobLocation = updateCandidate.CurrentJobLocation;
                candidateData.Qualifications = updateCandidate.Qualifications;
                candidateData.PositionAppliedFor = updateCandidate.PositionAppliedFor;
                candidateData.MobileNumber = updateCandidate.MobileNumber;
                candidateData.Email = updateCandidate.Email;
                candidateData.HomeTown = updateCandidate.HomeTown;
                candidateData.TechnologyKnown = updateCandidate.TechnologyKnown;
                candidateData.ExpectedCTC = updateCandidate.ExpectedCTC;
                candidateData.CurrentCTC = updateCandidate.CurrentCTC;
                candidateData.CreatedOn = DateTime.Now;
                candidateData.Status = updateCandidate.Status;
                candidateData.RoleID = updateCandidate.RoleID;
                candidateData.SecondaryContact = updateCandidate.SecondaryContact;
                candidateData.NoticePeriod = updateCandidate.NoticePeriod;
                candidateData.UploadResume = updateCandidate.UploadResume;

                _db.SaveChanges();

                response.StatusReason = true;
                response.Message = "Candidate Updated Successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Candidate

        #region Team

        [Route("AddTeamMember")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddTeamMember(Team createTeamMember)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var i = 0;
                var j = 0;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    foreach (var item in createTeamMember.EmployeeId)
                    {
                        var checkMember = (from ad in _db.Team where ad.TeamMemberId == item && ad.ProjectId == createTeamMember.ProjectId select ad).FirstOrDefault();
                        if (checkMember == null)
                        {
                            Team createTeam = new Team();
                            createTeam.ProjectId = createTeamMember.ProjectId;
                            createTeam.EndDate = createTeamMember.EndDate;
                            createTeam.StartDate = createTeamMember.StartDate;
                            createTeam.UitilizationPercentage = createTeamMember.UitilizationPercentage;
                            createTeam.TeamMemberId = item;
                            createTeam.IsActive = true;
                            createTeam.IsDelete = false;
                            _db.Team.Add(createTeam);
                            _db.SaveChanges();
                            i++;

                            var EmployeeData = _db.Employee.Where(x => x.EmployeeId == item).FirstOrDefault();
                            var project = _db.Project.Where(x => x.ProjectId == createTeamMember.ProjectId).FirstOrDefault();
                            var senderData = _db.Employee.Where(x => x.EmployeeId == project.EmployeeId).FirstOrDefault();

                            UserEmailDTOResponse responseMail = new UserEmailDTOResponse();
                            if (EmployeeData.PersonalEmail != null)
                            {
                                UserEmail MailModel = new UserEmail();
                                MailModel.To = "sumit@moreyeahs.co";
                                //MailModel.FromMail = "";
                                //MailModel.MailPassword = "";
                                MailModel.Subject = "Project Detail"; //add subject here
                                MailModel.Body = "Hello All <br><br>" +
                                    "<b>We are create a new project in our portal.</b><br>" +
                                    "There are the details of the project<br>" +
                                    "<b>Project Name</b> " + "<b>" + project.ProjectName + "</b>" + " <br>" +
                                    "For further information click on the given link" +
                                    " https://uatmoreyeahsportal.moreyeahs.in/ ";

                                var EmailResponse = UserEmailHelper.SendEmail(MailModel);
                            }
                            else
                            {
                                responseMail.Message = "Email Doesn't exist";
                                responseMail.Success = false;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                    if (createTeamMember.EmployeeId.Count == j)
                    {
                        response.StatusReason = false;
                        response.Message = "Record Already Exists";
                    }
                    else
                    {
                        response.StatusReason = true;
                        response.Message = "Record Added Successfully";
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("RemoveTeamMember")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult RemoveTeamMember(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                if (Id != 0)
                {
                    var teamMemberDelete = _db.Team.Where(x => x.TeamId == Id).FirstOrDefault();

                    teamMemberDelete.IsDelete = true;
                    teamMemberDelete.IsActive = false;
                    _db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Record Delete Successfully";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Please select the record";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ActiveInActiveMember")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult ActiveInActiveMember(Team updateTeam)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var teamMemberUpdate = _db.Team.Where(x => x.TeamId == updateTeam.TeamId).FirstOrDefault();
                if (updateTeam.IsDelete == true)
                {
                    teamMemberUpdate.IsDelete = true;
                    teamMemberUpdate.IsActive = false;
                    _db.SaveChanges();
                }
                else
                {
                    teamMemberUpdate.IsDelete = false;
                    teamMemberUpdate.IsActive = true;
                    _db.SaveChanges();
                }

                response.StatusReason = true;
                response.Message = "Record Update Successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTeamMember")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetTeamMember(int projectid)
        {
            try
            {
                Base response = new Base();
                var teamData = _db.Team.Where(x => x.ProjectId == projectid).ToList();
                if (teamData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Team

        #region Address

        [Route("GetCountry")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetCountry()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var countryData = _db.Country.Where(x => x.CountryId >= 0).ToList();
                if (countryData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetStateByCountry")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetStateByCountry(string countryName)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var countryData = (from ad in _db.Country where ad.CountryName == countryName select ad).FirstOrDefault();
                var countryId = countryData.CountryId;
                var stateData = _db.State.Where(x => x.CountryId == countryId).ToList();
                if (stateData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetState")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetState()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var stateData = _db.State.Where(x => x.CountryId == 101).ToList();
                if (stateData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetCity")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetCity(string stateName)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var stateData = (from ad in _db.State where ad.StateName == stateName select ad).FirstOrDefault();
                var stateId = stateData.StateId;
                var cityData = _db.City.Where(x => x.StateId == stateId).ToList();
                if (cityData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Address

        #region Classes

        public class InterviewData
        {
            public int InterviewId { get; set; }
            public string StartTime { get; set; }
            public Nullable<System.DateTime> InterviewDate { get; set; }
            public string InterviewerMail { get; set; }
            public List<int> InterviewerId { get; set; }
            public string InterviewerName { get; set; }
            public string CandidateMail { get; set; }
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string InterviewRoundName { get; set; }
            public int HR { get; set; }
            public string HRName { get; set; }
            public string InterviewLink { get; set; }
            public string Status { get; set; }
            public string EndTime { get; set; }
            public string Description { get; set; }
        }

        public class Administrater
        {
            public int HR { get; set; }
            public int Employee { get; set; }
        }

        public class CandidateWebRoleData
        {
            public int CandidateWebRoleId { get; set; }
            public string CandidateWebRoleType { get; set; }
            public int Count { get; set; }
        }

        public class CandidateData
        {
            public string UploadResume { get; set; }
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string Email { get; set; }
            public string MobileNumber { get; set; }
            public string HomeTown { get; set; }
            public string CurrentJobLocation { get; set; }
            public string PositionAppliedFor { get; set; }
            public string CurrentDesignation { get; set; }
            public string Experience { get; set; }
            public string RelevantExperience { get; set; }
            public string Qualifications { get; set; }
            public string CurrentCTC { get; set; }
            public string ExpectedCTC { get; set; }
            public string TechnologyKnown { get; set; }
            public string CandidateWebRoleType { get; set; }
            public string StatusVal { get; set; }
            public int RoleID { get; set; }
            public int Status { get; set; }
            public string NoticePeriod { get; set; }
            public string InterViewType { get; set; }
        }

        #endregion Classes

        #region Api For Add Candidate On Job

        /// <summary>
        /// API >> Post >> api/contact/candidateadd
        /// Created By Harshit Mitra on 02-02-2022
        /// Modify By Ankit Jain on 22-03-2023
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("candidateadd")]
        public async Task<ResponseBodyModel> AddJobCandidate(AddEditCandidateModel model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddCandidateModelResponse objResponse = new AddCandidateModelResponse();
            List<Education> list = new List<Education>();
            try
            {
                if (model != null)
                {
                    var stage = _db.HiringStages.Include("Job").FirstOrDefault(x => x.Job.JobPostId
                               == model.JobId && x.StageType == StageFlowType.Sourced);
                    if (stage != null)
                    {
                        var candidateData = _db.Candidates.FirstOrDefault(x => (x.Email == model.Email
                                          || x.MobileNumber == model.MobileNumber) && x.JobId ==
                                            model.JobId && x.IsActive && !x.IsDeleted);
                        if (candidateData == null)
                        {
                            Candidate obj = new Candidate();
                            obj.JobId = model.JobId;
                            obj.CandidateName = model.CandidateName;
                            obj.Email = model.Email;
                            obj.MobileNumber = model.MobileNumber;
                            obj.StageId = stage.StageId;
                            obj.HiringArchiveStage = null;
                            obj.Source = model.Source;
                            obj.Availabilitys = model.Availabilitys;
                            obj.CurrentLocation = model.CurrentLocation;
                            obj.CurrentCTC = model.CurrentCTC;
                            obj.ExpectedCTC = model.ExpectedCTC;
                            obj.Gender = Enum.GetName(typeof(GenderConstants), model.Gender);
                            if (model.DateOfBirth.HasValue)
                                obj.DateOfBirth = (DateTime)model.DateOfBirth;
                            obj.Experience = model.Experience;
                            obj.StageType = StageFlowType.Sourced;
                            obj.PrebordingStages = stage.StageType == StageFlowType.Preboarding ? PreboardingStages.Start : 0;
                            obj.UploadResume = model.UploadResume;
                            obj.SalaryType = model.SalaryType;
                            obj.PreferredLocation = model.PreferredLocation;
                            obj.CurrentOrganization = model.CurrentOrganization;
                            obj.NewCandidate = true;
                            obj.CreatedOn = DateTime.Now;
                            obj.CreatedBy = claims.employeeId;
                            obj.CompanyId = claims.companyId;
                            obj.OrgId = claims.orgId;
                            obj.CompanyName = _db.Company.Where(x => x.CompanyId == claims.companyId).Select(x => x.RegisterCompanyName).FirstOrDefault();
                            obj.IsCredentialProvided = false;
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            _db.Candidates.Add(obj);
                            await _db.SaveChangesAsync();
                            objResponse.Candidate = obj;

                            if (model.Educations != null)
                            {
                                foreach (var edu in model.Educations)
                                {
                                    Education edobj = new Education();
                                    edobj.CandidateId = obj.CandidateId;
                                    edobj.Degree = edu.Degree;
                                    edobj.BranchSpecialization = edu.BranchSpecialization;
                                    edobj.UniversityCollage = edu.UniversityCollage;
                                    edobj.DateOfJoining = edu.DateOfJoining;
                                    edobj.DateOfCompletion = edu.DateOfCompletion;
                                    edobj.Location = edu.Location;
                                    edobj.IsActive = true;
                                    edobj.IsDeleted = false;
                                    edobj.CreatedOn = DateTime.Now;
                                    _db.Education.Add(edobj);
                                    await _db.SaveChangesAsync();
                                    list.Add(edobj);
                                }
                                objResponse.Educations = list;
                            }
                            res.Message = "Candidate Added";
                            res.Status = true;
                            res.Data = objResponse;
                        }
                        else
                        {
                            res.Message = "Candidate Already Exists";
                            res.Status = false;
                            res.Data = objResponse;
                        }
                    }
                    else
                    {
                        res.Message = "Stage Is Invalid";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Model Is Invalid";
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

        #endregion Api For Add Candidate On Job

        #region API Use TO Restore Recycle Bin Data
        /// <summary>
        /// Created By Ankit Jain On 22/03/2023
        /// API >> POST >> api/contact/restoredata
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("restoredata")]
        public async Task<IHttpActionResult> RestoreData(RestoreHelperclass model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stage = _db.HiringStages.Include("Job").FirstOrDefault(x => x.Job.JobPostId
                              == model.JobId && x.StageType == StageFlowType.Sourced);
                if (stage != null)
                {
                    var candidateData = await _db.Candidates
                            .FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId && !x.IsActive && !x.IsDeleted);
                    if (candidateData != null)
                    {
                        candidateData.JobId = model.JobId;
                        candidateData.StageType = StageFlowType.Sourced;
                        candidateData.StageId = stage.StageId;
                        candidateData.IsActive = true;
                        candidateData.IsDeleted = false;
                        candidateData.UpdatedBy = tokenData.employeeId;
                        candidateData.UpdatedOn = DateTime.Now;

                        _db.Entry(candidateData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Candidate Updated";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Data = candidateData;
                    }
                    else
                    {
                        res.Message = "Data Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                    }
                }
                else
                {
                    res.Message = "Stage Is Invalid";
                    res.Status = false;
                }

            }
            catch (Exception ex)
            {
                logger.Error("API : api/contact/restoredata | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class RestoreHelperclass
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
        }

        #endregion

        #region API Use TO Soft DELETE Candidate and Move Recycle Bin
        /// <summary>
        /// Created By Ankit Jain On 22/03/2023
        /// API >> POST >> api/contact/deletecandidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecandidate")]
        public async Task<IHttpActionResult> DeleteCandidate(int candidateId, int value)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateData = await _db.Candidates
                          .FirstOrDefaultAsync(x => x.CandidateId == candidateId && x.IsActive && !x.IsDeleted);
                if (candidateData != null)
                {
                    if (value == 1)
                    {
                        candidateData.IsActive = false;
                        candidateData.IsDeleted = true;
                        candidateData.DeletedBy = tokenData.employeeId;
                        candidateData.DeletedOn = DateTime.Now;

                        _db.Entry(candidateData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Candidate Deleted";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Data = candidateData;
                    }
                    else
                    {
                        candidateData.IsActive = false;
                        candidateData.IsDeleted = false;
                        candidateData.DeletedBy = tokenData.employeeId;
                        candidateData.DeletedOn = DateTime.Now;
                        candidateData.IsPreboardingStarted = false;
                        _db.Entry(candidateData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Candidate Move In Recycle Bin";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Data = candidateData;
                    }

                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/contact/deletecandidate | " +
                    "Candiadte Id : " + candidateId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API Use TO Get All Move Recycle Bin Data
        /// <summary>
        /// Created By Ankit Jain On 22/03/2023
        /// API >> Get >> api/contact/getrecyclebindata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getrecyclebindata")]
        public async Task<IHttpActionResult> GetRecycleBinBata(int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateData = await (from cd in _db.Candidates
                                           join jd in _db.JobPosts on cd.JobId equals jd.JobPostId
                                           where cd.CompanyId == tokenData.
                                            companyId && !cd.IsActive && !cd.IsDeleted
                                           select new RecycleBinHelper
                                           {
                                               CandidateId = cd.CandidateId,
                                               CandidateName = cd.CandidateName,
                                               Email = cd.Email,
                                               MobileNumber = cd.MobileNumber,
                                               UploadResume = cd.UploadResume,
                                               JobId = cd.JobId,
                                               JobTitle = jd.JobTitle,
                                               StageId = cd.StageId,
                                               StageName = _db.HiringStages.Where(x => x.StageId == cd.StageId)
                                                          .Select(x => x.StageName).FirstOrDefault(),
                                           }).ToListAsync();
                if (candidateData.Count > 0)
                {
                    res.Message = "Get All Candidate";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = candidateData;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = candidateData.Count,
                            Counts = (int)count,
                            List = candidateData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                        res.Data = candidateData;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/contact/getrecyclebindata | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class RecycleBinHelper
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string UploadResume { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public int JobId { get; set; }
            public Guid? StageId { get; set; } = Guid.Empty;
            public string StageName { get; set; }
            public string JobTitle { get; set; }
        }
        #endregion

        #region This Api use To import candidate
        /// <summary>
        /// API >> Post >> api/contact/importcandidate
        /// Created By ankit jain Date - 14/03/2023
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("importcandidate")]
        public async Task<ResponseBodyModel> ImportCandidate(ImportCandidateModelResponse model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddCandidateModelResponse objResponse = new AddCandidateModelResponse();
            List<Education> list = new List<Education>();
            try
            {
                if (model != null)
                {
                    var jobdetails = _db.HiringStages.Include("Job").FirstOrDefault(x => x.Job.JobPostId
                               == model.JobId && x.StageType == StageFlowType.Sourced);
                    if (jobdetails != null)
                    {
                        foreach (var demo in model.Candidate)
                        {
                            Candidate obj = new Candidate();
                            obj.JobId = model.JobId;
                            obj.CandidateName = demo.CandidateName;
                            obj.Email = demo.Email;
                            obj.MobileNumber = demo.MobileNumber;
                            obj.StageId = jobdetails.StageId;
                            obj.HiringArchiveStage = null;
                            obj.Source = demo.Source;
                            obj.Availabilitys = demo.Availabilitys;
                            obj.CurrentLocation = demo.CurrentLocation;
                            obj.CurrentCTC = demo.CurrentCTC;
                            obj.ExpectedCTC = demo.ExpectedCTC;
                            obj.Gender = Enum.GetName(typeof(GenderConstants), demo.Gender);
                            if (demo.DateOfBirth.HasValue)
                                obj.DateOfBirth = (DateTime)demo.DateOfBirth;
                            obj.Experience = demo.Experience;
                            obj.StageType = StageFlowType.Sourced;
                            obj.PrebordingStages = jobdetails.StageType == StageFlowType.Preboarding ? PreboardingStages.Start : 0;
                            obj.UploadResume = demo.UploadResume;
                            obj.SalaryType = demo.SalaryType;
                            obj.PreferredLocation = demo.PreferredLocation;
                            obj.CurrentOrganization = demo.CurrentOrganization;
                            obj.NewCandidate = true;
                            obj.CreatedOn = DateTime.Now;
                            obj.CreatedBy = claims.employeeId;
                            obj.CompanyId = claims.companyId;
                            obj.OrgId = claims.orgId;
                            obj.CompanyName = _db.Company.Where(x => x.CompanyId == claims.companyId)
                                .Select(x => x.RegisterCompanyName).FirstOrDefault();
                            obj.IsCredentialProvided = false;
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            _db.Candidates.Add(obj);
                            await _db.SaveChangesAsync();
                            objResponse.Candidate = obj;
                        }

                        if (model.Educations != null)
                        {
                            foreach (var edu in model.Educations)
                            {
                                Education edobj = new Education();
                                edobj.Degree = edu.Degree;
                                edobj.BranchSpecialization = edu.BranchSpecialization;
                                edobj.UniversityCollage = edu.UniversityCollage;
                                edobj.DateOfJoining = edu.DateOfJoining;
                                edobj.DateOfCompletion = edu.DateOfCompletion;
                                edobj.Location = edu.Location;
                                edobj.IsActive = true;
                                edobj.IsDeleted = false;
                                edobj.CreatedOn = DateTime.Now;
                                _db.Education.Add(edobj);
                                await _db.SaveChangesAsync();
                                list.Add(edobj);
                            }
                            objResponse.Educations = list;
                        }
                        res.Message = "Candidate Added";
                        res.Status = true;
                        res.Data = objResponse;
                    }
                    else
                    {
                        res.Message = "Stage Is Invalid";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Model Is Invalid";
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

        /// <summary>
        /// Creaated By Harshit Mitra on 03-02-2022
        /// </summary>
        public class ImportCandidateModelResponse
        {
            public int JobId { get; set; }
            public List<CandidateModel> Candidate { get; set; }
            public List<Education> Educations { get; set; }
        }

        public class CandidateModel
        {
            public string CandidateName { get; set; }
            public string Email { get; set; }
            public string MobileNumber { get; set; }
            public int Source { get; set; }
            public string Availabilitys { get; set; }
            public string CurrentLocation { get; set; }
            public string CurrentCTC { get; set; }
            public string ExpectedCTC { get; set; }
            public int Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Experience { get; set; }
            public string UploadResume { get; set; }
            public string SalaryType { get; set; }
            public string PreferredLocation { get; set; }
            public List<Education> Educations { get; set; }
            public string CompanyName { get; set; }
            public string CurrentOrganization { get; set; } = string.Empty;
        }

        #endregion

        #region Api For Changing Candidate Stages

        /// <summary>
        /// API >> Post >> api/contact/changestage
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("changestage")]
        public async Task<ResponseBodyModel> ChangeCandidateStage(ChangeCandidateStageModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId);
                if (candidate != null)
                {
                    var Stage = await _db.HiringStages.FirstOrDefaultAsync(x => x.StageId == model.Stage);
                    if (Stage != null)
                    {
                        candidate.StageId = Stage.StageId;
                        candidate.StageType = Stage.StageType;
                        candidate.NewCandidate = false;
                        if (Stage.SechduleRequired)
                        {
                            candidate.InterViewType = Enum.GetName(typeof(InterviewType), model.InterviewType).Replace('_', ' ');
                            candidate.InterviewSechduleDate = model.SechduleDate;
                        }
                        if (candidate.StageType == StageFlowType.Preboarding)
                        {
                            candidate.PrebordingStages = PreboardingStages.Start;
                            candidate.PendingSince = DateTime.Now;
                        }
                        _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Stage Change";
                        res.Status = true;
                        res.Data = candidate;
                    }
                    else
                    {
                        res.Message = "Stage Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api For Changing Candidate Stages

        #region Api To Get All Candidate deta

        /// <summary>
        /// API >> Get >> api/contact/getallcandidatedetails
        /// Created By Ankit Jain on 11-10-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcandidatedetails")]
        public async Task<ResponseBodyModel> GetAllCandidateDetails(string search = null, int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var can = await (from im in _db.Candidates
                                 where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive
                                 select new Getcandidatedata
                                 {
                                     CandidateId = im.CandidateId,
                                     CandidateName = im.CandidateName,
                                     Email = im.Email,
                                     UploadResume = im.UploadResume,
                                     StageName = _db.HiringStages.Where(x => x.StageId == im.StageId).Select(x => x.StageName).FirstOrDefault(),
                                     MobileNumber = im.MobileNumber,
                                     DateOfBirth = im.DateOfBirth,
                                     JobTitle = _db.JobPosts.Where(x => x.JobPostId == im.JobId).Select(x => x.JobTitle).FirstOrDefault(),
                                     PrebordingStages = im.PrebordingStages.ToString(),
                                     StageId = im.StageId,
                                     JobId = im.JobId,
                                     Date = im.CreatedOn
                                 }).OrderByDescending(im => im.Date).ToListAsync();

                if (can.Count > 0)
                {
                    res.Message = "Candidate List Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        var listentry = can.Where(x => x.CandidateName.ToUpper().StartsWith(text) || x.MobileNumber.ToUpper().StartsWith(text) || x.JobTitle.ToUpper().StartsWith(text))
                                   /*.Skip(((int)page - 1) * (int)count).Take((int)count)*/.ToList();
                        res.Data = new PagechangeDatahelper
                        {

                            Counts = (int)count,
                            Listdata = can.Where(x => x.CandidateName.ToUpper().StartsWith(text) || x.MobileNumber.ToUpper().StartsWith(text) || x.JobTitle.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            TotalData = listentry.Count(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = can.Count,
                            Counts = (int)count,
                            List = can.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = can;
                    }
                }
                else
                {
                    res.Message = "Candidate List Not Found";
                    res.Status = false;
                    res.Data = can;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class PagechangeDatahelper
        {
            public int TotalData { get; set; }
            public int Counts { get; set; }
            public object Listdata { get; set; }
            //public object Totaldata { get; set; }

        }
        public class Getcandidatedata
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string Email { get; set; }
            public string MobileNumber { get; set; }
            public string UploadResume { get; set; }
            public string StageName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string JobTitle { get; set; }
            public string PrebordingStages { get; set; }
            public Guid? StageId { get; set; }
            public int JobId { get; set; }
            public DateTime Date { get; set; }
        }
        #endregion Api To Get Candidate Full Detail

        #region Api To Get All Archived Candidate deta

        /// <summary>
        /// API >> Get >> api/contact/getallfiltercandidate
        /// Created By Ankit Jain on 11-10-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallfiltercandidate")]
        public async Task<ResponseBodyModel> GetAllArchivedCandidate(int count)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                if (count == 3)
                {
                    var can = await (from im in _db.Candidates
                                     where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.StageType == StageFlowType.Archived
                                     select new Getcandidatedata
                                     {
                                         CandidateId = im.CandidateId,
                                         CandidateName = im.CandidateName,
                                         Email = im.Email,
                                         UploadResume = im.UploadResume,
                                         StageName = _db.HiringStages.Where(x => x.StageId == im.StageId).Select(x => x.StageName).FirstOrDefault(),
                                         MobileNumber = im.MobileNumber,
                                         DateOfBirth = im.DateOfBirth,
                                         JobTitle = _db.JobPosts.Where(x => x.JobPostId == im.JobId).Select(x => x.JobTitle).FirstOrDefault(),
                                         PrebordingStages = im.PrebordingStages.ToString(),
                                         StageId = im.StageId,
                                         JobId = im.JobId
                                     }).ToListAsync();
                    res.Message = "Candidate List Found";
                    res.Status = true;
                    res.Data = can;

                }
                else if (count == 2)
                {
                    var can = await (from im in _db.Candidates
                                     where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.PrebordingStages == PreboardingStages.Joined
                                     select new Getcandidatedata
                                     {
                                         CandidateId = im.CandidateId,
                                         CandidateName = im.CandidateName,
                                         Email = im.Email,
                                         UploadResume = im.UploadResume,
                                         StageName = _db.HiringStages.Where(x => x.StageId == im.StageId).Select(x => x.StageName).FirstOrDefault(),
                                         MobileNumber = im.MobileNumber,
                                         DateOfBirth = im.DateOfBirth,
                                         JobTitle = _db.JobPosts.Where(x => x.JobPostId == im.JobId).Select(x => x.JobTitle).FirstOrDefault(),
                                         PrebordingStages = im.PrebordingStages.ToString(),
                                         StageId = im.StageId,
                                         JobId = im.JobId
                                     }).ToListAsync();
                    res.Message = "Candidate List Found";
                    res.Status = true;
                    res.Data = can;

                }
                else
                {
                    res.Message = "Candidate List Not Found";
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

        #endregion Api To Get Candidate Full Detail

        #region Api To Get Candidate Full Detail

        /// <summary>
        /// API >> Get >> api/contact/detailsofcandidate
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("detailsofcandidate")]
        public async Task<ResponseBodyModel> GetDetailsOfCandidates(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var can = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == candidateId && x.CompanyId == claims.companyId);
                if (can != null)
                {
                    CandidateDetailModel obj = new CandidateDetailModel
                    {
                        CandidateId = can.CandidateId,
                        JobId = can.JobId,
                        CandidateName = can.CandidateName,
                        AppliedFrom = Enum.GetName(typeof(JobHiringSourceConstants), can.Source),
                        AppliedDate = (DateTime)can.CreatedOn,
                        MobileNumber = can.MobileNumber,
                        Email = can.Email,
                        SalaryType = can.SalaryType,
                        ResumeUrl = can.UploadResume,
                        Profile = new Profiles
                        {
                            Availabilitys = can.Availabilitys,
                            CurrentLocation = can.CurrentLocation,
                            CurrentCTC = can.CurrentCTC,
                            ExpectedCTC = can.ExpectedCTC,
                            Gender = can.Gender,
                            DateOfBirth = can.DateOfBirth,
                            Experience = can.Experience,
                        }
                    };

                    res.Message = "Candidate Details";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Candidate Not Found";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Candidate Full Detail

        #region Api To Get Candidate By Id on Job

        /// <summary>
        /// API >> Get >> api/contact/candidatbyid
        /// Create By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <param name="candidatid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("candidatbyid")]
        public async Task<ResponseBodyModel> GetCandidatById(int candidatId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var can = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == candidatId && x.CompanyId == claims.companyId);
                if (can != null)
                {
                    AddEditCandidateModel obj = new AddEditCandidateModel();
                    obj.CandidateId = can.CandidateId;
                    obj.JobId = can.JobId;
                    obj.CandidateName = can.CandidateName;
                    obj.Email = can.Email;
                    obj.MobileNumber = can.MobileNumber;
                    //obj.sta = (Guid)can.StageId;
                    obj.Source = can.Source;
                    obj.Availabilitys = can.Availabilitys;
                    obj.CurrentLocation = can.CurrentLocation;
                    obj.CurrentCTC = can.CurrentCTC;
                    obj.ExpectedCTC = can.ExpectedCTC;
                    obj.Gender = (int)Enum.Parse(typeof(GenderConstants), can.Gender);
                    obj.DateOfBirth = can.DateOfBirth;
                    obj.Experience = can.Experience;
                    obj.SalaryType = can.SalaryType;
                    obj.PreferredLocation = can.PreferredLocation;
                    obj.UploadResume = can.UploadResume;
                    obj.CurrentOrganization = can.CurrentOrganization;
                    obj.Educations = _db.Education.Where(x => x.IsActive == true && x.IsDeleted == false && x.CandidateId == can.CandidateId).OrderBy(x => x.CreatedOn).ToList();
                    obj.CompanyName = can.CompanyName;
                    res.Message = "Candidat By Id";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Candidat Not Found";
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

        #endregion Api To Get Candidate By Id on Job

        #region This Api Use to Get All Data Change Availabiltys use dev uat and live
        /// <summary>
        /// Created By Ankit On 25-08-2022
        /// Api >> Get >> api/contact/getavailabiltysdata
        /// </summary>
        [HttpGet]
        [Route("getavailabiltysdata")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateData = await _db.Candidates.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                if (candidateData.Count > 0)
                {
                    foreach (var demo in candidateData)
                    {
                        var check = candidateData.Where(x => x.CandidateId == demo.CandidateId).FirstOrDefault();
                        if (check != null)
                        {
                            check.Availabilitys = demo.Availability.ToString();

                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion Use 

        #region Api To Edit Candidate On Job

        /// <summary>
        /// API >> Post >> api/contact/candidateedit
        /// Created By Harshit Mitra on 02-02-2022
        /// Modify By Ankit Jain On 22-03-2023
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("candidateedit")]
        public async Task<ResponseBodyModel> CandidateEdit(AddEditCandidateModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            AddCandidateModelResponse objResponse = new AddCandidateModelResponse();
            List<Education> list = new List<Education>();
            try
            {
                var obj = await _db.Candidates
                    .FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId &&
                        x.CompanyId == claims.companyId);
                if (obj != null)
                {
                    if (await _db.Candidates.AnyAsync(x => (x.Email == model.Email || x.MobileNumber == model.MobileNumber)
                          && x.CandidateId != obj.CandidateId && x.JobId == model.JobId && x.IsActive && !x.IsDeleted))
                    {
                        res.Message = "Candidate Allready Exist";
                        res.Status = false;
                        return res;
                    }
                    obj.Email = model.Email;
                    obj.MobileNumber = model.MobileNumber;
                    obj.CandidateName = model.CandidateName;
                    obj.Availabilitys = model.Availabilitys;
                    obj.Source = model.Source;
                    obj.CurrentLocation = model.CurrentLocation;
                    obj.CurrentCTC = model.CurrentCTC;
                    obj.ExpectedCTC = model.ExpectedCTC;
                    obj.CurrentOrganization = model.CurrentOrganization;
                    obj.Gender = Enum.GetName(typeof(GenderConstants), model.Gender);
                    if (model.DateOfBirth.HasValue)
                        obj.DateOfBirth = (DateTime)model.DateOfBirth;
                    obj.Experience = model.Experience;
                    obj.UploadResume = model.UploadResume;
                    obj.SalaryType = model.SalaryType;
                    obj.PreferredLocation = model.PreferredLocation;
                    obj.UpdatedOn = DateTime.Now;
                    obj.UpdatedBy = claims.employeeId;

                    _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    objResponse.Candidate = obj;

                    if (model.Educations != null)
                    {
                        foreach (var edu in model.Educations)
                        {
                            if (edu.EducationId == 0)
                            {
                                Education edobj = new Education();
                                edobj.CandidateId = obj.CandidateId;
                                edobj.Degree = edu.Degree;
                                edobj.BranchSpecialization = edu.BranchSpecialization;
                                edobj.UniversityCollage = edu.UniversityCollage;
                                edobj.DateOfJoining = edu.DateOfJoining;
                                edobj.DateOfCompletion = edu.DateOfCompletion;
                                edobj.Location = edu.Location;
                                edobj.IsActive = true;
                                edobj.IsDeleted = false;
                                edobj.CreatedOn = DateTime.Now;
                                _db.Education.Add(edobj);
                                await _db.SaveChangesAsync();
                                list.Add(edobj);
                            }
                            else
                            {
                                Education edobj = _db.Education.Where(x => x.EducationId == edu.EducationId).FirstOrDefault();
                                if (edobj != null)
                                {
                                    edobj.CandidateId = obj.CandidateId;
                                    edobj.Degree = edu.Degree;
                                    edobj.BranchSpecialization = edu.BranchSpecialization;
                                    edobj.UniversityCollage = edu.UniversityCollage;
                                    edobj.DateOfJoining = edu.DateOfJoining;
                                    edobj.DateOfCompletion = edu.DateOfCompletion;
                                    edobj.Location = edu.Location;
                                    edobj.UpdatedOn = DateTime.Now;
                                    _db.Entry(edobj).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();
                                    list.Add(edobj);
                                }
                            }
                        }
                        objResponse.Educations = list;
                    }
                    res.Message = "Candidate Updated";
                    res.Status = true;
                    res.Data = objResponse;

                }
                else
                {
                    res.Message = "Candidat Not Found";
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

        #endregion Api To Edit Candidate On Job

        #region This Api Use To Get All The Data By Id

        /// <summary>
        /// Created By Ankit Date - 16-08-2022
        /// Api >> Get >> api/contact/getdata
        /// </summary>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdata")]
        public async Task<ResponseBodyModel> GetData(int jobid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Demo = await _db.Candidates.Where(x => x.JobId == jobid && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (Demo != null)
                {
                    res.Message = "Candidate Data Found";
                    res.Status = true;
                    res.Data = Demo;
                }
                else
                {
                    res.Message = "Candidate not found";
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

        #endregion This Api Use To Get All The Data By Id

        #region ReferCandidate

        ///<summary>
        ///Api >> Post >> api/contact/ReferCandidate
        ///created by shriya created on 29-04-2022
        ///Modify By Ankit Jain Date - 09-09-2022
        ///</summary>
        [HttpPost]
        [Route("ReferCandidate")]
        public async Task<ResponseBodyModel> ReferCandidate(AddEditCandidateModel candidateModel)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddCandidateModelResponse objResponse = new AddCandidateModelResponse();
            List<Education> list = new List<Education>();
            try
            {
                var Stage = _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == candidateModel.JobId && x.StageType == StageFlowType.Sourced).FirstOrDefault();

                Candidate obj = new Candidate();
                obj.JobId = candidateModel.JobId;
                obj.CandidateName = candidateModel.CandidateName;
                obj.Email = candidateModel.Email;
                obj.MobileNumber = candidateModel.MobileNumber;
                obj.StageId = Stage.StageId;
                obj.Source = (int)JobHiringSourceConstants.Referral;
                obj.Gender = Enum.GetName(typeof(GenderConstants), candidateModel.Gender);
                obj.UploadResume = candidateModel.UploadResume;
                obj.Experience = candidateModel.Experience;
                obj.ReferredBy = claims.employeeId;
                obj.CreatedBy = claims.employeeId;
                obj.CreatedOn = DateTime.Now;
                obj.StageType = Stage.StageType;
                obj.PreferredLocation = candidateModel.PreferredLocation;
                obj.SalaryType = candidateModel.SalaryType;
                obj.PrebordingStages = Stage.StageType == StageFlowType.Preboarding ? PreboardingStages.Start : 0;
                obj.IsActive = true;
                obj.IsDeleted = false;
                obj.OrgId = claims.orgId;
                obj.CompanyId = claims.companyId;
                _db.Candidates.Add(obj);
                await _db.SaveChangesAsync();
                objResponse.Candidate = obj;
                if (candidateModel.Educations != null)
                {
                    foreach (var edu in candidateModel.Educations)
                    {
                        Education edobj = new Education();
                        edobj.CandidateId = obj.CandidateId;
                        edobj.Degree = edu.Degree;
                        edobj.BranchSpecialization = edu.BranchSpecialization;
                        edobj.UniversityCollage = edu.UniversityCollage;
                        edobj.DateOfJoining = edu.DateOfJoining;
                        edobj.DateOfCompletion = edu.DateOfCompletion;
                        edobj.Location = edu.Location;
                        edobj.IsActive = true;
                        edobj.IsDeleted = false;
                        edobj.CreatedOn = DateTime.Now;
                        edobj.CompanyId = claims.companyId;
                        edobj.OrgId = claims.orgId;
                        _db.Education.Add(edobj);
                        await _db.SaveChangesAsync();
                        list.Add(edobj);
                    }
                    objResponse.Educations = list;
                }

                res.Message = "Candidate Referred succesfully";
                res.Status = true;
                res.Data = objResponse;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion ReferCandidate

        #region RefralCountOfEmp
        ///<summary>
        /// API >> Get >> api/contact/RefralCountOfEmp
        ///Created  by shriya
        ///created on 30-04-2022
        ///</summary>
        [HttpGet]
        [Route("RefralCountOfEmp")]
        public async Task<ResponseBodyModel> RefralCountOfEmp()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            RefralCountModel objResponse = new RefralCountModel();
            List<RefralCandidateDetail> refral = new List<RefralCandidateDetail>();
            try
            {
                var candidates = await _db.Candidates.Where(x => x.ReferredBy == claims.employeeId && x.CompanyId == claims.companyId)
                    .Select(x => x.CandidateId).ToListAsync();
                if (candidates.Count > 0)
                {
                    foreach (var candidate in candidates)
                    {
                        var candi = _db.Candidates.Where(x => x.CandidateId == candidate).FirstOrDefault();
                        RefralCandidateDetail refer = new RefralCandidateDetail()
                        {
                            CandidateId = candi.CandidateId,
                            CandidateName = candi.CandidateName,
                            ReferredBy = candi.ReferredBy,
                            Gender = candi.Gender,
                            CreatedDate = candi.CreatedOn,
                            MobileNumber = candi.MobileNumber,
                            Email = candi.Email,
                            StageId = (Guid)candi.StageId,
                            JobName = _db.JobPosts.Where(x => x.JobPostId == candi.JobId).Select(x => x.JobTitle).FirstOrDefault(),
                            StageName = _db.HiringStages.Where(x => x.StageId == candi.StageId).Select(x => x.StageName).FirstOrDefault(),
                        };
                        refral.Add(refer);
                    }
                    objResponse.Candidate = refral;
                    objResponse.Count = candidates.Count();
                    res.Message = "Refrence Count Succesfully By Refral Id";
                    res.Status = true;
                    res.Data = objResponse;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = true;
                    res.Data = candidates;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion RefralCountOfEmp

        #region RefralCountOfEmp By Job Id
        ///<summary>
        /// API >> Get >> api/contact/RefralCountOfEmpbyid
        ///Created  by shriya
        ///created on 30-04-2022
        ///</summary>
        [HttpGet]
        [Route("RefralCountOfEmpbyid")]
        public async Task<ResponseBodyModel> RefralCountOfEmpById(int jobId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            RefralCountModel objResponse = new RefralCountModel();
            List<RefralCandidateDetail> refral = new List<RefralCandidateDetail>();
            try
            {
                var candidates = await _db.Candidates.Where(x => x.JobId == jobId && x.IsActive && !x.IsDeleted)
                    .Select(x => x.CandidateId).ToListAsync();
                if (candidates.Count > 0)
                {
                    foreach (var candidate in candidates)
                    {
                        var candi = _db.Candidates.Where(x => x.CandidateId == candidate).FirstOrDefault();
                        RefralCandidateDetail refer = new RefralCandidateDetail()
                        {
                            CandidateId = candi.CandidateId,
                            CandidateName = candi.CandidateName,
                            ReferredBy = candi.ReferredBy,
                            Gender = candi.Gender,
                            CreatedDate = candi.CreatedOn,
                            MobileNumber = candi.MobileNumber,
                            Email = candi.Email,
                            StageId = (Guid)candi.StageId,
                            JobName = _db.JobPosts.Where(x => x.JobPostId == candi.JobId).Select(x => x.JobTitle).FirstOrDefault(),
                            StageName = _db.HiringStages.Where(x => x.StageId == candi.StageId).Select(x => x.StageName).FirstOrDefault(),
                        };
                        refral.Add(refer);
                    }
                    objResponse.Candidate = refral;
                    objResponse.Count = candidates.Count();
                    res.Message = "Refrence Count Succesfully By Refral Id";
                    res.Status = true;
                    res.Data = objResponse;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = true;
                    res.Data = candidates;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion RefralCountOfEmp

        #region This api use for upload Contact Document

        /// <summary>
        ///Created By Ankit On 25-05-2022
        /// Api>>Post>> api/contact/uploadReferdocuments
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadReferdocuments")]
        [AllowAnonymous]
        public async Task<UploadImageResponseContact> UploadGoalDocments()
        {
            UploadImageResponseContact result = new UploadImageResponseContact();
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/referdocument/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\referdocument\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
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

        #endregion This api use for upload Contact Document

        #region Helper Model Classes

        /// <summary>
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        public class AddEditCandidateModel
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public string CandidateName { get; set; }
            public string Email { get; set; }
            public string MobileNumber { get; set; }
            public int Source { get; set; }
            public string Availabilitys { get; set; }
            public string CurrentLocation { get; set; }
            public string CurrentCTC { get; set; }
            public string ExpectedCTC { get; set; }
            public int Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Experience { get; set; }
            public string UploadResume { get; set; }
            public string SalaryType { get; set; }
            public string PreferredLocation { get; set; }
            public List<Education> Educations { get; set; }
            public string CompanyName { get; set; }
            public string CurrentOrganization { get; set; } = string.Empty;
        }

        /// <summary>
        /// Creaated By Harshit Mitra on 03-02-2022
        /// </summary>
        public class AddCandidateModelResponse
        {
            public Candidate Candidate { get; set; }
            public List<Education> Educations { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        public class ChangeCandidateStageModel
        {
            public int CandidateId { get; set; }
            public Guid Stage { get; set; }
            public DateTime? SechduleDate { get; set; }
            public InterviewType? InterviewType { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        public class CandidateDetailModel
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public string CandidateName { get; set; }
            public string AppliedFrom { get; set; }
            public DateTime AppliedDate { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public string ResumeUrl { get; set; }
            public string SalaryType { get; set; }
            public Profiles Profile { get; set; }
            //public ScoreCard ScoreCard { get; set; }
            //public Document Document { get; set; }
            //public Activity Activity { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        public class Profiles
        {
            public string Availabilitys { get; set; }
            public string CurrentLocation { get; set; }
            public string CurrentCTC { get; set; }
            public string ExpectedCTC { get; set; }
            public string Gender { get; set; }
            public string Experience { get; set; }
            public DateTime? DateOfBirth { get; set; }

        }

        public class RefralCountModel
        {
            public List<RefralCandidateDetail> Candidate { get; set; }
            public int Count { get; set; }
        }

        public class RefralCandidateDetail
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string Gender { get; set; }
            public int ReferredBy { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public Guid StageId { get; set; }
            public string StageName { get; set; }
            public string JobName { get; set; }
        }

        /// <summary>
        /// Created By Ankit jain on 25-05-2022
        /// </summary>
        public class UploadImageResponseContact
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        #endregion Helper Model Classes

    }
}