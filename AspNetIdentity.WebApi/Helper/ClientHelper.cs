using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace AspNetIdentity.WebApi.Helper
{
    public class ClientHelper
    {
        public static string GetRandomClientImage(string gender)
        {
            Random random = new Random();
            var path = "uploadimage\\clientimages\\" + gender + random.Next(1, 4) + ".jpg";
            return path;
        }

        public static string GetflagImage(string flagcode)
        {
            Random random = new Random();
            var path = "uploadimage\\flags\\" + flagcode + ".svg";
            return path;
        }

        /// <summary>
        /// Craete by Shriya On 09-06-2022
        /// </summary>
        /// <param name="forgetemail"></param>
        /// <param name="baseurl"></param>
        /// <returns></returns>
        public static string SendForgetMail(string forgetemail, string baseurl)
        {
            var key = ConfigurationManager.AppSettings["EncryptKey"];
            var data = "UserEmail=" + forgetemail;
            string token = EncryptDecrypt.EncryptData(key, data);
            string path = baseurl + "/#/authentication/create-new-password?token=" + token;
            //string decrptdata= EncryptDecrypt.DecryptData(key, token);
            string fcode = "<body style=' display: flex;align-items: center;justify-content: center;height:100vh;'>";
            fcode += "<div class='  flextcontainer card p-2' style='text-align: center;width: 83%; min-height: 50px;position: relative;margin-bottom: 24px;border: 1px solid #f2f4f9;border-radius: 10px;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>";
            fcode += "<img class='imgg mb-2' style='  width: 40%;margin: auto;display: block;' src='" + baseurl + "'+'/assets/logo-moreyeahs.png'>";
            fcode += "<hr>";
            fcode += "<h1 class='mt-2 mb-2' style='margin-top: 10px;margin-bottom:10px;'>Reset Your Password ?</h1>";
            fcode += "<div class='m-2 mb-3'>";
            fcode += "<label style='margin-top: 10px;margin-bottom:20px;' >You are receiving this email becouse you are requested for password reset for your HRMS account. Click on the link below to set new password.</label>";
            fcode += "<br><br>";
            fcode += "</div>";
            fcode += "<div>";
            fcode += "<a  style='margin-top:20px;margin-botton:20px;background: #911924;border-color: #911924;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;text-transform: uppercase;' href='" + path + "'>Reset My Password</a>";
            fcode += "<br><br>";
            fcode += "</div>";
            fcode += "<div class='m-2 mb-3'>";
            fcode += "<label  style='margin-top: 20px;margin-bottom:10px;' >If you don't requested to chnage your password simply ignore this email.</label>";
            fcode += "</div>";
            fcode += "</div>";
            fcode += "</body>";
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            message.To.Add(new MailAddress(forgetemail));
            message.Subject = "Reset Your Password";
            message.IsBodyHtml = true; //to make message body as html
                                       //message.Body = htmlString;
            message.Body = fcode;
            smtp.Port = 587;
            //smtp.Host = "smtp.gmail.com"; //for gmail host
            smtp.Host = "in-v3.mailjet.com"; //for maijet host
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential("API KEY", "Secret Key");

            //message.From = new MailAddress("hmitra@moreyeahs.in"); smtp.Credentials = new NetworkCredential("ae90fa3366ca36be654ccad085e883f3", "de8612fe2189529566cf6f33c9c052a1"); // Harshit
            message.From = new MailAddress("hr@moreyeahs.in"); smtp.Credentials = new NetworkCredential("b1107278f15394e14147c21d599a7349", "3a0fe0d5c4b94377d8493cab02f73680"); // Moreyeahs
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
            return path;
        }




        #region This is used for encrytion function'

        public class EncryptDecrypt
        {
            public static string EncryptData(string key, string plainText)
            {
                byte[] iv = new byte[16];
                byte[] array;
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }
                return Convert.ToBase64String(array);
            }

            public static string DecryptData(string key, string cipherText)
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        #endregion This is used for encrytion function'

        public static string NumericNumConvToAbbv(double num)
        {
            if (num >= 0 && num < 1000)
            {
                return num.ToString("#,0");
            }
            else if (num >= 1000 && num <= 10000) /*1,000 =1 K*/
            {
                return (num / 1000).ToString("0.#") + " K";
            }
            else if (num >= 10000 && num <= 100000)
            {
                return (num / 1000).ToString("0.#") + " k";
            }
            else if (num >= 100000 && num <= 1000000)
            {
                return (num / 1000).ToString("#,0k");
            }
            else if (num >= 1000000 && num <= 10000000) /*1,000,000 = 1 millon*/
            {
                return (num / 1000000).ToString("0.#") + " m";

            }
            else if (num >= 10000000 && num <= 100000000)
            {
                return (num / 1000000).ToString("0.#") + " m";
            }
            else if (num >= 100000000 && num <= 1000000000)
            {
                return (num / 1000000).ToString("#,0m");
            }
            else if (num >= 1000000000 && num <= 10000000000) /*1,000,000,000 = 1 billon*/
            {
                return (num / 1000000000).ToString("0.#") + " B";
            }
            else if (num >= 10000000000 && num <= 100000000000)
            {
                return (num / 1000000000).ToString("0.#") + " b";
            }
            else if (num >= 100000000000 && num <= 1000000000000)
            {
                return (num / 1000000000).ToString("#,0b");
            }
            else if (num >= 1000000000000 && num <= 10000000000000) /*1,000,000,000,000 = 1 Trillon*/
            {
                return (num / 1000000000000).ToString("0.#") + " t";
            }
            else if (num >= 10000000000000 && num <= 100000000000000)
            {
                return (num / 1000000000000).ToString("0.#") + " t";
            }
            else if (num >= 100000000000000 && num <= 1000000000000000)
            {
                return (num / 1000000000000).ToString("#,0t");
            }
            else if (num >= 1000000000000000 && num <= 10000000000000) /*1,000,000,000,000,000 = 1 Quadrillon*/
            {
                return (num / 1000000000000000).ToString("0.#") + " qi";
            }
            else if (num >= 10000000000000 && num == 100000000000000)
            {
                return (num / 1000000000000000).ToString("0.#") + " qi";
            }
            else
            {
                return num.ToString("#,0");
            }
            //else if(num <= 100000000000000)
            //{
            //    return (num / 1000000000000000).ToString("#,0qi");
            //}
        }
    }
}