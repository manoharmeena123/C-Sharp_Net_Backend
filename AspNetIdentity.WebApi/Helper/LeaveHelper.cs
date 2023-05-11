using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Leave;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentity.WebApi.Helper
{
    public class LeaveHelper
    {
        /// <summary>
        /// Created By Harshit Mitra on 14-07-2022
        /// </summary>
        /// <param name="companyId"></param>
        public static async Task CheckLeave(int companyId)
        {
            using (var _db = new ApplicationDbContext())
            {
                var checkLeavesTypes = _db.LeaveTypes.Where(x => x.CompanyId == companyId).ToList();
                if (checkLeavesTypes.Count == 0)
                {
                    List<LeaveType> leave = new List<LeaveType>
                    {
                        new LeaveType
                        {
                            LeaveTypeName = "Paid Leave",
                            Description = "This is paid leave.",
                            IsPaidLeave = true,
                            RestrictToG = false,
                            Gender = null,
                            RestrictToS = false,
                            Status = null,
                            IsReasonRequired = true,
                            IsDelatable = false,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = 0,
                            CreatedOn = System.DateTime.Now,
                            CompanyId = companyId,
                        },
                        new LeaveType
                        {
                            LeaveTypeName = "Un-Paid Leave",
                            Description = "This is un-paid leave.",
                            IsPaidLeave = false,
                            RestrictToG = false,
                            Gender = null,
                            RestrictToS = false,
                            Status = null,
                            IsReasonRequired = true,
                            IsDelatable = false,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = 0,
                            CreatedOn = System.DateTime.Now,
                            CompanyId = companyId,
                        },
                        new LeaveType
                        {
                            LeaveTypeName = "Sick Leave",
                            Description = "This is sick leave.",
                            IsPaidLeave = true,
                            RestrictToG = false,
                            Gender = null,
                            RestrictToS = false,
                            Status = null,
                            IsReasonRequired = true,
                            IsDelatable = false,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = 0,
                            CreatedOn = System.DateTime.Now,
                            CompanyId = companyId,
                        }
                    };
                    _db.LeaveTypes.AddRange(leave);
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}