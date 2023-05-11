using AspNetIdentity.WebApi.Model;
using System;
using System.Net.Mail;

namespace AspNetIdentity.WebApi.Helper
{
    public class UserEmailHelper
    {
        public static UserEmail SendEmail(UserEmail mailModel)
        {
            try
            {
                if (mailModel != null)
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.To.Add(mailModel.To);
                    //senders email address
                    mailMessage.From = new MailAddress("Notification@moreyeahs.in");
                    mailMessage.Subject = mailModel.Subject;
                    mailMessage.Body = mailModel.Body;
                    if (mailModel.filepath != null)
                    {
                        mailMessage.Attachments.Add(new Attachment(mailModel.filepath));
                    }
                    mailMessage.IsBodyHtml = true;
                    //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("Notification@moreyeahs.in", "Mud70349");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mailMessage);
                    mailModel.Status = "Email Sent Successfully";
                    mailModel.Success = true;
                    return mailModel;
                }
                else
                {
                    mailModel.Status = "Email not Sent";
                    mailModel.Success = true;
                    return mailModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}