using EASendMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AspNetIdentity.WebApi.Models
{
    public class ResponseBodyModel
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public object Data { get; set; }

    }
    public class ResponseStatusCode : ResponseBodyModel
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
    public class UploadImageResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string URL { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public string ExtensionType { get; set; }
    }

    public class GetCurrencyList
    {
        public string ISOCurrencySymbol { get; set; }
        public string CurrencyEnglishName { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class PaginationData
    {
        public int TotalData { get; set; }
        public int Counts { get; set; }
        public object List { get; set; }

    }

    public static class TimeZoneConvert
    {
        public static DateTimeOffset ConvertTimeToSelectedZone(DateTime inTime, string zoneId = "India Standard Time")
        {
            return TimeZoneInfo.ConvertTimeFromUtc(inTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(zoneId));
        }
    }
    public class GetClassDiffrences
    {
        public static List<GetDiffrenceResponse> GetDifferences<T>(T oldData, T newData) =>
            typeof(T).GetProperties()
                .Where(property => property.GetValue(oldData) != property.GetValue(newData))
                .Select(property => new GetDiffrenceResponse
                {
                    Name = property.Name,
                    Change = property.GetValue(oldData) + " to " + property.GetValue(newData),
                })
                .ToList();
        public class GetDiffrenceResponse
        {
            public string Name { get; set; } = String.Empty;
            public string Change { get; set; } = String.Empty;
        }
    }
    public class SendMailModelRequest
    {
        public bool IsCompanyHaveDefaultMail { get; set; } = false;
        public string Subject { get; set; } = string.Empty;
        public string MailBody { get; set; } = String.Empty;
        public List<string> MailTo { get; set; } = new List<string>();
        public SmtpSendMailRequest SmtpSettings { get; set; }
        public string Url { get; set; }
    }
    public class SmtpSendMailRequest
    {
        public string From { get; set; } = String.Empty;
        public string SmtpServer { get; set; } = String.Empty;
        public string MailUser { get; set; } = String.Empty;
        public string MailPassword { get; set; } = String.Empty;
        public int Port { get; set; } = 0;
        public SmtpConnectType ConectionType { get; set; } = SmtpConnectType.ConnectSSLAuto;
    }

}