namespace AspNetIdentity.WebApi.Helper
{
    public class ShiftHelper
    {
        //    #region Add Default Shift and Timing 

        //    /// <summary>
        //    /// Created by Suraj Bundel on  10-10-2022
        //    /// </summary>
        //    /// <returns></returns>
        //    public static object AddDefaultShiftandTiming()
        //    {
        //        ResponseBodyModel res = new ResponseBodyModel();
        //        ApplicationDbContext db = new ApplicationDbContext();


        //        ShiftGroup obj = new ShiftGroup();

        //        obj.ShiftName = "Default Shift";
        //        obj.ShiftCode = "DS";
        //        obj.Description = "Default Shift by Admin";
        //        obj.IsFlexible = false;
        //        obj.IsTimingDifferent = false;
        //        obj.IsDurationDifferent = false;
        //        obj.IsDefaultShiftGroup = true;
        //        obj.IsActive = true;
        //        obj.IsDeleted = false;
        //        obj.CompanyId = claims.companyid;
        //        obj.OrgId = claims.orgid;
        //        obj.CreatedBy = claims.employeeid;
        //        obj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();

        //        var shiftdata = db.ShiftTimings.Where(x => x.IsActive && !x.IsDeleted).ToList();


        //        ShiftTiming timingobj = new ShiftTiming();
        //        timingobj.ShiftGroup.ShiftGoupId = obj.ShiftGoupId;
        //        timingobj.WeekDay = DayOfWeek.Monday;
        //        timingobj.WeekName = DayOfWeek.Monday.ToString();
        //        timingobj.StartTime = new TimeSpan(09, 0, 0);
        //        timingobj.EndTime = new TimeSpan(18, 0, 0);
        //        timingobj.BreakTime = 0;
        //        timingobj.IsActive = true;
        //        timingobj.IsDeleted = false;
        //        timingobj.CompanyId = claims.companyid;
        //        timingobj.OrgId = claims.orgid;
        //        timingobj.CreatedBy = claims.employeeid;
        //        timingobj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();

        //        timingobj.ShiftGroup.ShiftGoupId = obj.ShiftGoupId;
        //        timingobj.WeekDay = DayOfWeek.Tuesday;
        //        timingobj.WeekName = DayOfWeek.Tuesday.ToString();
        //        timingobj.StartTime = new TimeSpan(09, 0, 0);
        //        timingobj.EndTime = new TimeSpan(18, 0, 0);
        //        timingobj.BreakTime = 0;
        //        timingobj.IsActive = true;
        //        timingobj.IsDeleted = false;
        //        timingobj.CompanyId = claims.companyid;
        //        timingobj.OrgId = claims.orgid;
        //        timingobj.CreatedBy = claims.employeeid;
        //        timingobj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();
        //        timingobj.ShiftGroup.ShiftGoupId = obj.ShiftGoupId;
        //        timingobj.WeekDay = DayOfWeek.Wednesday;
        //        timingobj.WeekName = DayOfWeek.Wednesday.ToString();
        //        timingobj.StartTime = new TimeSpan(09, 0, 0);
        //        timingobj.EndTime = new TimeSpan(18, 0, 0);
        //        timingobj.BreakTime = 0;
        //        timingobj.IsActive = true;
        //        timingobj.IsDeleted = false;
        //        timingobj.CompanyId = claims.companyid;
        //        timingobj.OrgId = claims.orgid;
        //        timingobj.CreatedBy = claims.employeeid;
        //        timingobj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();
        //        timingobj.ShiftGroup.ShiftGoupId = obj.ShiftGoupId;
        //        timingobj.WeekDay = DayOfWeek.Thursday;
        //        timingobj.WeekName = DayOfWeek.Thursday.ToString();
        //        timingobj.StartTime = new TimeSpan(09, 0, 0);
        //        timingobj.EndTime = new TimeSpan(18, 0, 0);
        //        timingobj.BreakTime = 0;
        //        timingobj.IsActive = true;
        //        timingobj.IsDeleted = false;
        //        timingobj.CompanyId = claims.companyid;
        //        timingobj.OrgId = claims.orgid;
        //        timingobj.CreatedBy = claims.employeeid;
        //        timingobj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();
        //        timingobj.ShiftGroup.ShiftGoupId = obj.ShiftGoupId;
        //        timingobj.WeekDay = DayOfWeek.Friday;
        //        timingobj.WeekName = DayOfWeek.Friday.ToString();
        //        timingobj.StartTime = new TimeSpan(09, 0, 0);
        //        timingobj.EndTime = new TimeSpan(18, 0, 0);
        //        timingobj.BreakTime = 0;
        //        timingobj.IsActive = true;
        //        timingobj.IsDeleted = false;
        //        timingobj.CompanyId = claims.companyid;
        //        timingobj.OrgId = claims.orgid;
        //        timingobj.CreatedBy = claims.employeeid;
        //        timingobj.CreatedOn = DateTime.Now;
        //        db.ShiftGroups.Add(obj);
        //        db.SaveChanges();


        //        var emp = db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.OfficeEmail == officeemail && x.CompanyId == claims.companyid && x.ShiftGroupId == null).ToList();
        //        foreach (var item in emp)
        //        {
        //            if (item.ShiftGroupId == null)
        //            {
        //                item.ShiftGroupId = obj.ShiftGoupId;
        //                db.Entry(item).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }

        //        res.Message = "permission added";
        //        res.Status = true;
        //        res.Data = emp;
        //    }
        //        return res;
        //#endregion Add Module Permission List
    }

}