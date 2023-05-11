using AspNetIdentity.WebApi.Models;
using EASendMail;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AspNetIdentity.WebApi.Helper
{
    public static class SmtpMailHelper
    {
        public static async Task SendMailAsync(SendMailModelRequest sendMailRequest)
        {
            try
            {
                var mailTo = sendMailRequest.MailTo.Select(x => new MailAddress(x)).ToList();
                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                if (sendMailRequest.IsCompanyHaveDefaultMail)
                {
                    SmtpMail oMail = new SmtpMail("TryIt");
                    oMail.From = sendMailRequest.SmtpSettings.From;
                    oMail.To.AddRange(mailTo);
                    oMail.Subject = sendMailRequest.Subject;
                    oMail.ImportHtml(sendMailRequest.MailBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                    SmtpServer oServer = new SmtpServer(sendMailRequest.SmtpSettings.SmtpServer);
                    oServer.User = sendMailRequest.SmtpSettings.MailUser;
                    oServer.Password = sendMailRequest.SmtpSettings.MailPassword;
                    oServer.Port = sendMailRequest.SmtpSettings.Port;
                    if (sendMailRequest.Url != null)
                    {
                        var attachmentPathe = Path.Combine(HttpRuntime.AppDomainAppPath, sendMailRequest.Url);
                        oMail.AddAttachment(attachmentPathe);
                    }
                    oServer.ConnectType = sendMailRequest.SmtpSettings.ConectionType;
                    SmtpClient oSmtp = new SmtpClient();
                    await oSmtp.SendMailAsync(oServer, oMail);
                }
                else
                {
                    SmtpMail oMail = new SmtpMail("TryIt");
                    oMail.From = ConfigurationManager.AppSettings["MasterEmail"];
                    oMail.To.AddRange(mailTo);
                    oMail.Subject = sendMailRequest.Subject;
                    oMail.ImportHtml(sendMailRequest.MailBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                    SmtpServer oServer = new SmtpServer(ConfigurationManager.AppSettings["SmtpServer"]);
                    oServer.User = ConfigurationManager.AppSettings["MailUser"];
                    oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                    oServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                    if (sendMailRequest.Url != null)
                    {
                        var attachmentPathe = Path.Combine(HttpRuntime.AppDomainAppPath, sendMailRequest.Url);
                        oMail.AddAttachment(attachmentPathe);
                    }
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    SmtpClient oSmtp = new SmtpClient();
                    await oSmtp.SendMailAsync(oServer, oMail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
