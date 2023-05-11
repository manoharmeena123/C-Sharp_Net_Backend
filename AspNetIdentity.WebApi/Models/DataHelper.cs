using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AspNetIdentity.WebApi.Models
{
    public static class DataHelper
    {
        public static string GeneratePassword(int length) //length of salt
        {
            char[] chars = new char[length];
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        //public static string GeneratePasswords(int length) //length of salt
        //{
        //    char[] chars = new char[length];
        //    try
        //    {
        //        const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        //        var randNum = new Random();
        //        var allowedCharCount = allowedChars.Length;
        //        foreach (var item in chars)
        //        {
        //            chars[item] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //    return new string(chars);
        //}
        public static string GeneratePasswords(int length)
        {
            var result = "";
            try
            {
                Random random = new Random();
                var chars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
                result = new string(
                    Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());
            }
            catch (Exception ex)
            {
                result = ex.Message.Replace(" ", "").Take(10).ToString();
            }
            return result;
        }
        public static string RandomStringGenerate(int size)
        {
            Random randomStrigs = new Random();
            string @string = "1234567890@#!%abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string ran = "";

            for (int i = 0; i < size; i++)
            {
                int x = randomStrigs.Next(62);
                ran += @string[x];
            }
            return ran;
        }
        public static string EncodePassword(string pass, string salt) //encrypt password
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);
            return EncodePasswordMd5(Convert.ToBase64String(inArray));
        }

        public static string EncodePasswordMd5(string pass) //Encrypt using MD5
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        public static string EncodePassword(string sData) // Encode
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
    }
}