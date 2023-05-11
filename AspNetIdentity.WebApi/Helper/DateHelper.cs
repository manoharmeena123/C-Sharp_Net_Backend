using System;

namespace AspNetIdentity.WebApi.Helper
{
    public class DateHelper
    {
        public DataHelperResponse GetDateTime(string cohort)
        {
            DateTime currentDate = DateTime.Now;
            DateTime fromdate = DateTime.Now;
            DateTime todate = DateTime.Now;
            DataHelperResponse dateHelperDTO = new DataHelperResponse();
            if (cohort.ToLower() == "monthly")
            {
                fromdate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                DateTime endDate = fromdate.AddMonths(1).AddDays(-1);
                todate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 12, 59, 59);
            }
            else if (cohort.ToLower() == "weekly")
            {
                fromdate = currentDate.AddDays(-7);
                fromdate = new DateTime(fromdate.Year, fromdate.Month, fromdate.Day, 0, 0, 0);
                todate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 59, 59);
            }
            else
            {
                fromdate = currentDate.AddMonths(-12);
                fromdate = new DateTime(fromdate.Year, fromdate.Month, 1, 0, 0, 0);
                todate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 59, 59);
            }
            dateHelperDTO.startDate = fromdate;
            dateHelperDTO.endDate = todate;
            return dateHelperDTO;
        }
    }

    public class DataHelperResponse
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}