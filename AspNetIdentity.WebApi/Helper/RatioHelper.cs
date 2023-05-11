using System;

namespace AspNetIdentity.WebApi.Helper
{
    public class RatioHelper
    {
        public string Getratio(int TotalApplication, int TotalHired)
        {
            string ratio;
            try
            {
                for (var num = TotalApplication; num > 1; num--)
                {
                    if ((TotalHired % num) == 0 && (TotalApplication % num) == 0)
                    {
                        TotalHired = TotalHired / num;
                        TotalApplication = TotalApplication / num;
                    }
                }
                ratio = TotalHired + ":" + TotalApplication;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            return ratio;
        }

        public int Gsetpercent(double TotalApplication, double TotalHired)
        {
            double percentage;
            try
            {
                for (var num = TotalApplication; num > 1; num--)
                {
                    if ((TotalHired % num) == 0 && (TotalApplication % num) == 0)
                    {
                        TotalHired = TotalHired / num;
                        TotalApplication = TotalApplication / num;
                    }
                }
                percentage = (TotalHired / TotalApplication) * 100;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            return (int)percentage;
        }
    }
}