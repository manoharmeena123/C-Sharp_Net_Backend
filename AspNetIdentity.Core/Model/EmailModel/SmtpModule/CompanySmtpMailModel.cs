using EASendMail;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.SmtpModule
{
    public class CompanySmtpMailModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int CompanyId { get; set; } = 0;
        public string From { get; set; } = string.Empty;
        public string MailUser { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string SmtpServer { get; set; } = string.Empty;
        public SmtpConnectType ConnectType { get; set; } = SmtpConnectType.ConnectSSLAuto;
    }
}