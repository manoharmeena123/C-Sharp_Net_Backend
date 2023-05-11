namespace AspNetIdentity.WebApi.Helper
{
    public class TaskHelper
    {
        public static string CreateTaskMailBody { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: " +
            "1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'>" +
            "<img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\">" +
            "</div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>" +
            "<|CRETEBY|> Created A <br> Task Id : <|TASKID|>" +
            "</h1>" +
            "<div style='text-align: center;padding-top: 1px;'>" +
            "<p style='font-size: 12px;font-weight: 600;'>" +
            "<|CREATEFOR|> <|CREATEDATE|>" +
            "</p>" +
            "<div style='padding-top: 8px;'>" +
            "<a style='margin-top:20px;margin-botton:20px;background:#F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='<|TASKLINK|>'>View Task" +
            "</a>" +
            "</div>" +
            "</div>" +
            "<div style='padding-left: 10px;padding-right: 10px;'>" +
            "<p style='padding-left: 30px;font-size: 15px;color: #000C1D;font-weight: 600;margin-bottom: 10px;'>Title" +
            "</p>" +
            "<p style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'><|TASKTITLE|>" +
            "</p>" +
            "<p style='padding-left: 30px;font-size: 15px;color: #000C1D;font-weight: 600;margin-bottom: 10px;'>Description" +
            "</p>" +
            "<p style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>" +
            "<|DISCRIPTION|>" +
            "</p>" +
            "</div>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'>" +
            "<div style='margin-top: 30px;background: #F8F9FC;padding-top: 15px;padding-bottom: 15px;'>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Created by : " +
            "<span style='padding-left: 40px;'><|CREATEBY|>" +
            "</span>" +
            "</label>" +
            "<br>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Assigned to : " +
            "<span style='padding-left: 35px;'>" +
            "<|CREATEFOR|>" +
            "</span>" +
            "</label>" +
            "<br>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Status : <span style='padding-left: 68px;'>" +
            "<|STATUS|>" +
            "</span>" +
            "</label>" +
            "<br>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Priority : <span style='padding-left: 64px;'>" +
            "<|PRIORITY|>" +
            "</span>" +
            "</label>" +
            "<br>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Project Name : <span style='padding-left: 64px;'>" +
            "<|PROJECTNAME|>" +
            "</span>" +
            "</label>" +
            "<br>" +
            "<label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Original Estimate : <span style='padding-left: 1px;'>" +
            "<|ESTIMATETIME|>" +
            "</span>" +
            "</label>" +
            "</div>" +
            "<div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'>" +
            "<h5 style='margin: 0;color: #ffffff;padding: 10px;'><|COMPANYNAMEE|>" +
            "</h5>" +
            "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|>" +
            "</p>" +
            "</div>" +
            "</div>" +
            "</body>";

        public static string MentionInTask { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'><div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;padding-left: 30px;padding-right: 30px;'>" +
            "<|CREATEBY|> created a <br> task <|TASKTITLE|></h1><div style='text-align: center;padding-top: 1px;'><div style='padding-top: 15px;'><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
            ".<|TASKLINK|>'>View Task</a></div></div><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div style='margin-top: 35px;background-color: #F8F9FC;padding-top: 10px;padding-bottom: 10px;'><div style='display: inline-flex;width: 100%;'>" +
            "<div class='lables' style='width: 45%;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'> Created by:</label>/div><div class='labless' style='width: 100%;text-align: justify;padding-right: 15px;padding-left: 15px;'><label class='mt-2 mb-2' style='font-size: 12px; color: #616576;font-weight: 400;'>" +
            "<|CREATEBY|></label></div></div><div style='display: inline-flex;width: 100%;'><div class='lables' style='width: 45%;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Title:</label></div><div class='labless' style='width: 100%;text-align: justify;padding-right: 15px;padding-left: 15px;;'><label class='mt-2 mb-2' style='font-size: 12px; color: #616576;font-weight: 400;'>" +
            "<|TASKTITLE|></label></div></div><div style='display: inline-flex;width: 100%;'><div class='lables' style='width: 45%;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Assigned to:</label></div><div class='labless' style='width: 100%;text-align: justify;padding-right: 15px;padding-left: 15px;'><label class='mt-2 mb-2' style='font-size: 12px; color: #616576;font-weight: 400;'>" +
            "<|CREATEFOR|></label></div></div><div style='display: inline-flex;width: 100%;'><div class='lables' style='width: 45%;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 400;'>Comment:</label></div><div class='labless' style='width: 100%;text-align: justify;padding-right: 15px;padding-left: 15px;'><label class='mt-2 mb-2' style='font-size: 12px; color: #616576;font-weight: 400;'>" +
            "<|COMMENT|></label></div></div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";

        public static string NewSendDeadLineMailInTimeSheet { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'><div style='background-color: #000C1D; text-align: center;'> <img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;'>Emossy TimeSheet <br> Reminder</h1><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div><p style='padding-left: 30px;font-size: 15px;color: #000C1D;font-weight: 600;margin-bottom: 10px;font-size: 15px'>Hello, It Looks Like You Missed The TimeSheet</p>" +
            "<p style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;text-align: justify;padding-right: 30px;'>Hello " +
            "<|EMPLOYEENAME|> Please find the Pending work details of your Time sheet in the attached PDF,Hope you will complete your work in an efficient time,If you had already completed your time sheet please Ignore.</p></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";



        public static string InProjectNotCreatingTask { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'>" +
            "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'><div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\"></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;font-size: 22px;padding-left: 30px;padding-right: 30px;'>Forgot to Create Task..!</h1><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div><p style='padding-left: 30px;font-size: 15px;color: #616576;font-weight: 400;text-align: justify; padding-right: 30px;'>Hello," +
            "<|EMPLOYEENAME|> you are not maintaning any task in time sheet,Please create your task in " +
            "<|PROJECTNAME|>.</p></div><div style='text-align: center;'><h4 style='font-size: 20px;'>Thank You</h4></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAMEE|></h5><p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p></div></div></body>";


        public static string MentionInPostwall { get; set; } = "<body style='display: flex;justify-content: center;height:60vh;'><tr><td><table width=95% border=0 align=center cellpadding=0 cellspacing=0 style=max-width:670px; background:#fff; border-radius:3px;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);" +
            "box-shadow:0 6px 18px 0 rgba(0,0,0,.06);padding:0 40px;\"><tr><td style=\"height:40px;\">&nbsp;</td></tr><tr><td style=\" text-align:center;\"><h1 style=\"color:#1e1e2d; font-weight:bold; margin:0;font-size:28px;\">" +
            "<|MENTIONBY|> Mention in a post </h1><p>" +
            //"<|CREATEFOR|> " +
            //"<|CREATEDATE|> " +
            "</p>" + " <br><div>"
            //"<a style='margin-top:20px;margin-botton:30px;background: #F7471E;border-color: #911924;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;text-transform: uppercase;' href='"
            //+            "<|TASKLINK|>'>View Task</a> "
            + "</div></td></tr><tr><td><table cellpadding=0 cellspacing=0 style=\"margin-top:20px;width: 65%; margin-left: 15%; border: 1px solid #ededed\"><tbody><tr><td style=\"padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)\"> Created by:</td><td style=\"padding: 10px; border-bottom: 1px solid #ededed; color: #455056;text-align:center;\">" +
            "<|MENTIONBY|> </td></tr>" +
            //"<tr><td style=\"padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64) \">Title :</td>"             "</tr> " +
            "<tr><td style=\"padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)\">Mentioned to:</td><td style=\"padding: 10px; border-bottom: 1px solid #ededed; color: #455056; text-align:center;\">" +
            "<|MENTIONFOR|> </td></tr><tr> <td style=\"padding: 10px; border-bottom: 1px solid #ededed;border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)\">Content:</td><td style=\"padding: 10px; border-bottom: 1px solid #ededed; color: #455056; text-align:center;\">" +
            "<|CONTENT|> </td> </tr> <tr> <td style=\"padding: 10px;  border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%;font-weight:500; color:rgba(0,0,0,.64)\">  </td> </tr></tbody> </table> </td></tr> <tr><td style=\"height:70px;\">&nbsp;</td> </tr> </table> </td> </tr> <br><br></div></body>";

        public static class ProjectReportMailHelper
        {
            public static string TimeSheetNotFilledProperly = "<div>" +
                "<p style='padding-left: 30px;font-size: 15px;color: #616576;font-weight: 400;text-align: justify; padding-right: 30px;'>" +
                "Hello, <br>" +
                "<|EMPLOYEENAME|>, " +
                "<br>" +
                "<br> " +
                "You have not filled time sheet please fill your time sheet without any delay." +
                "</p>" +
                "</div>";
            public static string TaskNotCreatedMail = "<p style='padding-left: 30px;font-size: 15px;color: #616576;font-weight: 400;text-align: justify; padding-right: 30px;'>" +
                "Hello, <br>" +
                "<|EMPLOYEENAME|>, " +
                "<br>" +
                "<br> " +
                "It Seems you miss to fill or updating your task on time sheet, Please update or create your daily task on time sheet. Thank you for your attention to this matter." +
                "</p>";
        }

        public static string AddResourceInProject { get; set; } = "<label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'>" +
            "<|EMPLOYEENAME|></label><br><p style='padding-left: 30px;font-size: 15px;color: #616576;font-weight: 400;text-align: justify; padding-right: 30px;'>You are added in " +
            "<|PROJECTNAME|> as a " +
            "<|DESIGNATIONNAME|> by " +
            "<|ADDEDBYNAME|>. </p><div style='margin-top: 30px;background: #F8F9FC;padding-top: 15px;padding-bottom: 15px;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 600;'>Project Details</label><br><table style='padding-left: 25px;font-size: 14px;color: #616576;padding-right: 5px;margin-top: 10px'><tr style='font-size: 15px'><td style='padding-right: 5px'>Project Name : </td><td>" +
            "<|PROJECTNAME|></td></tr><tr style='font-size: 15px'><td style='padding-right: 5px'>Project Manager : </td><td >" +
            "<|MANAGERNAME|></td></tr><tr style='font-size: 15px'><td style='padding-right: 5px'>Created By : </td><td >" +
            "<|CREATERNAME|></td></tr><tr style='font-size: 15px'><td style='padding-right: 5px'>Created Date : </td><td >" +
            "<|CREATEDDATE|></td></tr></table><br></div>";

        public static string DeleteResourceInProject { get; set; } = "<label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'> Hello,</label> <br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 20px;color: #000C1D;font-weight: 600;'>" +
            "<|EMPLOYEENAME|></label><br><p style='padding-left: 30px;font-size: 15px;color: #616576;font-weight: 400;text-align: justify; padding-right: 30px;'>You are Unassigned from ProjectName   " +
            " <u><|PROJECTNAME|></u> by  " +
            "<|DELETEDBYNAME|>. </p><div style='margin-top: 30px;background: #F8F9FC;padding-top: 15px;padding-bottom: 15px;'><label class='mt-2 mb-2' style='padding-left: 30px; font-size: 14px; color: #616576;font-weight: 600;'>Project Details</label><br><table style='padding-left: 25px;font-size: 14px;color: #616576;padding-right: 5px;margin-top: 10px'><tr style='font-size: 15px'><td style='padding-right: 5px'> Unassigned Date : </td><td>" +
            "<|DELETEDDATE|></td></tr></table><br></div>";


    }
}