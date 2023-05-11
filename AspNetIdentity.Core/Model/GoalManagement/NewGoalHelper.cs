using Newtonsoft.Json;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class NewGoalHelper
    {
        public static bool CheckGoalPermission(string stringPermission, GoalTypeConstants type, PermissionType permission)
        {
            bool returnData = false;
            var deserialize = JsonConvert.DeserializeObject<ModuleClass>(stringPermission);
            switch (type)
            {
                case GoalTypeConstants.Individual_Goal:
                    switch (permission)
                    {
                        case PermissionType.Create:
                            returnData = deserialize.Individual_Goal.Create;
                            break;
                        case PermissionType.View:
                            returnData = deserialize.Individual_Goal.View;
                            break;
                        case PermissionType.Dashboard:
                            returnData = deserialize.Individual_Goal.Dashboard;
                            break;
                        default:
                            returnData = false;
                            break;
                    }
                    break;
                case GoalTypeConstants.Departmental_Goal:
                    switch (permission)
                    {
                        case PermissionType.Create:
                            returnData = deserialize.Departmental_Goal.Create;
                            break;
                        case PermissionType.View:
                            returnData = deserialize.Departmental_Goal.View;
                            break;
                        case PermissionType.Dashboard:
                            returnData = deserialize.Departmental_Goal.Dashboard;
                            break;
                        default:
                            returnData = false;
                            break;
                    }
                    break;
                case GoalTypeConstants.Company_Goal:
                    switch (permission)
                    {
                        case PermissionType.Create:
                            returnData = deserialize.Company_Goal.Create;
                            break;
                        case PermissionType.View:
                            returnData = deserialize.Company_Goal.View;
                            break;
                        case PermissionType.Dashboard:
                            returnData = deserialize.Company_Goal.Dashboard;
                            break;
                        default:
                            returnData = false;
                            break;
                    }
                    break;
                default:
                    returnData = false;
                    break;
            }
            return returnData;
        }
        public enum PermissionType
        {
            Create = 1,
            View = 2,
            Dashboard = 3,
        }

        #region This Function Use 

        public static List<RangeListResponse> GetRangeList(int model, int rangeValue = 5)
        {
            List<RangeListResponse> list = new List<RangeListResponse>();
            int start = model == 0 ? 0 : (model / rangeValue);
            for (int i = start; (i * rangeValue) <= 100; i++)
            {
                list.Add(new RangeListResponse
                {
                    Key = i * rangeValue,
                    Value = (i * rangeValue) + "%"
                });
            }
            return list;
        }

        public class RangeListResponse
        {
            public int Key { get; set; } = 0;
            public string Value { get; set; } = "0%";
        }
        #endregion
        public static string CreateGoals { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'> " +
           "<div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:  " +
           "24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'> " +
           "<div style= 'background-color: #000C1D; text-align: center;'> " +
           "<img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
           "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Goal Details </h1>" +
           "<br><div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><label class='mt-2 mb-2' style='font-style: normal;padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'> Hello,</label> " +
           "<br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 22px;color: #000C1D;font-weight: 600;'>Dear " +
           "<|GOALASSIGNEENAME|></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;'>This goal is assigned by " +
           "<|CREATEDBY|></label><br><br><br><div class='mt-2 mb-2'style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
           "<|DESCRIPTION|></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Start Date " +
           "<span style='padding-left: 40px;'>" +
           "<|STARTDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal End Date " +
           "<span style='padding-left: 43px;'>" +
           "<|ENDDATE|></span></label><br><br><hr><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;text-align: center;padding-right: 30px;padding-bottom: 10px;padding-top: 10px;'><a style='margin-top:20px;margin-botton:20px;background: #F7471E;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;' href='" +
           "<|VIEWGOAL|>'>View Goal</a><br><br><label class='mt-2 mb-2' style='font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span></label></div></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
           "<|COMPANYNAME|></h5>" +
           "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
           "<|COMPANYADDRESS|></p> " +
           "</div></div></body>";
        public static string StatusInGoal { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'><div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:   24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
            "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
            "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Goal Accepted</h1><br>" +
            "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
            "<|DESCRIPTION|></div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;'> is accepted by " +
            "<|GOALASSIGNEENAME|></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Accepted Date <span style='padding-left: 40px;'>" +
            "<|STARTDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal End Date <span style='padding-left: 68px;'>" +
            "<|ENDDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Status <span style='padding-left: 83px;'>" +
            "<|STATUS|></span></label><br><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span>" +
            "</label></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
            "<|COMPANYNAME|></h5>" +
            "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
            "<|COMPANYADDRESS|></p> " +
            "</div></div></body>";
        public static string DocStatusInGoal { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'><div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:   24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
             "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
             "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Document Updated </h1><br>" +
             "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
             "<|DESCRIPTION|></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Accepted Date <span style='padding-left: 40px;'>" +
             "<|STARTDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal End Date <span style='padding-left: 68px;'>" +
             "<|ENDDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Status <span style='padding-left: 83px;'>" +
             "<|STATUS|></span></label><br><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span>" +
             "</label></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
             "<|COMPANYNAME|></h5>" +
             "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
             "<|COMPANYADDRESS|></p> " +
             "</div></div></body>";
        public static string ApprovedStatusInGoal { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'><div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:   24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
             "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
             "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Goal Approved</h1><br>" +
             "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
             "<|DESCRIPTION|></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Accepted Date <span style='padding-left: 40px;'>" +
             "<|STARTDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal End Date <span style='padding-left: 68px;'>" +
             "<|ENDDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Status <span style='padding-left: 83px;'>" +
             "<|STATUS|></span></label><br><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span>" +
             "</label></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
             "<|COMPANYNAME|></h5>" +
             "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
             "<|COMPANYADDRESS|></p> " +
             "</div></div></body>";
        public static string RejectedStatusInGoal { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'><div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:   24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
             "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
             "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Goal Rejected</h1><br>" +
             "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
             "<|DESCRIPTION|></div><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Accepted Date <span style='padding-left: 40px;'>" +
             "<|STARTDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal End Date <span style='padding-left: 68px;'>" +
             "<|ENDDATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Status <span style='padding-left: 83px;'>" +
             "<|STATUS|></span></label><br><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span>" +
             "</label></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
             "<|COMPANYNAME|></h5>" +
             "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
             "<|COMPANYADDRESS|></p> " +
             "</div></div></body>";

        public static string CommentInGoal { get; set; } = "<body style='display: grid;justify-content: center;font-family: sans-serif;'><div class=' flextcontainer card p-2' style='width: 100%;position: relative;margin-bottom:   24px;border: 1px solid #f2f4f9;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>" +
           "<div style='background-color: #000C1D; text-align: center;'><img style='height: 30px;padding: 10px;margin-top: 5px;' src=\"<|IMAGE_PATH|>\" ></div>" +
           "<h1 class='mt-2 mb-2' style='margin-top: 20px;margin-bottom:10px;color: #000C1D;text-align: center;'>Goal Comments</h1><br>" +
           "<div class='m-2 mb-3' style='width: 375px;padding-left: 10px;padding-right: 10px;'><div class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #000C1D;font-weight: 600;text-align: justify;padding-right: 30px;'>Goal Detail - " +
           "<|MESSAGE|></div><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 14px;color: #616576;font-weight: 400;'> is accepted by " +
           "<|GOALASSIGNEENAME|></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Comment Date <span style='padding-left: 40px;'>" +
           "<|DATE|></span></label><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 12px;color: #616576;font-weight: 400;'>Goal Status <span style='padding-left: 83px;'>" +
           "<|STATUS|></span></label><br><br><br><label class='mt-2 mb-2' style='padding-left: 30px;font-size: 9px;color: #000C1D;font-weight: 400;'>Please contact the admin or goal creator for any queries.</span>" +
           "</label></div><div style='background-color: #000C1D;margin-top: 30px;height: 100px;text-align: center;'><h5 style='margin: 0;color: #ffffff;padding: 10px;'>" +
           "<|COMPANYNAME|></h5>" +
           "<p style='margin: 0;color: #616576;padding: 10px; font-size: 14px;'>" +
           "<|COMPANYADDRESS|></p> " +
           "</div></div></body>";
    }

}