using System.Net.Mail;

namespace AspNetIdentity.WebApi.Model
{
    public class UserEmail
    {
        public string To { get; set; }
        public string FromMail { get; set; }
        public string MailPassword { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string filepath { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public int StatusReason { get; set; }
        public MailMessage MailMessage { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class UserEmailMessage
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class UserEmailDTOResponse
    {
        public bool Success { get; set; }
        public int StatusReason { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}