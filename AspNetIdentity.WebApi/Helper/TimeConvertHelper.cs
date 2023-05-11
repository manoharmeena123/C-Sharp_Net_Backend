using System;
using System.Linq;

namespace AspNetIdentity.WebApi.Helper
{
    public static class TimeConvertHelper
    {
        public static TimeSpan ConvertStringToTimeSpan(string time)
        {
            if (string.IsNullOrEmpty(time)) { return TimeSpan.Zero; }
            else
            {
                if (time.Contains("."))
                {
                    int[] splitTime = time.Split('.').Select(x => Convert.ToInt32(x)).ToArray();
                    if (splitTime.Length == 2)
                    {
                        return new TimeSpan(splitTime[0], splitTime[1], 0);
                    }
                    else
                    {
                        return new TimeSpan(splitTime[0], 0, 0);
                    }
                }
                else if (time.Contains(":"))
                {
                    int[] splitTime = time.Split(':').Select(x => Convert.ToInt32(x)).ToArray();
                    if (splitTime.Length == 2)
                    {
                        return new TimeSpan(splitTime[0], splitTime[1], 0);
                    }
                    else
                    {
                        return new TimeSpan(splitTime[0], 0, 0);
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
    }
}