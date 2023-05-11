using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Teams;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 06-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/teammaster")]
    public class TeamMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get All Team Lead

        /// <summary>
        /// Created By Harshit Mitra on 06-04-2022
        /// API >> Get >> api/teammaster/getteamlead
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getteamlead")]
        public async Task<ResponseBodyModel> GetTeamLead(int departmentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamLead = await (from e in _db.Employee
                                      join d in _db.Designation on e.DesignationId equals d.DesignationId
                                      where e.IsActive == true && e.IsDeleted == false &&
                                      e.DepartmentId == departmentId && e.CompanyId == claims.companyId &&
                                      e.OrgId == claims.orgId && d.DesignationName.Contains("lead")
                                      select new
                                      {
                                          e.EmployeeId,
                                          e.DisplayName,
                                      }).ToListAsync();

                if (teamLead.Count > 0)
                {
                    res.Message = "Lead List";
                    res.Status = true;
                    res.Data = teamLead;
                }
                else
                {
                    res.Message = "No Lead Avaliable Right Now";
                    res.Status = false;
                    res.Data = teamLead;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Team Lead

        #region Api To Add Team

        /// <summary>
        /// Created By Harshit Mitra on 06-04-2022
        /// API >> Get >> api/teammaster/addteam
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addteam")]
        public async Task<ResponseBodyModel> AddTeam(AddEditTeamDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<TeamMembers> list = new List<TeamMembers>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leadId = model.TeamMemberList.Where(y => y.IsLead == true).Select(y => y.EmployeeId).FirstOrDefault();
                var employee = _db.Employee.FirstOrDefault(x => x.EmployeeId == leadId);
                if (employee != null)
                {
                    TeamMaster obj = new TeamMaster
                    {
                        TeamName = model.TeamName,
                        TeamLeadId = employee.EmployeeId,
                        TeamLeadName = employee.DisplayName,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                    };
                    _db.TeamMasters.Add(obj);
                    await _db.SaveChangesAsync();

                    foreach (var item in model.TeamMemberList)
                    {
                        var member = _db.Employee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        if (member != null)
                        {
                            TeamMembers newobj = new TeamMembers
                            {
                                TeamMasterId = obj.TeamId,
                                EmployeeId = member.EmployeeId,
                                EmployeeName = member.DisplayName,
                                EmployeeDesignation = _db.Designation.Where(x => x.DesignationId == member.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                DesignationId = member.DesignationId,

                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.userId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            _db.TeamMembers.Add(newobj);
                            await _db.SaveChangesAsync();
                            list.Add(newobj);
                        }
                    }
                    obj.TeamMemberList = list;

                    res.Message = "Team Added";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Lead Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Team

        #region Api To Get All Active Team List

        /// <summary>
        /// Created By Harshit Mitra on 06-04-2022
        /// Modify By Harshit Mitra On 22-08-2022
        /// Bug : EM-194
        /// API >> Get >> api/teammaster/getactivelist
        /// </summary>
        [HttpGet]
        [Route("getactivelist")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetAllActiveTeamList()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                dynamic teamList;
                if (claims.orgId != 0)
                {
                    teamList = _db.TeamMasters.Where(x => x.IsActive && !x.IsDeleted &&
                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                            .Select(x => new
                            {
                                x.TeamId,
                                x.TeamName,
                                x.TeamLeadName,
                                TotalMemberCount = _db.TeamMembers.Where(z => z.TeamMasterId == x.TeamId)
                                        .ToList().Count,
                            }).ToList().OrderByDescending(x => x.TeamId).ToList();
                }
                else
                {
                    teamList = _db.TeamMasters.Where(x => x.IsActive && !x.IsDeleted &&
                            x.CompanyId == claims.companyId)
                            .Select(x => new
                            {
                                x.TeamId,
                                x.TeamName,
                                x.TeamLeadName,
                                TotalMemberCount = _db.TeamMembers.Where(z => z.TeamMasterId == x.TeamId)
                                        .ToList().Count,
                            }).ToList().OrderByDescending(x => x.TeamId).ToList();
                }
                if (teamList.Count > 0)
                {
                    res.Message = "Team List";
                    res.Status = true;
                    res.Data = teamList;
                }
                else
                {
                    res.Message = "Team List Not Found";
                    res.Status = false;
                    res.Data = teamList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Active Team List

        //#region Api To Get All Active Team List By DepartmenatId
        ///// <summary>
        ///// Created By Harshit Mitra on 06-04-2022
        ///// API >> Get >> api/teammaster/getteamlistbydepartment
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getteamlistbydepartment")]
        //public async Task<ResponseBodyModel> GetAllActiveTeamListByDepartmentId(int departmentId)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    List<TeamListByDepartmentModel> list = new List<TeamListByDepartmentModel>();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (claims.roletype == "Administrator")
        //        {
        //            list = await _db.TeamMasters.Where(x => x.IsActive == true && x.IsDeleted == false &&
        //                    x.CompanyId == claims.companyid && x.DepartmentId == departmentId)
        //                    .Select(x => new TeamListByDepartmentModel
        //                    {
        //                        TeamId = x.TeamId,
        //                        TeamName = x.TeamName,

        //                    }).ToListAsync();
        //        }
        //        else
        //        {
        //            list = await _db.TeamMasters.Where(x => x.IsActive == true && x.IsDeleted == false &&
        //                    x.CompanyId == claims.companyid && x.OrgId == claims.orgid && x.DepartmentId == departmentId)
        //                    .Select(x => new TeamListByDepartmentModel
        //                    {
        //                        TeamId = x.TeamId,
        //                        TeamName = x.TeamName,

        //                    }).ToListAsync();
        //        }
        //        if (list.Count > 0)
        //        {
        //            res.Message = "Team List";
        //            res.Status = true;
        //            res.Data = list;
        //        }
        //        else
        //        {
        //            res.Message = "Team List Not Found";
        //            res.Status = false;
        //            res.Data = list;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        #region Api To Delete Team

        /// <summary>
        /// Created By Harshit Mitra on 06-04-2022
        /// API >> Get >> api/teammaster/deleteteam
        /// </summary>
        /// <param name="teamid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteteam")]
        public async Task<ResponseBodyModel> DeleteTeam(int teamid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamMaster = await _db.TeamMasters.FirstOrDefaultAsync(x => x.TeamId == teamid && x.IsDeleted == false &&
                        x.CompanyId == claims.companyId && x.OrgId == claims.orgId && x.IsActive == true);

                if (teamMaster != null)
                {
                    teamMaster.IsActive = false;
                    teamMaster.IsDeleted = true;

                    teamMaster.DeletedBy = claims.userId;
                    teamMaster.DeletedOn = DateTime.Now;
                    var teamMember = _db.TeamMembers.Where(x => x.TeamMasterId == teamid).ToList();
                    if (teamMember.Count > 0)
                    {
                        foreach (var item in teamMember)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;
                            teamMaster.DeletedBy = claims.userId;
                            teamMaster.DeletedOn = DateTime.Now;

                            _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    _db.Entry(teamMaster).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Team Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Team Not Found or Allready Deleted";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete Team

        #region Api To Get Team Lead Name For Feedback

        /// <summary>
        /// Created By Harshit Mitra on 07-04-2022
        /// API >> Get >> api/teammaster/getteamleadnames
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getteamleadnames")]
        public async Task<ResponseBodyModel> GetTeamLeadName()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetTeamLeadNameDTO> list = new List<GetTeamLeadNameDTO>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamList = await _db.TeamMasters.Where(x => x.IsActive == true && x.IsDeleted == false &&
                        x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .Select(x => new GetTeamLeadNameDTO
                        {
                            TeamId = x.TeamId,
                            TeamLeadName = x.TeamLeadName,
                        }).ToListAsync();
                GetTeamLeadNameDTO obj = new GetTeamLeadNameDTO
                {
                    TeamId = 0,
                    TeamLeadName = "Others",
                };
                teamList.Add(obj);
                if (teamList.Count > 0)
                {
                    res.Message = "Team List";
                    res.Status = false;
                }
                else
                {
                    res.Message = "Team List Is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Team Lead Name For Feedback

        #region Api To Get Team Memeber On Behalf Of Team Id

        /// <summary>
        /// Created By Harshit Mitra on 07-04-2022
        /// API >> Get >> api/teammaster/teammemberbyteamid
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("teammemberbyteamid")]
        public async Task<object> GetTeamMemberByTeamId(int teamId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetTeamMemberDTO> list = new List<GetTeamMemberDTO>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (teamId > 0)
                {
                    if (claims.roleType == "Administrator")
                    {
                        var teamMember = await _db.TeamMembers.Where(x => x.CompanyId == claims.companyId &&
                            x.TeamMasterId == teamId && x.IsActive == true && x.IsDeleted == false)
                            .Select(x => new GetTeamMemberDTO
                            {
                                EmployeeId = x.EmployeeId,
                                EmplooyeeName = x.EmployeeName,
                                DesignationId = x.DesignationId,
                            }).ToListAsync();
                        if (teamMember.Count > 0)
                            list = teamMember;
                    }
                    else
                    {
                        var teamMember = await _db.TeamMembers.Where(x => x.CompanyId == claims.companyId && x.OrgId == claims.orgId &&
                                x.TeamMasterId == teamId && x.IsActive == true && x.IsDeleted == false)
                                .Select(x => new GetTeamMemberDTO
                                {
                                    EmployeeId = x.EmployeeId,
                                    EmplooyeeName = x.EmployeeName,
                                    DesignationId = x.DesignationId,
                                }).ToListAsync();
                        if (teamMember.Count > 0)
                            list = teamMember;
                    }
                }
                else
                {
                    var teamMember = await (from e in _db.Employee
                                            join tm in _db.TeamMembers on e.EmployeeId equals tm.EmployeeId into g
                                            from x in g.DefaultIfEmpty()
                                            where e.CompanyId == claims.companyId && e.OrgId == claims.orgId &&
                                            e.IsActive == true && e.IsDeleted == false && String.IsNullOrEmpty(x.EmployeeName)
                                            select new GetTeamMemberDTO
                                            {
                                                EmployeeId = e.EmployeeId,
                                                EmplooyeeName = e.DisplayName,
                                                DesignationId = e.DesignationId,
                                            }).ToListAsync();
                    if (teamMember.Count > 0)
                        list = teamMember;
                }
                if (list.Count > 0)
                {
                    res.Message = "Team List Found";
                    res.Status = true;
                    res.Data = list;
                }
                else
                {
                    res.Message = "No Team List Found";
                    res.Status = false;
                    res.Data = list;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Team Memeber On Behalf Of Team Id

        #region Api To Get All Team Member List

        /// <summary>
        /// Created By Harshit Mitra on 08-04-2022
        /// API >> Get >> api/teammaster/getteammemberlist
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getteammemberlist")]
        public async Task<ResponseBodyModel> GetTeamMemberList(int teamId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamMaster = await _db.TeamMasters.FirstOrDefaultAsync(x => x.TeamId == teamId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (teamMaster != null)
                {
                    var team = await _db.TeamMembers.Where(x => x.TeamMasterId == teamMaster.TeamId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.EmployeeName,
                            x.EmployeeDesignation,
                        }).ToListAsync();
                    if (team.Count > 0)
                    {
                        res.Message = "Team List";
                        res.Status = true;
                        res.Data = team;
                    }
                    else
                    {
                        res.Message = "Team Not Found";
                        res.Status = false;
                        res.Data = team;
                    }
                }
                else
                {
                    res.Message = "Team Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Team Member List

        #region Api To Edit Team

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// API >> Put >> api/teammaster/editteam
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editteam")]
        public async Task<ResponseBodyModel> EditTeam(AddEditTeamDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<TeamMembers> list = new List<TeamMembers>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var team = await _db.TeamMasters.FirstOrDefaultAsync(x => x.TeamId == model.TeamId && x.OrgId == claims.orgId &&
                            x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true);
                if (team != null)
                {
                    var leadId = model.TeamMemberList.Where(y => y.IsLead == true).Select(y => y.EmployeeId).FirstOrDefault();
                    var employee = _db.Employee.FirstOrDefault(x => x.EmployeeId == leadId);
                    if (employee != null)
                    {
                        team.TeamName = model.TeamName;
                        team.TeamLeadId = employee.EmployeeId;
                        team.TeamLeadName = employee.DisplayName;
                        var deleteMember = _db.TeamMembers.Where(x => x.TeamMasterId == team.TeamId).ToList();

                        _db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        //_db.Entry(deleteMember).State = EntityState.Deleted;
                        foreach (var item in deleteMember)
                            _db.Entry(item).State = EntityState.Deleted;
                        await _db.SaveChangesAsync();

                        foreach (var item in model.TeamMemberList)
                        {
                            var member = _db.Employee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                            if (member != null)
                            {
                                TeamMembers newobj = new TeamMembers
                                {
                                    TeamMasterId = team.TeamId,
                                    EmployeeId = member.EmployeeId,
                                    EmployeeName = member.DisplayName,
                                    EmployeeDesignation = _db.Designation.Where(x => x.DesignationId == member.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                    DesignationId = member.DesignationId,

                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.userId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                };
                                _db.TeamMembers.Add(newobj);
                                await _db.SaveChangesAsync();
                                list.Add(newobj);
                            }
                        }
                        team.TeamMemberList = list;

                        res.Message = "Team Modified";
                        res.Status = true;
                        res.Data = team;
                    }
                    else
                    {
                        res.Message = "Lead Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Team Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Edit Team

        #region Get Employee List On Team (Team Lead Name Not Be in List)

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// API >> Get >> api/teammaster/emplistinteam
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        [HttpPost]
        [Route("emplistinteam")]
        public async Task<ResponseBodyModel> GetAllEmployeeListOnTeam(Employeelistrequestdata model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive && !e.IsDeleted && model.designationId.Contains(e.DesignationId) &&
                                          model.departmentId.Contains(e.DepartmentId)
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,

                                          }).ToListAsync();

                if (employeeList.Count > 0)
                {
                    res.Message = "Employee Team List";
                    res.Status = true;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "Employee Not Found";
                    res.Status = false;
                    res.Data = employeeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class Employeelistrequestdata
        {
            public List<int> departmentId { get; set; }
            public List<int> designationId { get; set; }
        }

        #endregion Get Employee List On Team (Team Lead Name Not Be in List)

        #region Api To Get Team By Team Id (For Edit)

        /// <summary>
        /// Created By Harshit Mitra on 30-04-2022
        /// API >> Get >> api/teammaster/getteambyid
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getteambyid")]
        public async Task<ResponseBodyModel> GetTeamById(int teamId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var team = await _db.TeamMasters.FirstOrDefaultAsync(x =>
                        x.TeamId == teamId && x.CompanyId == claims.companyId &&
                        x.OrgId == claims.orgId && x.IsDeleted == false && x.IsActive == true);
                if (team != null)
                {
                    GetTeamDTO obj = new GetTeamDTO
                    {
                        TeamId = team.TeamId,
                        TeamName = team.TeamName,
                        TeamLeadId = team.TeamLeadId,
                        TeamMemberList = await _db.TeamMembers.Where(x => x.TeamMasterId == team.TeamId)
                                .Select(x => new GetTeamMemberListDTO
                                {
                                    EmployeeId = x.EmployeeId,
                                    DisplayName = x.EmployeeName,
                                    DesignationId = x.DesignationId,
                                    DesignationName = x.EmployeeDesignation,
                                    IsLead = (x.EmployeeId == team.TeamLeadId),
                                }).ToListAsync(),
                    };
                    res.Message = "Team Member List";
                    res.Status = false;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Team Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Team By Team Id (For Edit)

        #region Api To Get All Team Lead Name

        /// <summary>
        /// Created By Harshit Mitra on 10-05-2022
        /// API >> Get >> api/teammaster/allteamleadname
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allteamleadname")]
        public async Task<ResponseBodyModel> GetAllTeamLeadName()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamLead = await (from e in _db.Employee
                                      join d in _db.Designation on e.DesignationId equals d.DesignationId
                                      where e.IsActive == true && e.IsDeleted == false &&
                                      e.CompanyId == claims.companyId &&
                                      e.OrgId == claims.orgId && d.DesignationName.Contains("lead")
                                      select new
                                      {
                                          e.EmployeeId,
                                          e.DisplayName,
                                      }).ToListAsync();

                if (teamLead.Count > 0)
                {
                    res.Message = "Lead List";
                    res.Status = true;
                    res.Data = teamLead;
                }
                else
                {
                    res.Message = "No Lead Avaliable Right Now";
                    res.Status = false;
                    res.Data = teamLead;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Team Lead Name

        #region Api To Get Team List Of Current Logged In User

        /// <summary>
        /// Created By Harshit Mitra on 10-05-2022
        /// API >> Get >> api/teammaster/teamlistofcurrentuser
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("teamlistofcurrentuser")]
        public async Task<ResponseBodyModel> TeamListOfCurrentLoggedInUser()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var teamIds = await _db.TeamMembers.Where(x => x.EmployeeId == claims.employeeId &&
                        x.IsDeleted == false && x.IsActive == true)
                        .Select(x => x.TeamMasterId).Distinct().ToListAsync();
                if (teamIds.Count > 0)
                {
                    var teamlist = await _db.TeamMasters
                            .Where(x => teamIds.Contains(x.TeamId))
                            .Select(x => new
                            {
                                x.TeamId,
                                x.TeamName,
                            }).ToListAsync();

                    if (teamlist.Count > 0)
                    {
                        res.Message = "Team List";
                        res.Status = true;
                        res.Data = teamlist;
                    }
                    else
                    {
                        res.Message = "You Are Not In any Team";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "You Are Not In any Team";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Team List Of Current Logged In User

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 06-04-2022
        /// </summary>
        public class AddEditTeamDTO
        {
            public int TeamId { get; set; }
            public string TeamName { get; set; }
            public List<TeamMembers> TeamMemberList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 07-04-2022
        /// </summary>
        public class GetTeamLeadNameDTO
        {
            public int TeamId { get; set; }
            public string TeamLeadName { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 07-04-2022
        /// </summary>
        public class GetTeamMemberDTO
        {
            public int EmployeeId { get; set; }
            public string EmplooyeeName { get; set; }
            public int DesignationId { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 30-04-2022
        /// </summary>
        public class GetTeamMemberListDTO
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public int DesignationId { get; set; }
            public string DesignationName { get; set; }
            public bool IsLead { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 30-04-2022
        /// </summary>
        public class GetTeamDTO
        {
            public int TeamId { get; set; }
            public int DepartmentId { get; set; }
            public int TeamLeadId { get; set; }
            public string TeamName { get; set; }
            public List<GetTeamMemberListDTO> TeamMemberList { get; set; }
        }

        public class TeamListByDepartmentModel
        {
            public int TeamId { get; set; }
            public string TeamName { get; set; }
        }

        #endregion Helper Model Class
    }
}