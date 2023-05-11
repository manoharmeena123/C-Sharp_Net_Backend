namespace AspNetIdentity.WebApi.Helper
{
    public class EmailHelperClass
    {
        public static string DefaultMailBody { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2'  style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: " +
            "0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src='<|IMAGE_PATH|>'>" +
            "</div><h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;padding-left: 30px;" +
            " padding-right: 30px;'><|MAIL_TITLE|></h1><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'>" +
            "<div style='text-align: justify; padding-left:30px;padding-right: 30px;margin-top: 35px;padding-bottom: 15px'>" +
            "<|INNER_BODY|></div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'><|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";
        public static string GetButtonInner(string btnTitle, string btnURL)
        {
            string button = "<div style='padding-top: 8px;margin-botton:8px;'> <a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: " +
                "10px;border-radius: 5px;text-decoration: none;color: #fff;' href='<|URL|>'> <|BUTTON_TITLE|> </a> </div>";
            return button
                .Replace("<|BUTTON_TITLE|>", btnTitle)
                .Replace("'<|URL|>", btnURL);
        }

    }
}