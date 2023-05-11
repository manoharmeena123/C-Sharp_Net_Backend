using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using EASendMail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Helper
{
    public static class JobHelper
    {
        public static List<HiringStage> CreateDefaultHiringFlow(JobPost job)
        {
            using (var _db = new ApplicationDbContext())
            {
                var stageType = Enum.GetValues(typeof(StageFlowType))
                                    .Cast<StageFlowType>()
                                    .Select(x => new HiringStage
                                    {
                                        StageId = Guid.NewGuid(),
                                        Job = job,
                                        StageName = x.ToString(),
                                        StageType = x,
                                        StageOrder = ((int)x),
                                        SechduleRequired = (x == StageFlowType.Interview),
                                        CreatedBy = 0,
                                        CreatedOn = DateTime.Now,
                                        IsActive = true,
                                        IsDeleted = false,
                                        CompanyId = job.CompanyId,
                                        OrgId = job.OrgId,
                                        IsDefault = true,

                                    }).ToList();
                return stageType;
            }
        }
        public static object GetListOfStage(List<HiringStage> list, List<Candidate> candidateListOnStage)
        {
            var Data = list
                    .Select(x => new
                    {
                        x.StageId,
                        x.StageName,
                        x.StageType,
                        x.IsDefault,
                        x.StageOrder,
                        NameEditable = !x.IsDefault,
                        HavingSubStage = ((x.StageType == StageFlowType.Screening || x.StageType == EnumClass.StageFlowType.Interview) && x.IsDefault)
                    })
                    .Select(x => new
                    {
                        x.StageId,
                        x.StageName,
                        x.StageType,
                        x.NameEditable,
                        x.HavingSubStage,
                        x.StageOrder,
                        IsDelatable = (candidateListOnStage.Count(z => z.StageId == x.StageId) == 0 && !x.IsDefault)
                    })
                    .ToList();
            return Data;
        }
        public static List<HiringStage> AddingStageInPosition(List<HiringStage> list, int position, HiringStage insertData)
        {
            list.Insert(position, insertData);
            int i = 1;
            foreach (var item in list)
            {
                item.StageOrder = i++;
            }
            UpdatingStage(list);
            return list;
        }
        public static List<HiringStage> RemoveStageFromPosition(List<HiringStage> list, int position, int employeeId)
        {
            list[position].StageOrder = 0;
            list[position].IsDeleted = true;
            list[position].IsActive = false;
            list[position].DeletedOn = DateTime.Now;
            list[position].DeletedBy = employeeId;

            list = list.OrderBy(x => x.StageOrder).ToList();
            int i = 0;
            foreach (var item in list)
            {
                item.StageOrder = i++;
            }
            UpdatingStage(list);
            return list;
        }
        public static void UpdatingStage(List<HiringStage> list)
        {
            using (var _db = new ApplicationDbContext())
            {
                foreach (var item in list)
                {
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
        }

        public static bool SendMail(string htmlBody)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];
                AddressCollection obj = new AddressCollection();
                oMail.To = obj;
                oMail.Subject = "Your Interview Link Url";
                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                oServer.Port = 587;
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);

                Console.WriteLine("email was sent to candidate successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ex.Message);
            }
            return true;

        }

        public static string InterviewScheduleForCandidate { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> We are excited about your interview at " +
            "<|COMPANYNAME|> for the post of " +
            "<|JOBTITLE|>.Your " +
            "<|INTERVIEWTYPE|> is scheduled at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span></label><br><br>" +
            "<a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|TEAMSLINK|>'>Interview Link</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string ScheduleInterviewdforRecruiter { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
           "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
           "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
           "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Recruiter,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
           "<|RECRUITERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>You have scheduled " +
           "<|INTERVIEWTYPE|> of " +
           "<|CANDIDATENAME|> with " +
            "<|INTERVIEWERNAME|> for the post of " +
           "<|JOBTITLE|>,at " +
           "<|STARTDATE|> on " +
           "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span>" +
           "</label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
           "<|TEAMSLINK|>'>Interview Link</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
           "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
           "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
           "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
           "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
           "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
           "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
           "<|COMPANYADDRESS|></p></div></div></body>";
        public static string ScheduleInterviewdforInterviewer { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Interviewer,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|INTERVIERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>We would like to inform you that " +
            "<|INTERVIEWTYPE|> has been Scheduled with " +
            "<|CANDIDATENAME|> for the " +
            "<|JOBTITLE|>,as an interviewer please be available at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span>" +
            "</label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|TEAMSLINK|>'>Interview Link</a><br><br><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Add candidate review using this link:</span></label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|REVIEWLINK|>'>Interview Review</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string RescheduleInterviewCandidate { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> We are excited about your interview at " +
            "<|COMPANYNAME|> for the post of " +
            "<|JOBTITLE|>.Your " +
            "<|INTERVIEWTYPE|> is now Rescheduled at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span></label><br><br>" +
            "<a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|TEAMSLINK|>'>Interview Link</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";

        public static string RescheduleInterviewRecruiter { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
           "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
           "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
           "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Recruiter,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
           "<|RECRUITERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>You have Rescheduled " +
           "<|INTERVIEWTYPE|> of " +
           "<|CANDIDATENAME|> with " +
           "<|INTERVIEWERNAME|> for the post of " +
           "<|JOBTITLE|>,at " +
           "<|STARTDATE|> on " +
           "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span>" +
           "</label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
           "<|TEAMSLINK|>'>Interview Link</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
           "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
           "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
           "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
           "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
           "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
           "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
           "<|COMPANYADDRESS|></p></div></div></body>";


        public static string RescheduleInterviewInterviewer { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Interviewer,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|INTERVIERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>We would like to inform you that " +
            "<|INTERVIEWTYPE|> has been Rescheduled with " +
            "<|CANDIDATENAME|> for the Post of " +
            "<|JOBTITLE|>,as an interviewer please be available at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Join the meeting using this link</span>" +
            "</label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|TEAMSLINK|>'>Interview Link</a><br><br><label class='mt-2 mb-2' style='font-size: 12px;color: #000C1D;font-weight: 700;'>Add candidate review using this link:</span></label><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            "<|REVIEWLINK|>'>Interview Review</a><br><br></div><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><hr><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string FaceToFaceInterviewCandidate { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class='flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> We are excited to inform you that your " +
            "<|INTERVIEWTYPE|> has been Scheduled at " +
            "<|COMPANYNAME|> for the post of " +
            "<|JOBTITLE|>.Please be available at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 11px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";

        public static string FacetoFaceInterviewInterviewer { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class='flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Interviewer,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|INTERVIEWERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>We would like to inform you that " +
            "<|INTERVIEWTYPE|> has been Scheduled with " +
            "<|CANDIDATENAME|> for the post of " +
            "<|JOBTITLE|>,as an interviewer please be available at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 11px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";

        public static string FacetoFaceInterviewRecruiter { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class='flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Interview Invitation</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello Recruiter,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|RECRUITERNAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>You have scheduled " +
            "<|INTERVIEWTYPE|> of " +
            "<|CANDIDATENAME|> with " +
            "<|INTERVIEWERNAME|> for the post of " +
            "<|JOBTITLE|> at " +
            "<|STARTDATE|> on " +
            "<|STARTTIME|>.</div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 11px;color: #000C1D;font-weight: 700;'>In case of any queries, please contact</label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Phone<span style='padding-left: 40px;'>" +
            "<|RECRUITERPHONE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Email<span style='padding-left: 43px;'>" +
            "<|RECRUITEREMAIL|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";

        public static string DocumentVarification { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
          "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
          "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Upload Your Documents</h1><br>" +
          "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'>" +
          "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> Thanks for participating in our interview process for the role of " +
          "<|JOBTITLE|> as a next step, we will need some additional information.</div><br><hr><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;text-align: center;'> <label>Please visit our portal to submit this information</label><br><br><br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
          "<|UPLOADDOCUMENT|>'>Upload Your Documents</a></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
          "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
          "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div>" +
          "<div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
          "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
          "<|COMPANYADDRESS|></p></div></div></body>";
        public static string UpdateYourDocument { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
              "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
              "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Reupload Your Documents</h1><br>" +
              "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'>" +
              "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> Thanks for participating in our interview process for the role of " +
              "<|JOBTITLE|> as a next step, we request you to reupload your doucument.</div><br><hr><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;text-align: center;'><label >Please visit our portal to submit this information</label> <br> <br> <br><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
              "<|UPLOADDOCUMENT|>'>Reupload Your Documents</a></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
              "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
              "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
              "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
              "<|COMPANYADDRESS|></p></div></div></body>";
        public static string FindyourCredential { get; set; } = "<body style='display: flex;justify-content: center;height:100vh;'> " +
            "<div class='  flextcontainer card p-2' style='text-align: center;width: 83%; min-height: 50px;position: relative;margin-bottom: " +
            "24px;border: 1px solid #f2f4f9;border-radius: 10px;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<hr> <h1 class='mt-2 mb-2' style='margin-top: 10px;margin-bottom:10px;'>Send Credintal</h1><div class='m-2 mb-3'><label class='mt-2 mb-2' style='margin-top: 10px;margin-bottom:10px;'> Dear </label>" +
            "<|CANDIDATENAME|> <br> <label style='margin-top: 10px;margin-bottom:20px;' > Welcome to the team </label>" +
            "<|CREDENTIALMESSAGE|> <br>" +
            "<|COMPANYNAME| <p> We are glad to receive your acceptance. We are so excited about having you on our team!  With your experience, you will be a great addition. We hope you bring a lot of positive energy with you and we can work well together and share many successes.Your documentation process is completed.Please find your login credentials below .</p> Thanks </div></body>";
        public static string OfferLetter { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 26px;'>Offer Letter</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'> We are all very excited to discuss and get to know you.We have been impressed with your background and would like to formally offer you the position of " +
            "<|JOBTITLE|> in our organization.Your expected starting date is " +
            "<|OFFERDATE|> and the offer will be revoked if the joining formalities are not completed by " +
            "<|OFFERDATE|> You will be asked to sign a contract agreement- like confidentiality, nondisclosure, and non-compete at the beginning of your employment.We would like to have your response by " +
            "<|OFFERDATE|>.I," +
            "<|HRNAME|> will be the immediate contact person for any query related to joining, offer letter, HR policies, and any query related to the process. For any queries feel free to contact me on " +
            "<|HRCONTACT|> and your immediate contact person for Project and Technology related discussions and Team meetings. Their Contact details we will provide you soon.In the meantime, please feel free to contact me via email or phone,if you have any questions.</div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|HRNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730;font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string Cancelinterview { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;'  src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Cancel Interview</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div style='padding-left: 30px;'>" +
            "<|CANCELREASON|></div><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string RevokeTemplete { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'> " +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'> " +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Revoke</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div style='padding-left: 30px;'>" +
            "<|REVOKEREASON|></div><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string CredentialTemplete { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'> " +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'> " +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Credential</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div style='padding-left: 30px;'>" +
            "<|FINDYOURCREDENTIAL|></div><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string ReminderTempletesDocSubmission { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Reminder</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>This mail is just a friendly reminder about your Document Submission.Please Submit Your Documents as Soon as Possible to proceed further, For more queries you can contact us.</div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards," +
            "</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string ReminderTempletesOfferAccpetation { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Reminder</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>" +
            "<|CANDIDATENAME|></label><br><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>This mail is just a friendly reminder about your Offer Acceptance.Please Submit Your Acceptance as Soon as Possible to proceed further, For more queries you can contact us.</div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Regards," +
            "</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #000C1D;font-weight: 700;'>" +
            "<|RECRUITERNAME|></label><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>" +
            "<|COMPANYNAME|></label><br><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #DE3730; font-weight: 400;text-align: center;padding-top: 20px;'>Don’t share this link or mail with anyone</div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
    }
}